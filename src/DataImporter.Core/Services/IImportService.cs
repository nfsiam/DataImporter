using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataImporter.Core.BusinessObjects;

namespace DataImporter.Core.Services
{
    public interface IImportService
    {
        Task<(List<string> headers, List<List<string>> rows, int totalRows, int totalCells)> GetImportPreviewAsync(
            Guid userId, Import import);
        Task ProcessImportQueueAsync();

        (IList<Import> records, int total, int totalDisplay)
            GetImports(Guid userId, DateTime startDate, DateTime endDate, int pageIndex, int pageSize, string searchText, string sortText);
        Task ConfirmImportAsync(Guid userId, int id);
        string CancelImport(Guid userId, int id);
    }
}