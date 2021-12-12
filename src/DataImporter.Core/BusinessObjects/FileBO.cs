using System.IO;

namespace DataImporter.Core.BusinessObjects
{
    public class FileBO
    {
        public MemoryStream Stream { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string StorageFileName { get; set; }
    }
}
