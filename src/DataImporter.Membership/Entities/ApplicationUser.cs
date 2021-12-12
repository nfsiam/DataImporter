using Microsoft.AspNetCore.Identity;
using System;

namespace DataImporter.Membership.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [PersonalData]
        public string Name { get; set; }
    }
}
