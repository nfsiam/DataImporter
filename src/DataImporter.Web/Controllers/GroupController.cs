using System;
using Autofac;
using DataImporter.Core.Exceptions;
using DataImporter.Web.Models;
using DataImporter.Web.Models.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DataImporter.Web.Controllers
{
    [Authorize(Policy = "ViewPermission")]
    public class GroupController : Controller
    {
        private readonly ILogger<GroupController> _logger;
        private readonly ILifetimeScope _scope;

        public GroupController(ILifetimeScope scope, ILogger<GroupController> logger)
        {
            _logger = logger;
            _scope = scope;
        }

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult GetGroupData()
        {
            var dataTablesModel = new DataTablesAjaxRequestModel(Request);
            var model = new GroupListModel();
            model.Resolve(_scope);
            var data = model.GetGroups(dataTablesModel);
            return Json(data);
        }

        public IActionResult GetAll(string term)
        {
            var model = new GroupListModel();
            model.Resolve(_scope);
            return Ok(model.GetGroups(term));
        }

        public IActionResult Create()
        {
            var model = new CreateGroupModel();
            model.Resolve(_scope);
            if (TempData.ContainsKey("Success"))
            {
                ViewBag.Success = TempData["Success"];
            }

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(CreateGroupModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.Resolve(_scope);
                    model.CreateGroup();
                    TempData["Success"] = "Group Created Successfully";
                    return RedirectToAction(nameof(Create));
                }
                catch (Exception ex)
                {
                    if (ex is InvalidParameterException or InvalidOperationException)
                        ModelState.AddModelError("", ex.Message);
                    else
                        ModelState.AddModelError("", "Failed to create Group");
                    _logger.LogError(ex, "Create Group Failed");
                }
            }

            return View(model);
        }


        public IActionResult Edit(int id)
        {
            var model = new EditGroupModel();
            model.Resolve(_scope);
            model.LoadModelData(id);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(EditGroupModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.Resolve(_scope);
                    model.Update();
                    ViewBag.Success = "Group Updated Successfully";
                }
                catch (Exception ex)
                {
                    if (ex is InvalidParameterException or InvalidOperationException)
                        ModelState.AddModelError("", ex.Message);
                    else
                        ModelState.AddModelError("", "Failed to update Group");
                    _logger.LogError(ex, "Update Group Failed");
                }
            }

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var model = new GroupListModel();
            model.Resolve(_scope);
            model.Delete(id);

            return RedirectToAction(nameof(Index));
        }
    }
}