using System;
using Autofac;
using CoreBO = DataImporter.Core.BusinessObjects;
using DataImporter.Core.Services;
using DataImporter.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using DataImporter.Membership.Extensions;

namespace DataImporter.Web.Models.Group
{
    public class GroupListModel
    {
        private IGroupService _groupService;
        private IHttpContextAccessor _httpContextAccessor;
        private IMapper _mapper;

        public int Id { get; set; }
        public string Name { get; set; }

        public GroupListModel()
        {
        }

        public void Resolve(ILifetimeScope scope)
        {
            _groupService = scope.Resolve<IGroupService>();
            _httpContextAccessor = scope.Resolve<IHttpContextAccessor>();
            _mapper = scope.Resolve<IMapper>();
        }

        public GroupListModel(IGroupService groupService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _groupService = groupService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public IList<GroupListModel> GetGroups(string searchText)
        {
            var groups = _groupService.GetGroups(_httpContextAccessor.HttpContext!.User.GetUserId<Guid>(), searchText);
            return groups.Select(x => _mapper.Map(x, new GroupListModel())).ToList();
        }

        public object GetGroups(DataTablesAjaxRequestModel tableModel)
        {
            var data = _groupService.GetGroups(
                _httpContextAccessor.HttpContext!.User.GetUserId<Guid>(),
                tableModel.PageIndex,
                tableModel.PageSize,
                tableModel.SearchText,
                tableModel.GetSortText(new List<string> {"Name"})
            );
            return new
            {
                recordsTotal = data.total,
                recordsFiltered = data.totalDisplay,
                data = data.records.Select(x => _mapper.Map(x, this))
            };
        }

        internal void Delete(int id)
        {
            _groupService.DeleteGroup(id);
        }
    }
}