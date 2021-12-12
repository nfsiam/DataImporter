using System.Collections.Generic;
using System.Threading.Tasks;
using DataImporter.Common.Models;

namespace DataImporter.Common.Utilities.Aws
{
    public interface ISqsUtility
    {
        Task<List<(string messageId, string body, string receiptHandle)>> GetMessagesAsync(string queueName, int maxMessages = 1);
        Task SendMessageAsync(string queueName, string messageBody);
        Task DeleteMessageAsync(string queueName, string receiptHandle);
    }
}