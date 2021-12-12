using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using DataImporter.Common.Models;
using DataImporter.Common.Utilities;
using DataImporter.Core.Exceptions;
using CoreBO = DataImporter.Core.BusinessObjects;
using DataImporter.Core.Services;
using DataImporter.Membership.Extensions;
using DataImporter.Web.Services;
using DataImporter.Core.Enums;
using DataImporter.Core.Factories;
using DataImporter.Core.Services.Storage;
using Microsoft.AspNetCore.Http;

namespace DataImporter.Web.Models.Import
{
    public class NewTaskModel
    {
        [Required(ErrorMessage = "Invalid Group"), Range(1, int.MaxValue, ErrorMessage = "Invalid Group")]
        public int GroupId { get; set; }

        [Required, DisplayName("Excel File")] public IFormFile ExcelFile { get; set; }
        public string StorageFileName { get; set; }
        public string DisplayFileName { get; set; }

        private IImportService _importService;
        private IMapper _mapper;
        private IFileService _fileService;
        private IHttpContextAccessor _httpContextAccessor;
        private IFileUtility _fileUtility;

        public NewTaskModel()
        {
        }

        public NewTaskModel(IImportService importService,
            IMapper mapper,
            IFileService fileService,
            IHttpContextAccessor httpContextAccessor,
            IFileUtility fileUtility)
        {
            _importService = importService;
            _mapper = mapper;
            _fileService = fileService;
            _httpContextAccessor = httpContextAccessor;
            _fileUtility = fileUtility;
        }

        public void Resolve(ILifetimeScope scope)
        {
            _importService = scope.Resolve<IImportService>();
            _mapper = scope.Resolve<IMapper>();
            _fileService = scope.Resolve<IFileService>();
            _httpContextAccessor = scope.Resolve<IHttpContextAccessor>();
            _fileUtility = scope.Resolve<IFileUtility>();
        }

        internal async Task<PreviewTaskModel> LoadPreviewTaskModelAsync()
        {
            var file = FormatFormFile();
            var uploadedFile = await _fileService.UploadAsync(file, FileOperationType.Import);
            await uploadedFile.Stream.DisposeAsync();
            StorageFileName = uploadedFile.StorageFileName;
            DisplayFileName = uploadedFile.Name;

            try
            {
                var import = _mapper.Map<CoreBO.Import>(this);

                var previewData = await _importService.GetImportPreviewAsync(
                    _httpContextAccessor.HttpContext!.User.GetUserId<Guid>(), import);

                var previewTaskModel = new PreviewTaskModel
                {
                    Id = import.Id,
                    DisplayFileName = DisplayFileName,
                    HeadersRow = previewData.headers,
                    DataRows = previewData.rows,
                    TotalRows = previewData.totalRows,
                    TotalCells = previewData.totalCells,
                };
                return previewTaskModel;
            }
            catch (Exception)
            {
                await _fileService.RemoveAsync(StorageFileName, FileOperationType.Import);
                throw;
            }
        }

        private CoreBO.FileBO FormatFormFile()
        {
            if (ExcelFile is null)
                throw new InvalidOperationException("No file was provided");

            CoreBO.FileBO file = new();
            file.Extension = Path.GetExtension(ExcelFile.FileName);
            file.Name = _fileUtility.GetSanitizedName(ExcelFile.FileName);
            file.StorageFileName = Guid.NewGuid() + file.Extension;
            file.Stream = new MemoryStream();
            ExcelFile.CopyTo(file.Stream);

            return file;
        }
    }
}