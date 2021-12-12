using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataImporter.Data;
using DataImporter.Core.Contexts;
using DataImporter.Core.Entities;
using Microsoft.Data.SqlClient;

namespace DataImporter.Core.Repositories
{
    public interface IContactRepository
    {
        List<Contact> GetContacts(int groupId,
            int pageIndex,
            int pageSize,
            Guid applicationUserId,
            string searchText,
            string sortField,
            string sortOrder,
            DateTime startDate,
            DateTime endDate);
    }
}