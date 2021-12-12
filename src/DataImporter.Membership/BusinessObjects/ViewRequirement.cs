using Microsoft.AspNetCore.Authorization;

namespace DataImporter.Membership.BusinessObjects
{
    public class ViewRequirement : IAuthorizationRequirement
    {
        public ViewRequirement()
        {
        }
    }
}
