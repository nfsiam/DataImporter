using System.IO;
using System.Threading.Tasks;

namespace DataImporter.Common.Utilities
{
    public interface IEmailService
    {
        Task SendEmailAsync(string receiver, string subject, string body);
        Task SendEmailAsync(string receiver, string subject, string body, MemoryStream stream, string fileName = "");
    }
}
