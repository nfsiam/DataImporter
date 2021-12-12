using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataImporter.Data;
using DataImporter.Membership.Entities;

namespace DataImporter.Core.Entities
{
    public class Group : IEntity<int>
    {
        public int Id { get; set; }
        public Guid ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string Name { get; set; }
        public IList<Import> Imports { get; set; }
        public IList<Header> Headers { get; set; }
        public IList<Row> Rows { get; set; }
    }
}
