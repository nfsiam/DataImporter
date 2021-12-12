using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DataImporter.Web.Extensions
{
    public static class ModelStateExtensions
    {
        public static string GetValidationBlock(this ModelStateDictionary modelState)
        {
            return
                "<ul>" 
                + string.Join("",modelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => $"<li>{e.ErrorMessage}</li>")) 
                + "</ul>";
        }
    }
}
