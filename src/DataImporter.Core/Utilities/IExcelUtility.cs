using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataImporter.Core.Utilities
{
    public interface IExcelUtility
    {
        Task<(List<string> headers, List<List<string>> rows, int totalRows, int totalCells)>
            ParseExcelFileAsync(string storageFileName, int rowLimit = 10);
    }
}