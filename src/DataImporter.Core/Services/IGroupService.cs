using System;
using DataImporter.Core.BusinessObjects;
using System.Collections.Generic;

namespace DataImporter.Core.Services
{
    public interface IGroupService
    {
        void CreateGroup(Group group);
        (IList<Group> records, int total, int totalDisplay)
            GetGroups(Guid userId, int pageIndex, int pageSize, string searchText, string sortText);

        Group GetGroup(int id);
        void UpdateGroup(Group group);
        void DeleteGroup(int id);
        IList<Group> GetGroups(Guid userId,string searchText);
    }
}