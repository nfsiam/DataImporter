using DataImporter.Core.Services.Storage;

namespace DataImporter.Core.Factories
{
    public interface IFileServiceFactory
    {
        IFileService GetFileService();
    }
}