using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DataImporter.Membership.BusinessObjects
{
    public class ViewRequirementHandler :
          AuthorizationHandler<ViewRequirement>
    {
        protected override Task HandleRequirementAsync(
               AuthorizationHandlerContext context,
               ViewRequirement requirement)
        {
            var claim = context.User.FindFirst("view_permission");
            if (claim != null && bool.Parse(claim.Value))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
