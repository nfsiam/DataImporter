using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataImporter.Web.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DataImporter.Web.Filters
{
    public class ReCaptchaAsyncFilter : IAsyncActionFilter
    {
        private readonly IReCaptchaService _reCaptchaService;
        private readonly double _minScore;
        private readonly string _errorMessage;

        public ReCaptchaAsyncFilter(IReCaptchaService reCaptchaService, double minScore, string errorMessage)
        {
            _reCaptchaService = reCaptchaService;
            _minScore = minScore;
            _errorMessage = errorMessage;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var result = await _reCaptchaService.VerifyAsync(context.HttpContext.Request);
            if(!result)
                context.ModelState.AddModelError("Recaptcha", _errorMessage);
            await next();
        }
    }
}
