using System;

namespace DataImporter.Data
{
    public interface IUnitOfWork : IDisposable
    {
        void Save();
    }
}
