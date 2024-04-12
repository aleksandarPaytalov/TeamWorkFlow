using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using static TeamWorkFlow.Infrastructure.Constants.DataConstants;

namespace TeamWorkFlow.Infrastructure.Data.Models
{
	public class AppUser : IdentityUser
	{
		[Required]
		[MaxLength(AppUserFirstNameMaxLength)]
		public string FirstName { get; set; } = string.Empty;

		[Required]
		[MaxLength(AppUserLastNameMaxLength)]
		public string LastName { get; set; } = string.Empty;
	}
}
