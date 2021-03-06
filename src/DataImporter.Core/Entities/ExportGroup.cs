using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataImporter.Data;

namespace DataImporter.Core.Entities
{
    public class ExportGroup : IEntity<int>
    {
        public int Id { get; set; }
        public int ExportId { get; set; }
        public Export Export { get; set; }
        public int? GroupId { get; set; }
        public Group Group { get; set; }
    }
}
