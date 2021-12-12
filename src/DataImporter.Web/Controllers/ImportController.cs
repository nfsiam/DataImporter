using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using DataImporter.Core.Exceptions;
using DataImporter.Web.Models;
using DataImporter.Web.Models.Group;
using DataImporter.Web.Models.Import;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DataImporter.Web.Extensions;

namespace DataImporter.Web.Controllers
{
    [Authorize(Policy = "ViewPermission")]
    public class ImportController : Controller
    {
        private readonly ILogger<ImportController> _logger;
        private readonly ILifetimeScope _scope;

        public ImportController(ILogger<ImportController> logger, ILifetimeScope scope)
        {
            _logger = logger;
            _scope = scope;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        public JsonResult GetImportData()
        {
            var dataTablesModel = new DataTablesAjaxRequestModel(Request);
            var model = new ImportListModel();
            model.Resolve(_scope);
            var data = model.GetImports(dataTablesModel);
            return Json(data);
        }

        [HttpGet("Import/New-Task")]
        public IActionResult NewTask()
        {
            var model = new NewTaskModel();
            model.Resolve(_scope);
            return View(model);
        }

        [HttpPost("Import/New-Task"), ValidateAntiForgeryToken]
        public async Task<IActionResult> NewTask(NewTaskModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.Resolve(_scope);
                    var previewTaskModel = await model.LoadPreviewTaskModelAsync();
                    return PartialView("_ImportPreview", previewTaskModel);
                }
                catch (Exception ex)
                {
                    if (ex is InvalidParameterException or ColumnNameMismatchException)
                        ModelState.AddModelError("", ex.Message);
                    else
                        ModelState.AddModelError("", "Failed to initiate a new import task");
                    _logger.LogError(ex, "New Import Task Failed");
                }
            }

            return BadRequest(ModelState.GetValidationBlock());
        }

        [HttpPost("Import/Finalize-Task"), ValidateAntiForgeryToken]
        public async Task<IActionResult> FinalizeTask(FinalizeTaskModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.Resolve(_scope);
                    await model.FinalizeTaskAsync();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Task Finalization Failed");
                    _logger.LogError(ex, "Task Finalization Failed");
                }
            }

            return PartialView("_FinalizeTask", model);
        }
    }
}