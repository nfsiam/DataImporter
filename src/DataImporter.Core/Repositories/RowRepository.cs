using Microsoft.EntityFrameworkCore;
using DataImporter.Data;
using DataImporter.Core.Contexts;
using DataImporter.Core.Entities;

namespace DataImporter.Core.Repositories
{
    public class RowRepository : Repository<Row, int>, IRowRepository
    {
        public RowRepository(ICoreDbContext context)
            : base(context as DbContext)
        {
        }
    }
}