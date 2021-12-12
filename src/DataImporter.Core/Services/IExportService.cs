using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataImporter.Core.BusinessObjects;

namespace DataImporter.Core.Services
{
    public interface IExportService
    {
        Task CreateExportTask(Export export);
        (IList<Export> records, int total, int totalDisplay)
            GetExports(Guid userId, DateTime startDate, DateTime endDate, int pageIndex, int pageSize, string searchText, string sortText);

        Task ProcessExportQueue();
        Task ProcessEmailQueue();
        Export GetExport(Guid userId, int id);
    }
}