using Autofac;
using DataImporter.Core.Contexts;
using DataImporter.Core.Factories;
using DataImporter.Core.Repositories;
using DataImporter.Core.Services;
using DataImporter.Core.Services.Queue;
using DataImporter.Core.Services.Storage;
using DataImporter.Core.UnitOfWorks;
using DataImporter.Core.Utilities;

namespace DataImporter.Core.Modules
{
    public class CoreModule : Module
    {
        private readonly string _connectionString;
        private readonly string _migrationAssemblyName;

        public CoreModule(string connectionString, string migrationAssemblyName)
        {
            _connectionString = connectionString;
            _migrationAssemblyName = migrationAssemblyName;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CoreDbContext>().AsSelf()
                .WithParameter("connectionString", _connectionString)
                .WithParameter("migrationAssemblyName", _migrationAssemblyName)
                .InstancePerLifetimeScope();

            builder.RegisterType<CoreDbContext>().As<ICoreDbContext>()
                .WithParameter("connectionString", _connectionString)
                .WithParameter("migrationAssemblyName", _migrationAssemblyName)
                .InstancePerLifetimeScope();

            builder.RegisterType<GroupRepository>().As<IGroupRepository>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ImportRepository>().As<IImportRepository>()
                .InstancePerLifetimeScope();
            builder.RegisterType<HeaderRepository>().As<IHeaderRepository>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ContactRepository>().As<IContactRepository>()
                .InstancePerLifetimeScope();
            builder.RegisterType<RowRepository>().As<IRowRepository>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ExportRepository>().As<IExportRepository>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ExportGroupRepository>().As<IExportGroupRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CoreUnitOfWork>().As<ICoreUnitOfWork>()
                .InstancePerLifetimeScope();

            builder.RegisterType<GroupService>().As<IGroupService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ImportService>().As<IImportService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ContactService>().As<IContactService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ExportService>().As<IExportService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<DashboardService>().As<IDashboardService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ExcelUtility>().As<IExcelUtility>()
                .InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
