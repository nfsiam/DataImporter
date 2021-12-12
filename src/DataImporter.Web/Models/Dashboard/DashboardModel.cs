using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using DataImporter.Core.Exceptions;
using DataImporter.Core.Services;
using DataImporter.Membership.Extensions;
using Microsoft.AspNetCore.Http;

namespace DataImporter.Web.Models.Dashboard
{
    public class DashboardModel
    {
        private IDashboardService _dashboardService;
        private IMapper _mapper;
        private IHttpContextAccessor _httpContextAccessor;
        public int ImportCount { get; set; }
        public int ContactCount { get; set; }
        public int ExportCount { get; set; }
        public int GroupCount { get; set; }
        public int EmailCount { get; set; }

        public DashboardModel()
        {
        }

        public DashboardModel(IDashboardService dashboardService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _dashboardService = dashboardService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public void Resolve(ILifetimeScope scope)
        {
            _dashboardService = scope.Resolve<IDashboardService>();
            _mapper = scope.Resolve<IMapper>();
            _httpContextAccessor = scope.Resolve<IHttpContextAccessor>();
        }

        public void LoadModelData()
        {
            var userId = _httpContextAccessor.HttpContext!.User.GetUserId<Guid>();
            var dashboard = _dashboardService.GetDashboardStats(userId);
            _mapper.Map(dashboard, this);
        }
    }
}