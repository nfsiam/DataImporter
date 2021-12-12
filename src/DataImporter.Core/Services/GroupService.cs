using AutoMapper;
using DataImporter.Core.BusinessObjects;
using DataImporter.Core.Exceptions;
using DataImporter.Core.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataImporter.Core.Services
{
    public class GroupService : IGroupService
    {
        private readonly ICoreUnitOfWork _coreUnitOfWork;
        private readonly IMapper _mapper;

        public GroupService(ICoreUnitOfWork coreUnitOfWork, IMapper mapper)
        {
            _coreUnitOfWork = coreUnitOfWork;
            _mapper = mapper;
        }

        public void CreateGroup(Group group)
        {
            if (group is null)
                throw new InvalidParameterException("Group was not provided");
            if (ExistingName(group.ApplicationUserId, group.Name))
                throw new InvalidParameterException("A group with the name already exists");

            _coreUnitOfWork.Groups
                .Add(_mapper.Map<Entities.Group>(group));
            _coreUnitOfWork.Save();
        }


        public (IList<Group> records, int total, int totalDisplay)
            GetGroups(Guid userId, int pageIndex, int pageSize, string searchText, string sortText)
        {
            var (data, total, totalDisplay) = _coreUnitOfWork.Groups.GetDynamic(
                x => x.Name.Contains(searchText.Trim()) && x.ApplicationUserId == userId,
                sortText, string.Empty, pageIndex, pageSize);

            var groups = (from entity in data
                select _mapper.Map<Group>(entity)).ToList();
            return (groups, total, totalDisplay);
        }

        public Group GetGroup(int id)
        {
            var entity = _coreUnitOfWork.Groups.GetById(id);
            return entity != null ? _mapper.Map<Group>(entity) : null;
        }

        public void UpdateGroup(Group group)
        {
            if (group is null)
                throw new InvalidParameterException("Group was not provided");
            if (ExistingName(group.ApplicationUserId, group.Name, group.Id))
                throw new InvalidParameterException("A group with the name already exists");

            var entity = _coreUnitOfWork.Groups.GetById(group.Id);
            if (entity == null)
                throw new InvalidOperationException("Could not find Group");

            _mapper.Map(group, entity);

            _coreUnitOfWork.Save();
        }

        public void DeleteGroup(int id)
        {
            var exportGroupEntities = _coreUnitOfWork.ExportGroups.Get(x => x.GroupId == id, string.Empty);
            foreach (var eg in exportGroupEntities)
            {
                eg.GroupId = null;
            }

            _coreUnitOfWork.Groups.Remove(id);
            _coreUnitOfWork.Save();
        }

        public IList<Group> GetGroups(Guid userId, string searchText)
        {
            var groups = string.IsNullOrEmpty(searchText?.Trim())
                ? _coreUnitOfWork.Groups.Get(x => x.ApplicationUserId == userId, string.Empty)
                : _coreUnitOfWork.Groups.Get(x => x.ApplicationUserId == userId && x.Name.Contains(searchText.Trim()),
                    string.Empty);
            return groups != null
                ? (from entity in groups
                    select _mapper.Map<Group>(entity)).ToList()
                : null;
        }

        private bool ExistingName(Guid groupApplicationUserId, string groupName)
            => _coreUnitOfWork.Groups
                .GetCount(x => x.ApplicationUserId == groupApplicationUserId
                               && x.Name == groupName) > 0;

        private bool ExistingName(Guid groupApplicationUserId, string groupName, int groupId)
            => _coreUnitOfWork.Groups
                .GetCount(x => x.ApplicationUserId == groupApplicationUserId
                               && x.Name == groupName && x.Id != groupId) > 0;
    }
}