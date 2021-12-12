using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using DataImporter.Core.Services;
using DataImporter.Web.Models;
using DataImporter.Web.Models.Contact;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace DataImporter.Web.Controllers
{
    [Authorize(Policy = "ViewPermission")]
    public class ContactController : Controller
    {
        private readonly ILogger<ContactController> _logger;
        private readonly ILifetimeScope _scope;

        public ContactController(ILifetimeScope scope, ILogger<ContactController> logger)
        {
            _logger = logger;
            _scope = scope;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult GetContactColumns(int groupId)
        {
            try
            {
                if (groupId <= 0)
                {
                    return default;
                }

                var model = new ContactListModel();
                model.Resolve(_scope);
                var data = model.GetContactColumns(groupId);
                return Json(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DataTable JSON Response Failed");
            }

            return Json(null);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult GetContactData()
        {
            try
            {
                var dataTablesModel = new DataTablesAjaxRequestModel(Request);
                var model = new ContactListModel();
                model.Resolve(_scope);
                var data = model.GetContacts(dataTablesModel);
                return Json(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DataTable JSON Response Failed");
            }

            return Json(null);
        }
    }
}