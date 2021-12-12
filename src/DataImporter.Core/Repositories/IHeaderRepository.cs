using Microsoft.EntityFrameworkCore;
using DataImporter.Data;
using DataImporter.Core.Contexts;
using DataImporter.Core.Entities;

namespace DataImporter.Core.Repositories
{
    public interface IHeaderRepository : IRepository<Header, int>
    {
    }
}