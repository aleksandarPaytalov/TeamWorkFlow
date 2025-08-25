using System.Security.Claims;
using static TeamWorkFlow.Core.Constants.UsersConstants;

namespace TeamWorkFlow.Extensions
{
	public static class ClaimPrincipalExtensions
	{
		public static string Id(this ClaimsPrincipal user)
		{
			return user.FindFirstValue(ClaimTypes.NameIdentifier);
		}

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.IsInRole(AdminRole);
        }

        public static bool IsOperator(this ClaimsPrincipal user)
        {
            return user.IsInRole(OperatorRole);
        }

        public static bool IsGuest(this ClaimsPrincipal user)
        {
            return user.IsInRole(GuestRole);
        }

    }
}
