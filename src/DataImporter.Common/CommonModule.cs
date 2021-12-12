using Autofac;
using DataImporter.Common.Utilities;
using DataImporter.Common.Utilities.Aws;

namespace DataImporter.Common
{
    public class CommonModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DateTimeUtility>().As<IDateTimeUtility>()
                .InstancePerLifetimeScope();

            builder.RegisterType<FileUtility>().As<IFileUtility>()
                .InstancePerLifetimeScope();

            builder.RegisterType<S3Utility>().As<IS3Utility>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SqsUtility>().As<ISqsUtility>()
                .InstancePerLifetimeScope();

            builder.RegisterType<EmailService>().As<IEmailService>()
                .InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}