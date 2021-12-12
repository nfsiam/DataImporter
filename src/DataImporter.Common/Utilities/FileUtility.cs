using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter.Common.Utilities
{
    public class FileUtility : IFileUtility
    {
        public string GetSanitizedName(string fileName)
        {
            var invalids = Path.GetInvalidFileNameChars();
            var sanitizedFileName = string.Join("_", fileName.Split(invalids, StringSplitOptions.RemoveEmptyEntries))
                .TrimEnd('.');
            return sanitizedFileName;
        }
    }
}
