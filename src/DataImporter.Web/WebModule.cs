using Autofac;
using DataImporter.Web.Models.Dashboard;
using DataImporter.Web.Models.Group;
using DataImporter.Web.Models.Import;
using DataImporter.Web.Models.Contact;
using DataImporter.Web.Services;

namespace DataImporter.Web
{
    public class WebModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ReCaptchaService>().As<IReCaptchaService>().InstancePerLifetimeScope();

            //builder.RegisterType<FileService>().As<IFileService>()
            //    .InstancePerLifetimeScope();

            builder.RegisterType<GroupListModel>().AsSelf();
            builder.RegisterType<CreateGroupModel>().AsSelf();
            builder.RegisterType<EditGroupModel>().AsSelf();

            builder.RegisterType<ImportListModel>().AsSelf();
            builder.RegisterType<NewTaskModel>().AsSelf();
            builder.RegisterType<FinalizeTaskModel>().AsSelf();
            builder.RegisterType<PreviewTaskModel>().AsSelf();

            builder.RegisterType<Models.Export.NewTaskModel>().AsSelf();
            builder.RegisterType<Models.Export.ExportListModel>().AsSelf();

            builder.RegisterType<ContactListModel>().AsSelf();

            builder.RegisterType<DashboardModel>().AsSelf();

            base.Load(builder);
        }
    }
}