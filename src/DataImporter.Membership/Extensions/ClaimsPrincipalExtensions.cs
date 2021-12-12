using System;
using System.Security.Claims;
using System.Security.Principal;

namespace DataImporter.Membership.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static T GetUserId<T>(this IPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException("ClaimsPrincipal");
            }
            var cp = principal as ClaimsPrincipal;
            if (cp != null)
            {
                var userId = cp.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                if (typeof(T) == typeof(Guid))
                {
                    return (T)Convert.ChangeType(new Guid(userId), typeof(T));
                }
                else
                {
                    return (T)Convert.ChangeType(userId, typeof(T));
                }
            }
            return default;
        }
    }
}
