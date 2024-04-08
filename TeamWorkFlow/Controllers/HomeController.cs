using Microsoft.AspNetCore.Mvc;

namespace TeamWorkFlow.Controllers
{
	public class HomeController : BaseController
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

		public IActionResult Index()
        {
	        if (User.Identity is { IsAuthenticated: true })
	        {
		        return RedirectToAction("All", "Task");
	        }

			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View();
		}

		[Route("Home/Error/{statusCode}")]
		public IActionResult Error(int? statusCode)
		{
			if (statusCode.HasValue)
			{
				if (statusCode == 401)
				{
					return View("Error401");
				}
				else if (statusCode == 404)
				{
					ViewBag.ErrorMessage = "404 Page not found Exception";
					return View("Error404");
				}
				else if (statusCode == 500)
				{
					return View("Error500");
				}
			}

			return View(); 
		}
	}
}