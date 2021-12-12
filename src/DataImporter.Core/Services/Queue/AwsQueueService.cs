using System;
using System.Linq;
using System.Threading.Tasks;
using DataImporter.Common.Models;
using DataImporter.Common.Utilities.Aws;
using DataImporter.Core.BusinessObjects;
using DataImporter.Core.Enums;

namespace DataImporter.Core.Services.Queue
{
    public class AwsQueueService : IQueueService
    {
        private readonly ISqsUtility _sqsUtility;
        private readonly SqsConfiguration _sqsConfiguration;

        public AwsQueueService(ISqsUtility sqsUtility, SqsConfiguration sqsConfiguration)
        {
            _sqsUtility = sqsUtility;
            _sqsConfiguration = sqsConfiguration;
        }

        public async Task EnqueueTaskAsync(int taskId, QueueTaskType queueTaskType)
        {
            await _sqsUtility.SendMessageAsync(GetQueueName(queueTaskType), taskId.ToString());
        }
        public async Task<QueueMessageBO> GetQueuedTaskAsync(QueueTaskType queueTaskType)
        {
            var messages = await _sqsUtility.GetMessagesAsync(GetQueueName(queueTaskType));
            var message = messages.FirstOrDefault();
            return message != default ? new QueueMessageBO
            {
                MessageId = message.messageId,
                Body = message.body,
                ReceiptHandle = message.receiptHandle
            } : null;
        }
        public async Task RemoveQueuedTaskAsync(QueueTaskType queueTaskType, QueueMessageBO messageBo)
        {
            await _sqsUtility.DeleteMessageAsync(GetQueueName(queueTaskType), messageBo.ReceiptHandle);
        }

        private string GetQueueName(QueueTaskType queueTaskType) => queueTaskType switch
        {
            QueueTaskType.Import => _sqsConfiguration.ImportQueueName,
            QueueTaskType.Export => _sqsConfiguration.ExportQueueName,
            QueueTaskType.Email => _sqsConfiguration.EmailQueueName,
            _ => throw new InvalidOperationException("Invalid task type")
        };
    }
}
