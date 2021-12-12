using Microsoft.EntityFrameworkCore;
using DataImporter.Data;
using DataImporter.Core.Contexts;
using DataImporter.Core.Entities;

namespace DataImporter.Core.Repositories
{
    public class GroupRepository : Repository<Group, int>, IGroupRepository
    {
        public GroupRepository(ICoreDbContext context)
            : base(context as DbContext)
        {
        }
    }
}