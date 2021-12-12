using Autofac;
using DataImporter.Core.Services.Queue;
using DataImporter.Core.Services.Storage;

namespace DataImporter.Core.Modules
{
    public class DynamicBindingModule : Module
    {
        private readonly string _preferredStorage;

        public DynamicBindingModule(string preferredStorage)
        {
            _preferredStorage = preferredStorage;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // storage services
            if (_preferredStorage == "aws")
            {

                builder.RegisterType<AwsFileService>().As<IFileService>()
                    .InstancePerLifetimeScope();

                builder.RegisterType<AwsQueueService>().As<IQueueService>()
                    .InstancePerLifetimeScope();
            }
            else
            {
                builder.RegisterType<LocalFileService>().As<IFileService>()
                    .InstancePerLifetimeScope();

                builder.RegisterType<FakeQueueService>().As<IQueueService>()
                    .InstancePerLifetimeScope();
            }
            
            base.Load(builder);
        }
    }
}
