using System.Threading.Tasks;
using DataImporter.Common.Models;
using DataImporter.Core.BusinessObjects;
using DataImporter.Core.Enums;

namespace DataImporter.Core.Services.Queue
{
    public interface IQueueService
    {
        Task EnqueueTaskAsync(int taskId, QueueTaskType queueTaskType);
        Task<QueueMessageBO> GetQueuedTaskAsync(QueueTaskType queueTaskType);
        Task RemoveQueuedTaskAsync(QueueTaskType queueTaskType, QueueMessageBO messageBo);
    }
}