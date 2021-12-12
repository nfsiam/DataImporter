using System;
using System.ComponentModel.DataAnnotations;
using Autofac;
using AutoMapper;
using DataImporter.Core.Services;
using DataImporter.Membership.Extensions;
using Microsoft.AspNetCore.Http;

namespace DataImporter.Web.Models.Group
{
    public class EditGroupModel
    {
        [Required, Range(1, int.MaxValue)] public int Id { get; set; }

        [Required, MaxLength(100, ErrorMessage = "Name should be less than 100 characters")]
        public string Name { get; set; }

        private IGroupService _groupService;
        private IMapper _mapper;
        private IHttpContextAccessor _httpContextAccessor;

        public EditGroupModel()
        {
        }

        public void Resolve(ILifetimeScope scope)
        {
            _groupService = scope.Resolve<IGroupService>();
            _httpContextAccessor = scope.Resolve<IHttpContextAccessor>();
            _mapper = scope.Resolve<IMapper>();
        }

        public EditGroupModel(IGroupService groupService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _groupService = groupService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public void LoadModelData(int id)
        {
            var group = _groupService.GetGroup(id);
            if (group is null) return;
            _mapper.Map(group, this);
        }

        internal void Update()
        {
            var groupBO = _mapper.Map<Core.BusinessObjects.Group>(this);
            groupBO.ApplicationUserId = _httpContextAccessor.HttpContext!.User.GetUserId<Guid>();
            _groupService.UpdateGroup(groupBO);
        }
    }
}