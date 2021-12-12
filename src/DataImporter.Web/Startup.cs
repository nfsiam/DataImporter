using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.SQS;
using DataImporter.Common;
using DataImporter.Common.Models;
using DataImporter.Core;
using DataImporter.Core.Contexts;
using DataImporter.Core.Modules;
using DataImporter.Membership;
using DataImporter.Membership.BusinessObjects;
using DataImporter.Membership.Contexts;
using DataImporter.Membership.Entities;
using DataImporter.Membership.Services;
using DataImporter.Web.Models;
using DataImporter.Web.Models.ReCaptcha;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ApplicationUser = DataImporter.Membership.Entities.ApplicationUser;

namespace DataImporter.Web
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            WebHostEnvironment = env;

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; set; }
        public ILifetimeScope AutofacContainer { get; set; }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // session settings
            services.AddDistributedMemoryCache(); // from microsoft docs
            var connectionInfo = GetConnectionStringAndAssemblyName();

            services.AddDbContext<CoreDbContext>(options =>
                options.UseSqlServer(connectionInfo.connectionString, b =>
                    b.MigrationsAssembly(connectionInfo.migrationAssemblyName)));

            // Identity customization started here
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionInfo.connectionString, b =>
                    b.MigrationsAssembly(connectionInfo.migrationAssemblyName)));

            services
                .AddIdentity<ApplicationUser, Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<UserManager>()
                .AddRoleManager<RoleManager>()
                .AddSignInManager<SignInManager>()
                .AddDefaultUI()
                .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;

                // SignIn settings
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedAccount = true;

            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(100);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // cookie settings
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.Cookie.MaxAge = TimeSpan.FromHours(2);
                options.ExpireTimeSpan = TimeSpan.FromHours(2);

                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminAccess", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Admin");
                });

                options.AddPolicy("RestrictedArea", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("view_permission", "true");
                });

                options.AddPolicy("ViewPermission", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new ViewRequirement());
                });
            });

            // needed for the ViewPermission claim based policy
            services.AddSingleton<IAuthorizationHandler, ViewRequirementHandler>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();
            services.AddRazorPages();
            services.AddDatabaseDeveloperPageExceptionFilter();

            // aws
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();
            services.AddAWSService<IAmazonSQS>();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var connectionInfo = GetConnectionStringAndAssemblyName();
            builder.RegisterModule(new WebModule());
            builder.RegisterModule(new CommonModule());
            builder.RegisterModule(new CoreModule(connectionInfo.connectionString,
                connectionInfo.migrationAssemblyName));
            builder.RegisterModule(new DynamicBindingModule(Configuration
                .GetSection("Storage:PreferredStorage").Value));
            builder.RegisterModule(new MembershipModule(connectionInfo.connectionString,
                connectionInfo.migrationAssemblyName));
            builder.Register(p => Configuration.GetSection("Smtp").Get<SmtpConfiguration>()).InstancePerLifetimeScope();
            builder.Register(p => Configuration.GetSection("ReCaptcha").Get<Credential>()).InstancePerDependency();
            builder.Register(p => Configuration.GetSection("Storage").Get<StorageConfiguration>()).InstancePerDependency();
            builder.Register(p => Configuration.GetSection("S3BucketConfiguration")
                .Get<S3BucketConfiguration>()).InstancePerDependency();
            builder.Register(p => Configuration.GetSection("SqsConfiguration")
                .Get<SqsConfiguration>()).InstancePerDependency();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                // area endpoint
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{Id?}");

                // default endpoint
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Dashboard}/{action=Index}/{id?}");
                //endpoints.MapRazorPages();
            });
        }

        private (string connectionString, string migrationAssemblyName) GetConnectionStringAndAssemblyName()
        {
            var connectionStringName = "DefaultConnection";
            var connectionString = Configuration.GetConnectionString(connectionStringName);
            var migrationAssemblyName = typeof(Startup).Assembly.FullName;
            return (connectionString, migrationAssemblyName);
        }
    }
}