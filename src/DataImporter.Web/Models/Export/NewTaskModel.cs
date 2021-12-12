using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using DataImporter.Core.Services;
using DataImporter.Membership.Extensions;
using DataImporter.Web.Models.Import;
using DataImporter.Web.Services;
using Microsoft.AspNetCore.Http;
using CoreBO = DataImporter.Core.BusinessObjects;

namespace DataImporter.Web.Models.Export
{
    public class NewTaskModel
    {
        [Required(ErrorMessage = "Invalid Group"), MinLength(1, ErrorMessage = "Select At least One Group")]
        public int[] Groups { get; set; }

        [MinLength(1)] public string DisplayFileName { get; set; }
        [EmailAddress] public string Email { get; set; }

        private IExportService _exportService;
        private IMapper _mapper;
        private IHttpContextAccessor _httpContextAccessor;

        public NewTaskModel()
        {
        }

        public NewTaskModel(IExportService exportService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _exportService = exportService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public void Resolve(ILifetimeScope scope)
        {
            _exportService = scope.Resolve<IExportService>();
            _mapper = scope.Resolve<IMapper>();
            _httpContextAccessor = scope.Resolve<IHttpContextAccessor>();
        }

        internal async Task CreateExportTask()
        {
            var export = _mapper.Map<CoreBO.Export>(this);
            export.ApplicationUserId = _httpContextAccessor.HttpContext!.User.GetUserId<Guid>();
            export.Email ??= _httpContextAccessor.HttpContext.User.Identity!.Name;
            await _exportService.CreateExportTask(export);
        }
    }
}