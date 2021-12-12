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

namespace DataImporter.Web.Models.Contact
{
    public class ContactListModel
    {
        private IContactService _contactService;
        private IHttpContextAccessor _httpContextAccessor;
        private IMapper _mapper;
        
        public ContactListModel()
        {
        }

        public void Resolve(ILifetimeScope scope)
        {
            _contactService = scope.Resolve<IContactService>();
            _httpContextAccessor = scope.Resolve<IHttpContextAccessor>();
            _mapper = scope.Resolve<IMapper>();
        }

        public ContactListModel(IContactService contactService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _contactService = contactService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public object GetContactColumns(int groupId)
        {
            
            var columns =  _contactService.GetColumns(_httpContextAccessor.HttpContext!.User.GetUserId<Guid>(),groupId);

            var list = new List<ContactColumnModel>();
            foreach (var name in columns)
            {
                list.Add(new ContactColumnModel
                {
                    Title = name,
                    Data = char.ToLower(name[0]) + name.Substring(1)
                });
            }
            return list;
        }
        public object GetContacts(DataTablesAjaxRequestModel tableModel)
        {
            var userId = _httpContextAccessor.HttpContext!.User.GetUserId<Guid>();
            var groupId = tableModel.GetRequestValue<int>("groupId");
            var (sortField, sortOrder) = 
                tableModel.GetSortFieldAndOrder(_contactService.GetColumns(userId,groupId));

            var data = _contactService.GetContacts(groupId,
                tableModel.PageIndex,
                tableModel.PageSize,
                _httpContextAccessor.HttpContext!.User.GetUserId<Guid>(),
                tableModel.SearchText,
                sortField,
                sortOrder,
                tableModel.StartDate,
                tableModel.EndDate);

            return new
            {
                recordsTotal = data.total,
                recordsFiltered = data.totalDisplay,
                data = data.data
            };
        }
    }
}