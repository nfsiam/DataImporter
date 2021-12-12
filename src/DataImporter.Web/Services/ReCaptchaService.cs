using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DataImporter.Web.Models.ReCaptcha;
using Microsoft.AspNetCore.Http;

namespace DataImporter.Web.Services
{
    public class ReCaptchaService : IReCaptchaService
    {
        private readonly Credential _credential;

        public ReCaptchaService(Credential credential)
            => _credential = credential;
        public async Task<bool> VerifyAsync(HttpRequest request)
        {
            if (!request.Form.ContainsKey("g-recaptcha-response"))
                return false;
            
            var gResponseClient = request.Form["g-recaptcha-response"];
            string gResponseServer = null;
            using (var httpClient = new HttpClient())
            {
                gResponseServer = await httpClient.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret={_credential.Secret}&response={gResponseClient}");
            }
            var response = JsonSerializer.Deserialize<Response>(gResponseServer, new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
            if (!response.Success)
                return false;
            return true;
        }
    }
}
