using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using DataImporter.Common.Extensions;
using DataImporter.Core.Exceptions;
using DataImporter.Core.Repositories;
using DataImporter.Core.Services;
using DataImporter.Web.Models.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace DataImporter.Web.Controllers
{
    [Authorize(Policy = "ViewPermission")]
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly ILifetimeScope _scope;

        public DashboardController(ILogger<DashboardController> logger, ILifetimeScope scope)
        {
            _logger = logger;
            _scope = scope;
        }

        public IActionResult Index()
        {
            var model = new DashboardModel();
            try
            {
                model.Resolve(_scope);
                model.LoadModelData();
            }
            catch (Exception ex)
            {
                if (ex is InvalidParameterException)
                    ModelState.AddModelError("", ex.Message);
                else
                    ModelState.AddModelError("", "Failed to load Dashboard");
                _logger.LogError(ex, "Failed to load Dashboard");
            }

            return View(model);
        }
    }
}