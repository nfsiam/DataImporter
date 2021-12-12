using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter.Core.Services
{
    public interface IContactService
    {
        (Dictionary<string, string>[] data, int total, int totalDisplay)
            GetContacts(int groupId,
                int pageIndex,
                int pageSize,
                Guid userId,
                string searchText,
                string sortField,
                string sortOrder,
                DateTime startDate,
                DateTime endDate);

        List<string> GetColumns(Guid userId, int groupId);
    }
}
