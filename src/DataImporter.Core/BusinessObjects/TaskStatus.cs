using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter.Core.BusinessObjects
{
    public static class TaskStatus
    {
        public static string Pending => "Pending";
        public static string Queued => "Queued";
        public static string Processing => "Processing";
        public static string Done => "Done";
        public static string Error => "Error";
    }
}
