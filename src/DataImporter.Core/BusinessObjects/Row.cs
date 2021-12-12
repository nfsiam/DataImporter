using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataImporter.Data;

namespace DataImporter.Core.BusinessObjects
{
    public class Row
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public List<Cell> Cells { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
