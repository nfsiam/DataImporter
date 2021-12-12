using AutoMapper;
using DataImporter.Core.BusinessObjects;
using DataImporter.Core.Exceptions;
using DataImporter.Core.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ClosedXML.Excel;
using DataImporter.Common.Models;
using DataImporter.Common.Utilities;
using DataImporter.Core.Enums;
using DataImporter.Core.Factories;
using DataImporter.Core.Properties;
using DataImporter.Core.Services.Queue;
using DataImporter.Core.Services.Storage;
using Header = DataImporter.Core.Entities.Header;
using TaskStatus = DataImporter.Core.BusinessObjects.TaskStatus;


namespace DataImporter.Core.Services
{
    public class ExportService : IExportService
    {
        private readonly ICoreUnitOfWork _coreUnitOfWork;
        private readonly IMapper _mapper;
        private readonly IDateTimeUtility _dateTimeUtility;
        private readonly IEmailService _emailService;
        private readonly IFileService _fileService;
        private readonly IQueueService _queueService;

        public ExportService(ICoreUnitOfWork coreUnitOfWork,
            IMapper mapper,
            IDateTimeUtility dateTimeUtility,
            IEmailService emailService,
            IFileService fileService,
            IQueueService queueService)
        {
            _coreUnitOfWork = coreUnitOfWork;
            _mapper = mapper;
            _dateTimeUtility = dateTimeUtility;
            _emailService = emailService;
            _fileService = fileService;
            _queueService = queueService;
        }

        public async Task CreateExportTask(Export export)
        {
            if (export is null)
                throw new InvalidOperationException("Export was not provided");
            //check valid groups with data or not
            var exportEntity = new Entities.Export();
            _mapper.Map(export, exportEntity);
            exportEntity.ExportGroups = new List<Entities.ExportGroup>();
            var strBuild = new StringBuilder();
            foreach (var groupId in export.Groups)
            {
                var group = _coreUnitOfWork.Groups.Get(
                        x => x.ApplicationUserId == export.ApplicationUserId && x.Id == groupId, string.Empty)
                    .FirstOrDefault();
                if (group == null)
                    throw new InvalidOperationException("Group was not found");
                if (_coreUnitOfWork.Rows.GetCount(x => x.GroupId == group.Id) <= 0)
                    throw new InvalidParameterException(
                        $"Group named {group.Name} has no contact in it, please exclude it from export task");

                exportEntity.ExportGroups.Add(new Entities.ExportGroup {Group = group});
                strBuild.Append(group.Name.Replace(' ', '-')).Append('_');
            }

            strBuild.Remove(strBuild.Length - 1, 1).Append($"-{_dateTimeUtility.Now:dd-MM-yyyy_hh-mm-ss}");

            exportEntity.CreatedAt = _dateTimeUtility.Now;
            exportEntity.Status = TaskStatus.Queued;
            exportEntity.EmailStatus = TaskStatus.Pending;
            exportEntity.DisplayFileName =
                string.IsNullOrEmpty(exportEntity.DisplayFileName?.Trim())
                    ? strBuild.ToString()
                    : exportEntity.DisplayFileName?.Trim().Replace(' ', '-');

            exportEntity.DisplayFileName += ".xlsx";

            _coreUnitOfWork.Exports.Add(exportEntity);
            _coreUnitOfWork.Save();
            await _queueService.EnqueueTaskAsync(exportEntity.Id, QueueTaskType.Export);
        }

        public (IList<Export> records, int total, int totalDisplay) GetExports(Guid userId, DateTime startDate,
            DateTime endDate,
            int pageIndex, int pageSize, string searchText, string sortText)
        {
            var (data, total, totalDisplay) = _coreUnitOfWork.Exports.GetDynamic(
                x => x.DisplayFileName.Contains(searchText.Trim())
                     && x.ApplicationUserId == userId
                     && x.CreatedAt.Date <= (endDate == default ? x.CreatedAt : endDate)
                     && x.CreatedAt >= (startDate == default ? x.CreatedAt : startDate.Date),
                sortText, "ExportGroups.Group", pageIndex, pageSize);

            var exports = data.Select(x => new Export
            {
                Id = x.Id,
                DisplayFileName = x.DisplayFileName,
                Email = x.Email,
                Status = x.Status,
                GroupNames = string.Join(',', x.ExportGroups.Select(y => y.Group?.Name ?? "Deleted Group")),
                CreatedAt = x.CreatedAt,
                StorageFileName = x.StorageFileName
            }).ToList();

            return (exports, total, totalDisplay);
        }

