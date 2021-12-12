namespace DataImporter.Core.BusinessObjects
{
    public class QueueMessageBO
    {
        public string MessageId { get; set; }
        public string Body { get; set; }
        public string ReceiptHandle { get; set; }
    }
}
