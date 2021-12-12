using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.SQS;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DataImporter.Common;
using DataImporter.Common.Models;
using DataImporter.Core;
using DataImporter.Core.Modules;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace DataImporter.EmailWorker
{
    public class Program
    {
        private static string _connectionString;
        private static string _migrationAssemblyName;
        private static IConfiguration _configuration;
        public static void Main(string[] args)
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables()
                .Build();

            _connectionString = _configuration.GetConnectionString("DefaultConnection");

            _migrationAssemblyName = typeof(Worker).Assembly.FullName;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(_configuration)
                .CreateLogger();
            try
            {
                Log.Information("Application Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .UseSerilog()
                .ConfigureContainer<ContainerBuilder>(builder => {
                    builder.RegisterModule(new CoreModule(_connectionString,
                        _migrationAssemblyName));

                    builder.RegisterModule(new DynamicBindingModule(_configuration
                        .GetSection("Storage:PreferredStorage").Value));

                    builder.RegisterModule(new CommonModule());

                    builder.Register(p => _configuration.GetSection("Smtp")
                        .Get<SmtpConfiguration>()).InstancePerLifetimeScope();

                    builder.Register(p => _configuration.GetSection("Storage")
                        .Get<StorageConfiguration>()).InstancePerDependency();

                    builder.Register(p => _configuration.GetSection("S3BucketConfiguration")
                        .Get<S3BucketConfiguration>()).InstancePerDependency();

                    builder.Register(p => _configuration.GetSection("SqsConfiguration")
                        .Get<SqsConfiguration>()).InstancePerDependency();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
                    services.AddAWSService<IAmazonS3>();
                    services.AddAWSService<IAmazonSQS>();
                });
    }
}
