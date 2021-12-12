using System;
using System.Linq;
using System.Threading.Tasks;
using DataImporter.Core.BusinessObjects;
using DataImporter.Core.Enums;
using DataImporter.Core.UnitOfWorks;

namespace DataImporter.Core.Services.Queue
{
    public class FakeQueueService : IQueueService
    {
        private readonly ICoreUnitOfWork _coreUnitOfWork;

        public FakeQueueService(ICoreUnitOfWork coreUnitOfWork)
        {
            _coreUnitOfWork = coreUnitOfWork;
        }

        public async Task EnqueueTaskAsync(int taskId, QueueTaskType queueTaskType)
        {
            await Task.Delay(10);
        }

        public async Task<QueueMessageBO> GetQueuedTaskAsync(QueueTaskType queueTaskType)
        {
            var id = "0";
            switch (queueTaskType)
            {
                case QueueTaskType.Import:
                {
                    var importEntity = await Task.Run(() => _coreUnitOfWork.Imports
                        .Get(x => x.Status == "Queued", string.Empty).FirstOrDefault());
                    id = importEntity?.Id.ToString();
                    break;
                }
                case QueueTaskType.Export:
                {
                    var exportEntity = await Task.Run(() => _coreUnitOfWork.Exports
                        .Get(x => x.Status == "Queued", string.Empty).FirstOrDefault());

                    id = exportEntity?.Id.ToString();
                    break;
                }
                default:
                {
                    var exportEntity = await Task.Run(() => _coreUnitOfWork.Exports
                        .Get(x => x.Status == "Done" && x.EmailStatus == "Queued", string.Empty)
                        .FirstOrDefault());

                    id = exportEntity?.Id.ToString();
                    break;
                }
            }

            return new QueueMessageBO {Body = id};
        }


        public async Task RemoveQueuedTaskAsync(QueueTaskType queueTaskType, QueueMessageBO messageBo)
        {
            await Task.Delay(10);
        }
    }
}