using DataImporter.Data;
using DataImporter.Core.Repositories;

namespace DataImporter.Core.UnitOfWorks
{
    public interface ICoreUnitOfWork : IUnitOfWork
    {
        IGroupRepository Groups { get; }
        IImportRepository Imports { get; }
        IHeaderRepository Headers { get; }
        IContactRepository Contacts { get; }
        IRowRepository Rows { get; }
        IExportRepository Exports { get; }
        IExportGroupRepository ExportGroups { get; }
    }
}