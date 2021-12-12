using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToFullLocal(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy hh:mm:ss tt");
        }
    }
}
