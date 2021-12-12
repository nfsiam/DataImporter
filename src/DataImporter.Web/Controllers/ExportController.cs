using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using DataImporter.Core.Exceptions;
using DataImporter.Web.Models;
using DataImporter.Web.Models.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DataImporter.Web.Extensions;
using DataImporter.Web.Models.Export;

namespace DataImporter.Web.Controllers
{
    [Authorize(Policy = "ViewPermission")]
    public class ExportController : Controller
    {
        private readonly ILogger<ExportController> _logger;
        private readonly ILifetimeScope _scope;

        public ExportController(ILogger<ExportController> logger, ILifetimeScope scope)
        {
            _logger = logger;
            _scope = scope;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult GetExportData()
        {
            var dataTablesModel = new DataTablesAjaxRequestModel(Request);
            var model = new ExportListModel();
            model.Resolve(_scope);
            var data = model.GetExports(dataTablesModel);
            return Json(data);
        }

        [HttpGet("Export/New-Task")]
        public IActionResult NewTask()
        {
            var model = new NewTaskModel();
            model.Resolve(_scope);
            return View(model);
        }

        [HttpPost("Export/New-Task")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewTask(NewTaskModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.Resolve(_scope);
                    await model.CreateExportTask();
                    ViewBag.Success = "Task Created Successfully!";
                }
                catch (Exception ex)
                {
                    if (ex is InvalidParameterException)
                        ModelState.AddModelError("", ex.Message);
                    else
                        ModelState.AddModelError("", "Failed to initiate a new import task");
                    _logger.LogError(ex, "New Export Task Failed");
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Download(int id)
        {
            if (id >= 0)
            {
                try
                {
                    var model = new ExportListModel();
                    model.Resolve(_scope);
                    var (memoryStream, contentType, fileName) = await model.GetFileAsync(id);
                    var file = memoryStream.ToArray();
                    return File(file, contentType, fileName);
                }
                catch (Exception ex)
                {
                    if (ex is InvalidParameterException || ex is FileNotFoundException)
                        ModelState.AddModelError("", ex.Message);
                    else
                        ModelState.AddModelError("", "Failed To Download File");
                    _logger.LogError(ex, "File Download Failed");
                    return NoContent();
                }
            }

            return NoContent();
        }
    }
}