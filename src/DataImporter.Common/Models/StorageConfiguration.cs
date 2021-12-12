using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter.Common.Models
{
    public class StorageConfiguration
    {
        public string LocalStorage { get; set; }
        public string LocalImport { get; set; }
        public string LocalExport { get; set; }
        public string PreferredStorage { get; set; }
        public string AwsImport { get; set; }
        public string AwsExport { get; set; }
    }
}
