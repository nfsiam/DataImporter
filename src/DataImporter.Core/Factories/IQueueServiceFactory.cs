using DataImporter.Core.Services.Queue;

namespace DataImporter.Core.Factories
{
    public interface IQueueServiceFactory
    {
        IQueueService GetQueueService();
    }
}