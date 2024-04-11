using System.Security.Claims;

namespace TeamWorkFlow.Extensions
{
	public static class UserClaimPrincipalExtensions
	{
		public static string Id(this ClaimsPrincipal user)
		{
			return user.FindFirstValue(ClaimTypes.NameIdentifier);
		}
	}
}
