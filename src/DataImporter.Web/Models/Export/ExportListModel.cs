using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using DataImporter.Common.Extensions;
using DataImporter.Common.Models;
using DataImporter.Core.Enums;
using DataImporter.Core.Exceptions;
using DataImporter.Core.Factories;
using DataImporter.Core.Services;
using DataImporter.Core.Services.Storage;
using DataImporter.Membership.Extensions;
using DataImporter.Web.Services;
using Microsoft.AspNetCore.Http;

namespace DataImporter.Web.Models.Export
{
    public class ExportListModel
    {
        private IExportService _exportService;
        private IHttpContextAccessor _httpContextAccessor;
        private IFileService _fileService;
        public string DisplayFileName { get; set; }
        public string Email { get; set; }
        public string GroupNames { get; set; }
        public string CreatedAt { get; set; }
        public string Status { get; set; }
        public object Action { get; set; }

        public ExportListModel()
        {
        }

        public ExportListModel(IExportService exportService, IHttpContextAccessor httpContextAccessor,
            IFileService fileService)
        {
            _exportService = exportService;
            _httpContextAccessor = httpContextAccessor;
            _fileService = fileService;
        }

        public void Resolve(ILifetimeScope scope)
        {
            _exportService = scope.Resolve<IExportService>();
            _httpContextAccessor = scope.Resolve<IHttpContextAccessor>();
            _fileService = scope.Resolve<IFileService>();
        }

        public object GetExports(DataTablesAjaxRequestModel tableModel)
        {
            var data = _exportService.GetExports(
                _httpContextAccessor.HttpContext!.User.GetUserId<Guid>(),
                tableModel.StartDate,
                tableModel.EndDate,
                tableModel.PageIndex,
                tableModel.PageSize,
                tableModel.SearchText,
                tableModel.GetSortText(new List<string>
                    {"DisplayFileName", "", "Email", "CreatedAt", "Status", "Action"})
            );
            return new
            {
                recordsTotal = data.total,
                recordsFiltered = data.totalDisplay,
                data = data.records.Select(r => new ExportListModel
                {
                    DisplayFileName = r.DisplayFileName,
                    GroupNames = r.GroupNames,
                    Email = r.Email,
                    CreatedAt = r.CreatedAt.ToFullLocal(),
                    Status = r.Status,
                    Action = new
                    {
                        Id = r.Id,
                        Status = r.Status
                    }
                })
            };
        }

        public async Task<(MemoryStream memoryStream, string contentType, string fileName)> GetFileAsync(int id)
        {
            var export = _exportService.GetExport(_httpContextAccessor.HttpContext!.User.GetUserId<Guid>(), id);
            if (export == null)
                throw new FileNotFoundException("The file does not exists");

            var memoryStream = await _fileService.GetFileAsync(export.StorageFileName, FileOperationType.Export);
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return (memoryStream, contentType, export.DisplayFileName);
        }
    }
}