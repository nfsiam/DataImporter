using AutoMapper;
using DataImporter.Core.BusinessObjects;
using DataImporter.Core.Exceptions;
using DataImporter.Core.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using ClosedXML.Excel;
using DataImporter.Common.Models;
using DataImporter.Common.Utilities;
using DataImporter.Core.Enums;
using DataImporter.Core.Factories;
using DataImporter.Core.Services.Queue;
using DataImporter.Core.Services.Storage;
using DataImporter.Core.Utilities;
using Header = DataImporter.Core.Entities.Header;
using TaskStatus = DataImporter.Core.BusinessObjects.TaskStatus;


namespace DataImporter.Core.Services
{
    public class ImportService : IImportService
    {
        private readonly ICoreUnitOfWork _coreUnitOfWork;
        private readonly IMapper _mapper;
        private readonly IDateTimeUtility _dateTimeUtility;
        private readonly IExcelUtility _excelUtility;
        private readonly IFileService _fileService;
        private readonly IQueueService _queueService;

        public ImportService(ICoreUnitOfWork coreUnitOfWork,
            IMapper mapper,
            IDateTimeUtility dateTimeUtility,
            IExcelUtility excelUtility,
            IFileService fileService,
            IQueueService queueService)
        {
            _coreUnitOfWork = coreUnitOfWork;
            _mapper = mapper;
            _dateTimeUtility = dateTimeUtility;
            _excelUtility = excelUtility;
            _fileService = fileService;
            _queueService = queueService;
        }

        public async Task<(List<string> headers, List<List<string>> rows, int totalRows, int totalCells)>
            GetImportPreviewAsync(
                Guid userId, Import import)
        {
            if (import is null)
                throw new InvalidOperationException("Import was not provided");
            if (userId == default)
                throw new InvalidOperationException("UserId was not provided");

            var groupEntity = _coreUnitOfWork.Groups
                .Get(x => x.Id == import.GroupId && x.ApplicationUserId == userId, "Headers")
                .FirstOrDefault();

            if (groupEntity is null)
                throw new InvalidOperationException("Group not found");

            var extractedData = await _excelUtility.ParseExcelFileAsync(import.StorageFileName); // todo: change
            var incomingHeaders = extractedData.headers;

            var position = 1;
            if (groupEntity.Headers.Count == 0)
                incomingHeaders.ForEach(x => groupEntity.Headers.Add(new() {Name = x, Position = position++}));

            else if (!groupEntity.Headers.OrderBy(x => x.Position).Select(x => x.Name).SequenceEqual(incomingHeaders))
                throw new ColumnNameMismatchException(
                    "The file can not be imported under this group due to column mismatch");

            import.Status = TaskStatus.Pending;
            import.CreatedAt = _dateTimeUtility.Now;

            var importEntity = _mapper.Map(import, new Entities.Import());

            _coreUnitOfWork.Imports.Add(importEntity);

            _coreUnitOfWork.Save();

            import.Id = importEntity.Id;

            return extractedData;
        }

        public (IList<Import> records, int total, int totalDisplay)
            GetImports(Guid userId, DateTime startDate, DateTime endDate, int pageIndex, int pageSize,
                string searchText, string sortText)
        {
            var (data, total, totalDisplay) = _coreUnitOfWork.Imports.GetDynamic(
                x => x.DisplayFileName.Contains(searchText.Trim())
                     && x.Group.ApplicationUserId == userId && x.Status != "Pending"
                     && x.CreatedAt.Date <= (endDate == default ? x.CreatedAt : endDate)
                     && x.CreatedAt >= (startDate == default ? x.CreatedAt : startDate.Date),
                sortText.Contains("GroupName") ? sortText.Replace("GroupName", "Group.Name") : sortText, "Group",
                pageIndex, pageSize);
            var imports = (from entity in data
                select _mapper.Map<Import>(entity)).ToList();
            return (imports, total, totalDisplay);
        }

        public async Task ProcessImportQueueAsync()
        {
            var message = await _queueService.GetQueuedTaskAsync(QueueTaskType.Import);
            if (message == null) return;

            var id = Convert.ToInt32(message.Body);

            var importEntity = _coreUnitOfWork.Imports
                .Get(x => x.Id == id && x.Status == "Queued", "Group.Headers").FirstOrDefault();

            if (importEntity == null)
                return;

            var headers = importEntity.Group.Headers.OrderBy(x => x.Position).ToList();
            var headerCount = headers.Count;

            importEntity.Status = TaskStatus.Processing;
            _coreUnitOfWork.Save(); // updating the status
            try
            {
                await using var memoryStream = await _fileService.GetFileAsync(importEntity.StorageFileName,
                    FileOperationType.Import);

                using var book = new XLWorkbook(memoryStream);
                var sheet = book.Worksheet(1);
                var xlRows = sheet.RowsUsed();
                var rowCount = xlRows.Count();

                if (rowCount <= 1)
                {
                    importEntity.Status = TaskStatus.Error;
                    _coreUnitOfWork.Save(); // updating the status
                    return;
                }

                importEntity.Group.Rows = new List<Entities.Row>();
                foreach (var xlRow in xlRows.Skip(1))
                {
                    var rowEntity = new Entities.Row
                    {
                        CreatedAt = importEntity.CreatedAt,
                        Cells = new()
                    };
                    for (var i = 1; i <= headerCount; i++)
                    {
                        var cellEntity = new Entities.Cell
                        {
                            HeaderId = headers.ElementAt(i - 1).Id,
                            Value = xlRow.Cell(i).GetString(),
                            Position = i
                        };
                        rowEntity.Cells.Add(cellEntity);
                    }

                    importEntity.Group.Rows.Add(rowEntity);
                }

                importEntity.Status = "Done";

                await _fileService.RemoveAsync(importEntity.StorageFileName,
                    FileOperationType.Import);
                await _queueService.RemoveQueuedTaskAsync(QueueTaskType.Import, message);
            }
            catch (Exception)
            {
                importEntity.Status = "Error";
                throw;
            }
            finally
            {
                _coreUnitOfWork.Save(); // finishing task
            }
        }

        public async Task ConfirmImportAsync(Guid userId, int id)
        {
            var importEntity = GetImport(userId, id);
            importEntity.Status = TaskStatus.Queued;
            importEntity.CreatedAt = _dateTimeUtility.Now;
            await _queueService.EnqueueTaskAsync(importEntity.Id, QueueTaskType.Import);
            _coreUnitOfWork.Save();
        }

        public string CancelImport(Guid userId, int id)
        {
            var importEntity = GetImport(userId, id);

            if (IsHeadersRemovable(importEntity.GroupId, importEntity.Id))
                _coreUnitOfWork.Headers.Remove(x => x.GroupId == importEntity.GroupId);

            _coreUnitOfWork.Imports.Remove(importEntity);
            _coreUnitOfWork.Save();

            return importEntity.StorageFileName;
        }

        private Entities.Import GetImport(Guid userId, int id)
        {
            if (userId == default)
                throw new InvalidOperationException("User is Invalid");

            if (id == default)
                throw new InvalidOperationException("Invalid import task reference");

            var importEntity = _coreUnitOfWork.Imports
                .Get(x => x.Id == id && x.Group.ApplicationUserId == userId, "Group")
                .FirstOrDefault();

            if (importEntity is null)
                throw new InvalidOperationException("No such import row found");

            return importEntity;
        }

        private bool IsHeadersRemovable(int groupId, int id)
            => _coreUnitOfWork.Imports.GetCount(x => x.GroupId == groupId && x.Id != id) == 0;
    }
}