        public async Task ProcessExportQueue()
        {
            var message = await _queueService.GetQueuedTaskAsync(QueueTaskType.Export);
            if (message == null) return;

            var id = Convert.ToInt32(message.Body);

            var exportEntity = _coreUnitOfWork.Exports
                .Get(x => x.Id == id && x.Status == TaskStatus.Queued, "ExportGroups.Group.Headers").FirstOrDefault();

            if (exportEntity == null)
                return;

            exportEntity.Status = TaskStatus.Processing;
            _coreUnitOfWork.Save();

            try
            {
                using var book = new XLWorkbook();

                foreach (var exportGroup in exportEntity.ExportGroups)
                {
                    var sheet = book.AddWorksheet(exportGroup.Group.Name);
                    var headers = exportGroup.Group.Headers.OrderBy(x => x.Position).ToList();
                    var columnCount = headers.Count;

                    for (int i = 0; i < columnCount; i++)
                    {
                        sheet.Cell(1, i + 1).Value = headers.ElementAt(i).Name;
                    }

                    var rowIndex = 2;
                    var total = _coreUnitOfWork.Rows.GetCount(x => x.GroupId == exportGroup.Group.Id);
                    var pageIndex = 1;
                    var chunkSize = total >= 1000 ? 1000 : 100;
                    while (total > 0)
                    {
                        var contacts = _coreUnitOfWork.Contacts
                            .GetContacts(exportGroup.Group.Id, pageIndex, chunkSize, exportEntity.ApplicationUserId,
                                string.Empty, string.Empty, string.Empty, default, default);

                        if (contacts == null) break;

                        var contactsPerRow = contacts.GroupBy(x => x.RowId);

                        foreach (var cg in contactsPerRow)
                        {
                            var cells = cg.OrderBy(x => x.Position).ToList();
                            for (int i = 0; i < headers.Count; i++)
                            {
                                sheet.Cell(rowIndex, i + 1).Value = cells[i]?.Value ?? "";
                            }

                            rowIndex++;
                        }

                        total -= chunkSize;
                        pageIndex++;
                    }

                    sheet.Columns()
                        .AdjustToContents(); //requires shared library 'libgdiplus' or one of its dependencies
                }

                exportEntity.StorageFileName = Guid.NewGuid() + ".xlsx";

                var fileBO = new FileBO
                {
                    StorageFileName = exportEntity.StorageFileName,
                    Stream = new MemoryStream()
                };

                book.SaveAs(fileBO.Stream);

                await _fileService.UploadAsync(fileBO, FileOperationType.Export);

                exportEntity.Status = TaskStatus.Done;
                exportEntity.EmailStatus = TaskStatus.Queued;

                await _queueService.EnqueueTaskAsync(exportEntity.Id, QueueTaskType.Email);

                await _queueService.RemoveQueuedTaskAsync(QueueTaskType.Export, message);
            }
            catch (Exception)
            {
                exportEntity.Status = TaskStatus.Error;
                exportEntity.EmailStatus = TaskStatus.Error;
                throw;
            }
            finally
            {
                _coreUnitOfWork.Save();
            }
        }

        public async Task ProcessEmailQueue()
        {
            var message = await _queueService.GetQueuedTaskAsync(QueueTaskType.Email);
            if (message == null) return;

            var id = Convert.ToInt32(message.Body);

            var exportEntity = _coreUnitOfWork.Exports
                .Get(x => x.Id == id && x.Status == TaskStatus.Done && x.EmailStatus == TaskStatus.Queued,
                    "ApplicationUser")
                .FirstOrDefault();

            if (exportEntity == null)
                return;

            exportEntity.EmailStatus = TaskStatus.Processing;
            try
            {
                await using var stream = await _fileService
                    .GetFileAsync(exportEntity.StorageFileName, FileOperationType.Export);

                var subject =
                    $"File Received [{_dateTimeUtility.Now:dd-M-y hh-mm-ss}]";

                var body = string.Format(Resources.exportEmailTemplate,
                    exportEntity.ApplicationUser.Name, exportEntity.ApplicationUser.Email);

                await _emailService
                    .SendEmailAsync(exportEntity.Email, subject, body, stream, exportEntity.DisplayFileName);

                exportEntity.EmailStatus = TaskStatus.Done;

                await _queueService.RemoveQueuedTaskAsync(QueueTaskType.Email, message);
            }
            catch (Exception)
            {
                exportEntity.EmailStatus = TaskStatus.Error;
                throw;
            }
            finally
            {
                _coreUnitOfWork.Save();
            }
        }

        public Export GetExport(Guid userId, int id)
        {
            if (userId == default(Guid))
                throw new InvalidOperationException("User Id was not provided");
            if (id <= 0)
                throw new InvalidOperationException("Id is invalid");

            var exportEntity = _coreUnitOfWork.Exports.Get(x => x.ApplicationUserId == userId
                                                                && x.Id == id
                                                                && x.Status == TaskStatus.Done, string.Empty)
                .FirstOrDefault();

            return exportEntity != null
                ? _mapper.Map<Export>(exportEntity)
                : default;
        }
    }
}