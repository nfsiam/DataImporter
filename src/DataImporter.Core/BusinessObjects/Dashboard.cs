using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter.Core.BusinessObjects
{
    public class Dashboard
    {
        public int ImportCount { get; set; }
        public int ContactCount { get; set; }
        public int ExportCount { get; set; }
        public int GroupCount { get; set; }
        public int EmailCount { get; set; }
    }
}