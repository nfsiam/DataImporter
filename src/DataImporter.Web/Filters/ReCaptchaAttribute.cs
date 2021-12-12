using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataImporter.Web.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;


namespace DataImporter.Web.Filters
{
    public class ReCaptchaAttribute : Attribute, IFilterFactory
    {
        private readonly string _errorMessage;
        private readonly double _minScore;
        public bool IsReusable => true;
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var reCaptchaService = serviceProvider.GetService<IReCaptchaService>();
            return new ReCaptchaAsyncFilter(reCaptchaService, _minScore, _errorMessage);
        }
        public ReCaptchaAttribute(double minScore = 0, string errorMessage = "reCAPTCHA validation failed. Please try again.")
        {
            _minScore = minScore;
            _errorMessage = errorMessage;
        }
    }
}
