using System;
using DataImporter.Core.BusinessObjects;

namespace DataImporter.Core.Services
{
    public interface IDashboardService
    {
        Dashboard GetDashboardStats(Guid userId);
    }
}