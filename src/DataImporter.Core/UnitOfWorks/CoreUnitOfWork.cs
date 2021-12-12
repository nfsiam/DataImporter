using Microsoft.EntityFrameworkCore;
using DataImporter.Data;
using DataImporter.Core.Contexts;
using DataImporter.Core.Repositories;

namespace DataImporter.Core.UnitOfWorks
{
    public class CoreUnitOfWork : UnitOfWork, ICoreUnitOfWork
    {
        public IGroupRepository Groups { get; private set; }
        public IImportRepository Imports { get; private set; }
        public IHeaderRepository Headers { get; private set; }
        public IContactRepository Contacts { get; private set; }
        public IRowRepository Rows { get; private set; }
        public IExportRepository Exports { get; private set; }
        public IExportGroupRepository ExportGroups { get; private set; }


        public CoreUnitOfWork(ICoreDbContext context,
            IGroupRepository groups,
            IImportRepository imports,
            IHeaderRepository headers,
            IContactRepository contacts,
            IRowRepository rows,
            IExportRepository exports,
            IExportGroupRepository exportGroups
            ) : base(context as DbContext)
        {
            Groups = groups;
            Imports = imports;
            Headers = headers;
            Contacts = contacts;
            Rows = rows;
            Exports = exports;
            ExportGroups = exportGroups;
        }

    }
}