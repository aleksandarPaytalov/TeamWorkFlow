using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static TeamWorkFlow.Core.Constants.UsersConstants;

namespace TeamWorkFlow.Areas.Admin.Controllers
{
	[Area(AdminAreaName)]
	[Authorize(Roles = AdminRole)]
	public class AdminBaseController : Controller
	{
	}
}
