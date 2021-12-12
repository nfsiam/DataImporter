using Microsoft.EntityFrameworkCore;
using DataImporter.Data;
using DataImporter.Core.Contexts;
using DataImporter.Core.Entities;

namespace DataImporter.Core.Repositories
{
    public class ExportRepository : Repository<Export, int>, IExportRepository
    {
        public ExportRepository(ICoreDbContext context)
            : base(context as DbContext)
        {
        }
    }
}