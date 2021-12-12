using System;
using Microsoft.AspNetCore.Identity;

namespace DataImporter.Membership.BusinessObjects
{
    public class ApplicationUser
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
