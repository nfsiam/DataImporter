using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter.Common.Models
{
    public class SqsConfiguration
    {
        public string ImportQueueName { get; set; }
        public string ExportQueueName { get; set; }
        public string EmailQueueName { get; set; }
    }
}
