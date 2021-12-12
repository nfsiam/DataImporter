using Microsoft.EntityFrameworkCore;
using DataImporter.Data;
using DataImporter.Core.Contexts;
using DataImporter.Core.Entities;

namespace DataImporter.Core.Repositories
{
    public class ExportGroupRepository : Repository<ExportGroup, int>, IExportGroupRepository
    {
        public ExportGroupRepository(ICoreDbContext context)
            : base(context as DbContext)
        {
        }
    }
}