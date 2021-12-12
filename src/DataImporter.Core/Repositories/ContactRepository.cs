using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataImporter.Data;
using DataImporter.Core.Contexts;
using DataImporter.Core.Entities;
using DataImporter.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace DataImporter.Core.Repositories
{
    [Authorize(Policy ="ViewPermission")]
    public class ContactRepository: IContactRepository
    {
        private readonly DbContext _context;
        private readonly DbSet<Contact> _dbSet;
        //private readonly ILogger<ContactRepository> _logger;

        public ContactRepository(ICoreDbContext context, ILogger<ContactRepository> logger)
        {
            _context = context as DbContext;
            _dbSet = _context.Set<Contact>();
            //_logger = logger;
        }
        
        public List<Contact> GetContacts(int groupId,
            int pageIndex,
            int pageSize,
            Guid applicationUserId,
            string searchText,
            string sortField,
            string sortOrder,
            DateTime startDate,
            DateTime endDate)
        {
            if (groupId <= 0)
                throw new InvalidOperationException("Group Id is not Valid");
            if (applicationUserId == default)
                throw new InvalidOperationException("User Id is not Valid");

            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            var param = new object[]
            {
                new SqlParameter("@GroupId", groupId),
                new SqlParameter("@PageIndex", pageIndex),
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@ApplicationUserId", applicationUserId),
                new SqlParameter("@SearchText", string.IsNullOrEmpty(searchText) ? DBNull.Value : searchText),
                new SqlParameter("@SortField", string.IsNullOrEmpty(sortField) ? DBNull.Value : sortField),
                new SqlParameter("@SortOrder", string.IsNullOrEmpty(sortOrder) ? DBNull.Value : sortOrder),
                new SqlParameter("@StartDate", startDate == default ? DBNull.Value : startDate),
                new SqlParameter("@EndDate", endDate == default ? DBNull.Value : endDate),
            };

            var query = _dbSet.FromSqlRaw(
                "GET_PAGINATED_CONTACTS @GroupId,@PageIndex,@PageSize,@ApplicationUserId,@SearchText,@SortField,@SortOrder,@StartDate,@EndDate", param);

            //_logger.LogInformation(query.ToQueryString(), param);

            return query.ToList();
        }
    }
}