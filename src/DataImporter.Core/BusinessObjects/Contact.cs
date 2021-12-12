using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataImporter.Data;

namespace DataImporter.Core.BusinessObjects
{
    public class Contact
    {
        public DateTime CreatedAt { get; set; }
        public int RowId { get; set; }
        public int HeaderId { get; set; }
        public int CellId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
