using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataImporter.Data;

namespace DataImporter.Core.Entities
{
    public class Cell : IEntity<int>
    {
        public int Id { get; set; }
        public int RowId { get; set; }
        public Row Row { get; set; }
        public int HeaderId { get; set; }
        public Header Header { get; set; }
        public string Value { get; set; }
        public int Position { get; set; }
    }
}
