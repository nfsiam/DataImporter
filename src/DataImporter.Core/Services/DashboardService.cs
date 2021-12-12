using AutoMapper;
using DataImporter.Core.BusinessObjects;
using DataImporter.Core.Exceptions;
using DataImporter.Core.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataImporter.Core.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ICoreUnitOfWork _coreUnitOfWork;

        public DashboardService(ICoreUnitOfWork coreUnitOfWork)
        {
            _coreUnitOfWork = coreUnitOfWork;
        }

        public Dashboard GetDashboardStats(Guid userId)
        {
            if (userId == default)
                throw new InvalidOperationException("User is not Valid");

            var dashboard = new Dashboard
            {
                GroupCount = _coreUnitOfWork.Groups.GetCount(x => x.ApplicationUserId == userId),
                ContactCount = _coreUnitOfWork.Rows.GetCount(x => x.Group.ApplicationUserId == userId),
                ImportCount = _coreUnitOfWork.Imports.GetCount(x => x.Group.ApplicationUserId == userId),
                ExportCount = _coreUnitOfWork.Exports.GetCount(x => x.ApplicationUserId == userId),
                EmailCount = _coreUnitOfWork.Exports
                    .GetCount(x => x.EmailStatus == TaskStatus.Done && x.ApplicationUserId == userId)
            };
            return dashboard;
        }
    }
}