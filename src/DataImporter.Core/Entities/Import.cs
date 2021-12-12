using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataImporter.Data;

namespace DataImporter.Core.Entities
{
    public class Import : IEntity<int>
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public string DisplayFileName { get; set; }
        public string StorageFileName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
    }
}
