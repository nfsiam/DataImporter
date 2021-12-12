using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using DataImporter.Common.Models;
using Org.BouncyCastle.Asn1.Cms;

namespace DataImporter.Common.Utilities.Aws
{
    public class SqsUtility : ISqsUtility
    {
        private readonly IAmazonSQS _sqsClient;

        public SqsUtility(IAmazonSQS sqsClient)
        {
            _sqsClient = sqsClient;
        }

        private async Task<string> GetQueueUrl(string queueName)
        {
            try
            {
                var response = await _sqsClient.GetQueueUrlAsync(new GetQueueUrlRequest
                {
                    QueueName = queueName
                });

                return response.QueueUrl;
            }
            catch (QueueDoesNotExistException)
            {
                // todo: builder pattern
                var attributes = new Dictionary<string, string> {{QueueAttributeName.VisibilityTimeout, "300"}};

                var response = await _sqsClient.CreateQueueAsync(new CreateQueueRequest
                {
                    QueueName = queueName,
                    Attributes = attributes
                });

                return response.QueueUrl;
            }
        }

        public async Task<List<(string messageId, string body, string receiptHandle)>> GetMessagesAsync(
            string queueName, int maxMessages = 1)
        {
            var request = new ReceiveMessageRequest
            {
                QueueUrl = await GetQueueUrl(queueName),
                MaxNumberOfMessages = maxMessages,
            };

            var messages = await _sqsClient.ReceiveMessageAsync(request);


            var modelMessages = messages.Messages.Select(x => (x.MessageId, x.Body, x.ReceiptHandle)).ToList();

            return modelMessages;
        }

        public async Task SendMessageAsync(string queueName, string messageBody)
        {
            var response = await _sqsClient.SendMessageAsync(new SendMessageRequest
            {
                MessageBody = messageBody,
                QueueUrl = await GetQueueUrl(queueName)
            });

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                throw new InvalidOperationException("Message sending failed!");
        }

        public async Task DeleteMessageAsync(string queueName, string receiptHandle)
        {
            await _sqsClient.DeleteMessageAsync(new DeleteMessageRequest
            {
                QueueUrl = await GetQueueUrl(queueName),
                ReceiptHandle = receiptHandle
            });
        }
    }
}