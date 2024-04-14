using Microsoft.AspNetCore.Mvc;

namespace TeamWorkFlow.Areas.Admin.Controllers
{
	public class UserController : AdminBaseController
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
