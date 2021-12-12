using Microsoft.EntityFrameworkCore;
using DataImporter.Data;
using DataImporter.Core.Contexts;
using DataImporter.Core.Entities;

namespace DataImporter.Core.Repositories
{
    public class HeaderRepository : Repository<Header, int>, IHeaderRepository
    {
        public HeaderRepository(ICoreDbContext context)
            : base(context as DbContext)
        {
        }
    }
}