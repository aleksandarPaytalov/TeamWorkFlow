using Microsoft.AspNetCore.Mvc;

namespace TeamWorkFlow.Areas.Admin.Controllers
{
	public class HomeController : AdminBaseController
	{
		public IActionResult Check()
		{
			return View();
		}

		public async Task<IActionResult> ForReview()
		{
			return View();
		}
	}
}
