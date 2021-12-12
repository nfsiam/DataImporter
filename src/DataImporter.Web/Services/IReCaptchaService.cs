using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DataImporter.Web.Services
{
    public interface IReCaptchaService
    {
        Task<bool> VerifyAsync(HttpRequest request);
    }
}