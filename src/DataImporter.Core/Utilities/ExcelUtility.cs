using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DataImporter.Common.Models;
using DataImporter.Core.Enums;
using DataImporter.Core.Exceptions;
using DataImporter.Core.Factories;
using DataImporter.Core.Services.Storage;

namespace DataImporter.Core.Utilities
{
    public class ExcelUtility : IExcelUtility
    {
        private readonly IFileService _fileService;

        public ExcelUtility(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<(List<string> headers, List<List<string>> rows, int totalRows, int totalCells)>
            ParseExcelFileAsync(string storageFileName, int rowLimit = 10)
        {
            await using var memoryStream = await _fileService.GetFileAsync(storageFileName, FileOperationType.Import);

            using var book = new XLWorkbook(memoryStream);
            var sheet = book.Worksheet(1);
            //todo: worksheet exists or not

            var cellCount = sheet.CellsUsed().Count();
            var xlRows = sheet.RowsUsed();
            var rowCount = xlRows.Count();

            if (rowCount <= 2)
            {
                throw new InvalidParameterException(
                    "The excel file should contain at least 1 row extra except the columns");
            }

            var headerXlRow = xlRows.ElementAt(0);
            var columnCount = headerXlRow.LastCellUsed().Address.ColumnNumber;
            if (columnCount < 1)
                throw new InvalidParameterException("At least one column is required");

            var headers = ExtractCellValues(headerXlRow, columnCount,
                x => string.IsNullOrEmpty(x),
                "Cells from first used row can't be Empty as they will be treated as Column Names");

            if (headers.Count != headers.Distinct().Count())
                throw new InvalidParameterException("Column names can not conatin duplicate values");

            var rows = new List<List<string>>();

            foreach (var xlRow in xlRows.Skip(1))
            {
                rows.Add(ExtractCellValues(xlRow, columnCount));
                if (--rowLimit <= 0)
                    break;
            }

            return (headers, rows, rowCount, cellCount);
        }

        private List<string> ExtractCellValues(IXLRow row, int columnCount, Predicate<string> condition = null,
            string message = "")
        {
            var cellValues = new List<string>();
            for (var i = 1; i <= columnCount; i++)
            {
                var cellValue = row.Cell(i).GetString().Trim();
                if (condition != null && condition.Invoke(cellValue))
                    throw new InvalidParameterException(message);
                else
                    cellValues.Add(cellValue);
            }

            return cellValues;
        }
    }
}