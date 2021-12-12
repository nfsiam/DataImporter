using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using DataImporter.Core.Services;
using DataImporter.Membership.Extensions;
using Microsoft.AspNetCore.Http;
using DataImporter.Common.Extensions;

namespace DataImporter.Web.Models.Import
{
    public class ImportListModel
    {
        private IImportService _importService;
        private IHttpContextAccessor _httpContextAccessor;
        public IList<Core.BusinessObjects.Import> Imports { get; set; }
        public string DisplayFileName { get; set; }
        public string GroupName { get; set; }
        public string CreatedAt { get; set; }
        public string Status { get; set; }

        public ImportListModel()
        {
        }

        public ImportListModel(IImportService importService, IHttpContextAccessor httpContextAccessor)
        {
            _importService = importService;
            _httpContextAccessor = httpContextAccessor;
        }

        public void Resolve(ILifetimeScope scope)
        {
            _importService = scope.Resolve<IImportService>();
            _httpContextAccessor = scope.Resolve<IHttpContextAccessor>();
        }

        public object GetImports(DataTablesAjaxRequestModel tableModel)
        {
            var data = _importService.GetImports(
                _httpContextAccessor.HttpContext!.User.GetUserId<Guid>(),
                tableModel.StartDate,
                tableModel.EndDate,
                tableModel.PageIndex,
                tableModel.PageSize,
                tableModel.SearchText,
                tableModel.GetSortText(new List<string>{"DisplayFileName", "GroupName", "CreatedAt", "Status"})
            );
            return new
            {
                recordsTotal = data.total,
                recordsFiltered = data.totalDisplay,
                data = data.records.Select(r => new ImportListModel
                {
                    DisplayFileName = r.DisplayFileName,
                    GroupName = r.Group.Name,
                    CreatedAt = r.CreatedAt.ToFullLocal(),
                    Status = r.Status
                })
            };
        }
    }
}