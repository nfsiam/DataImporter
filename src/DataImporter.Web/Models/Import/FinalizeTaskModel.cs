using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using DataImporter.Common.Models;
using DataImporter.Core.Enums;
using DataImporter.Core.Exceptions;
using DataImporter.Core.Factories;
using CoreBO = DataImporter.Core.BusinessObjects;
using DataImporter.Core.Services;
using DataImporter.Core.Services.Storage;
using DataImporter.Membership.Extensions;
using DataImporter.Web.Services;
using Microsoft.AspNetCore.Http;

namespace DataImporter.Web.Models.Import
{
    public class FinalizeTaskModel
    {
        [Required] public int Id { get; set; }
        [Required] public string Decision { get; set; }

        private IImportService _importService;
        private IFileService _fileService;
        private IHttpContextAccessor _httpContextAccessor;

        public FinalizeTaskModel()
        {
        }

        public void Resolve(ILifetimeScope scope)
        {
            _importService = scope.Resolve<IImportService>();
            _fileService = scope.Resolve<IFileService>();
            _httpContextAccessor = scope.Resolve<IHttpContextAccessor>();
        }

        public FinalizeTaskModel(IImportService importService,
            IFileService fileService,
            IHttpContextAccessor httpContextAccessor)
        {
            _importService = importService;
            _fileService = fileService;
            _httpContextAccessor = httpContextAccessor;
        }

        internal async Task FinalizeTaskAsync()
        {
            if (Decision == "cancel")
            {
                var storageFileName = _importService
                    .CancelImport(_httpContextAccessor.HttpContext!.User.GetUserId<Guid>(), Id);

                if (!string.IsNullOrEmpty(storageFileName))
                    await _fileService.RemoveAsync(storageFileName, FileOperationType.Import);
            }
            else if (Decision == "confirm")
            {
                await _importService.ConfirmImportAsync(
                    _httpContextAccessor.HttpContext!.User.GetUserId<Guid>(), Id);
            }
        }
    }
}