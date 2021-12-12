using Autofac;
using DataImporter.Common.Models;
using DataImporter.Core.Services.Storage;

namespace DataImporter.Core.Factories
{
    public class FileServiceFactory : IFileServiceFactory
    {
        private readonly ILifetimeScope _scope;

        public FileServiceFactory(ILifetimeScope scope)
        {
            _scope = scope;
        }
        public IFileService GetFileService()
        {
            var key = _scope.Resolve<StorageConfiguration>().PreferredStorage;
            var fileService = _scope.ResolveKeyed<IFileService>(key);
            return fileService;
        }
    }
}
