using System;
using System.Security.Claims;
using System.Security.Principal;

namespace DataImporter.Membership.Extensions
{
    public static class IdentityClaimsExtensions
    {
        public static string GetDisplayName(this IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }
            var ci = identity as ClaimsIdentity;
            if (ci != null)
            {
                return ci.FindFirst("DisplayName")?.Value;
            }
            return null;
        }
    }
}
