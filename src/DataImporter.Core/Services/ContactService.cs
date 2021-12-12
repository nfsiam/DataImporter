using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ClosedXML.Excel;
using DataImporter.Common.Extensions;
using DataImporter.Core.BusinessObjects;
using DataImporter.Core.UnitOfWorks;

namespace DataImporter.Core.Services
{
    public class ContactService : IContactService
    {
        private readonly ICoreUnitOfWork _coreUnitOfWork;

        public ContactService(ICoreUnitOfWork coreUnitOfWork)
        {
            _coreUnitOfWork = coreUnitOfWork;
        }

        public (Dictionary<string, string>[] data, int total, int totalDisplay)
            GetContacts(int groupId,
                int pageIndex,
                int pageSize,
                Guid userId,
                string searchText,
                string sortField,
                string sortOrder,
                DateTime startDate,
                DateTime endDate)
        {
            var contactEntityList = _coreUnitOfWork.Contacts
                .GetContacts(groupId, pageIndex, pageSize, userId, searchText, sortField, sortOrder, startDate,
                    endDate);

            var total = _coreUnitOfWork.Rows.GetCount(x => x.GroupId == groupId);

            if (contactEntityList.Count <= 0)
                return (new Dictionary<string, string>[] { }, total, 0);

            var totalDisplay = contactEntityList.ElementAt(0).TotalFiltered;

            var headers = GetHeaders(groupId).OrderBy(x => x.Position).ToList();

            var rows = new List<Dictionary<string, string>>();

            var contactsPerRow = contactEntityList.GroupBy(x => x.RowId);

            foreach (var cg in contactsPerRow)
            {
                var row = new Dictionary<string, string>();
                var cells = cg.OrderBy(x => x.Position).ToList();

                row.Add("createdAt", cg.FirstOrDefault()?.CreatedAt.ToString("dd-MM-yyyy hh:mm:ss"));
                for (int i = 0; i < headers.Count; i++)
                {
                    row.Add(
                        char.ToLower(headers[i].Name[0]) + headers[i].Name[1..],
                        cells[i].Value ?? ""
                    );
                }

                row.Add("action", cg.Key.ToString());
                rows.Add(row);
            }

            return (rows.ToArray(), total, totalDisplay);
        }

        public List<string> GetColumns(Guid userId, int groupId)
        {
            var columns = _coreUnitOfWork.Headers
                .Get(x => x.GroupId == groupId && x.Group.ApplicationUserId == userId, string.Empty)
                .OrderBy(x => x.Position)
                .Select(x => x.Name).ToList();
            columns.Insert(0, "CreatedAt");
            columns.Add("Action");
            return columns;
        }

        // todo: move to header service
        public List<Entities.Header> GetHeaders(int groupId)
        {
            if (groupId <= default(int))
                throw new InvalidOperationException("Invalid Group Id");

            return _coreUnitOfWork.Headers.Get(x => x.GroupId == groupId, string.Empty).ToList();
        }
    }
}