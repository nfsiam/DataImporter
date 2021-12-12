using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataImporter.Data;

namespace DataImporter.Core.Entities
{
    public class Contact
    {
        public DateTime CreatedAt { get; set; }
        public int RowId { get; set; }
        public Row Row { get; set; }
        public int HeaderId { get; set; }
        public Header Header { get; set; }
        public int CellId { get; set; }
        public Cell Cell { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }
        public string Value { get; set; }
        public int TotalFiltered { get; set; }
    }
}
