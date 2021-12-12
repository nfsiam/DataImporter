using System;
using System.Collections.Generic;
using DataImporter.Membership.Entities;

namespace DataImporter.Membership.Seeds
{
    public static class DataSeed
    {
        public static List<ApplicationUser> ApplicationUsers { get; } = new()
        {
            new ApplicationUser
            {
                Id = new Guid("00000000-0000-0000-0000-0000000000a1"),
                Email = "user@mail.com",
                NormalizedEmail = "USER@MAIL.COM",
                UserName = "user@mail.com",
                NormalizedUserName = "USER@MAIL.COM",
                Name = "Default User",
                PasswordHash = "AQAAAAEAACcQAAAAEH1qiSF2ml3RgxYRhamcFViQUPvjHRONCXWoGCpDJF3/gn8DFdOwCdV/GyCTuak6Uw==",
                EmailConfirmed = true,
                SecurityStamp = "PZEJCLS4YDA5IBWGJ5SA2KSOKPDSV5NS",
                ConcurrencyStamp = "3f243aa9-4231-4819-8f11-c52bc02dbf56",
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0
            }
        };
        public static List<Role> Roles { get; } = new()
        {
            new Role
            {
                Id = new Guid("00000000-0000-0001-0000-000000000002"),
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new Role
            {
                Id = new Guid("00000000-0000-0001-0000-000000000001"),
                Name = "User",
                NormalizedName = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            }
        };
        public static List<UserClaim> UserClaims { get; } = new()
        {
            new UserClaim
            {
                Id = 1,
                UserId = new Guid("00000000-0000-0000-0000-0000000000a1"),
                ClaimType = "view_permission",
                ClaimValue = "true"
            }
        };
        public static List<UserRole> UserRoles { get; } = new()
        {
            new UserRole
            {
                UserId = new Guid("00000000-0000-0000-0000-0000000000a1"),
                RoleId = new Guid("00000000-0000-0001-0000-000000000001")
            },
            new UserRole
            {
                UserId = new Guid("00000000-0000-0000-0000-0000000000a1"),
                RoleId = new Guid("00000000-0000-0001-0000-000000000002")
            },
        };
    }
}
