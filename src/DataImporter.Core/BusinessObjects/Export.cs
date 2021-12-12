using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataImporter.Data;

namespace DataImporter.Core.BusinessObjects
{
    public class Export
    {
        public int Id { get; set; }
        public Guid ApplicationUserId { get; set; }
        public string DisplayFileName { get; set; }
        public string StorageFileName { get; set; }
        public string Email { get; set; }
        public string EmailStatus { get; set; }
        public int[] Groups { get; set; }
        public string GroupNames { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
