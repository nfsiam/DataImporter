using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataImporter.Data;
using DataImporter.Membership.Entities;

namespace DataImporter.Core.Entities
{
    public class Export : IEntity<int>
    {
        public int Id { get; set; }
        public Guid ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string DisplayFileName { get; set; }
        public string StorageFileName { get; set; }
        public string Email { get; set; }
        public string EmailStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public  IList<ExportGroup> ExportGroups { get; set; }
    }
}
