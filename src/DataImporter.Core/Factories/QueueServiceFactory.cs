using Autofac;
using DataImporter.Common.Models;
using DataImporter.Core.Services.Queue;

namespace DataImporter.Core.Factories
{
    public class QueueServiceFactory : IQueueServiceFactory
    {
        private readonly ILifetimeScope _scope;

        public QueueServiceFactory(ILifetimeScope scope)
        {
            _scope = scope;
        }
        public IQueueService GetQueueService()
        {
            var key = _scope.Resolve<StorageConfiguration>().PreferredStorage;
            var queueService = _scope.ResolveKeyed<IQueueService>(key);
            return queueService;
        }
    }
}
