using Microsoft.EntityFrameworkCore;
using DataImporter.Data;
using DataImporter.Core.Contexts;
using DataImporter.Core.Entities;

namespace DataImporter.Core.Repositories
{
    public class ImportRepository : Repository<Import, int>, IImportRepository
    {
        public ImportRepository(ICoreDbContext context)
            : base(context as DbContext)
        {
        }
    }
}