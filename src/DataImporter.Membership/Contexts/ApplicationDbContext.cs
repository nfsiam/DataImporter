using System;
using DataImporter.Membership.Entities;
using DataImporter.Membership.Seeds;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataImporter.Membership.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,Role,Guid,UserClaim,UserRole,UserLogin,RoleClaim,UserToken>, IApplicationDbContext
    {
        private readonly string _connectionString;
        private readonly string _migrationAssemblyName;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext(string connectionString, string migrationAssemblyName)
        {
            _connectionString = connectionString;
            _migrationAssemblyName = migrationAssemblyName;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            if (!dbContextOptionsBuilder.IsConfigured)
            {
                dbContextOptionsBuilder.UseSqlServer(
                    _connectionString,
                    m => m.MigrationsAssembly(_migrationAssemblyName));
            }

            base.OnConfiguring(dbContextOptionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>()
                .HasData(DataSeed.ApplicationUsers);
            builder.Entity<Role>()
                .HasData(DataSeed.Roles);
            builder.Entity<UserClaim>()
                .HasData(DataSeed.UserClaims);
            base.OnModelCreating(builder);
        }
    }
}
