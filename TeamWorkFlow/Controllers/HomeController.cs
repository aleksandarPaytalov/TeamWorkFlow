using Microsoft.AspNetCore.Authorization;
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

		[AllowAnonymous]
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

		[AllowAnonymous]
		[Route("Home/Error/{statusCode}")]
		public IActionResult Error(int? statusCode)
		{
			if (statusCode.HasValue)
			{
				switch (statusCode.Value)
				{
					case 400:
						ViewBag.ErrorCode = "400";
						ViewBag.ErrorTitle = "Bad Request";
						ViewBag.ErrorMessage = "The request could not be understood by the server due to malformed syntax.";
						ViewBag.ErrorDescription = "Please check your request and try again.";
						return View("Error400");

					case 401:
						ViewBag.ErrorCode = "401";
						ViewBag.ErrorTitle = "Unauthorized";
						ViewBag.ErrorMessage = "You are not authorized to access this resource.";
						ViewBag.ErrorDescription = "Please log in with appropriate credentials.";
						return View("Error401");

					case 403:
						ViewBag.ErrorCode = "403";
						ViewBag.ErrorTitle = "Forbidden";
						ViewBag.ErrorMessage = "You don't have permission to access this resource.";
						ViewBag.ErrorDescription = "Contact your administrator if you believe this is an error.";
						return View("Error403");

					case 404:
						ViewBag.ErrorCode = "404";
						ViewBag.ErrorTitle = "Page Not Found";
						ViewBag.ErrorMessage = "The page you are looking for could not be found.";
						ViewBag.ErrorDescription = "The page may have been moved, deleted, or you entered the wrong URL.";
						return View("Error404");

					case 408:
						ViewBag.ErrorCode = "408";
						ViewBag.ErrorTitle = "Request Timeout";
						ViewBag.ErrorMessage = "The server timed out waiting for the request.";
						ViewBag.ErrorDescription = "Please try again. If the problem persists, contact support.";
						return View("Error408");

					case 429:
						ViewBag.ErrorCode = "429";
						ViewBag.ErrorTitle = "Too Many Requests";
						ViewBag.ErrorMessage = "You have sent too many requests in a short period.";
						ViewBag.ErrorDescription = "Please wait a moment before trying again.";
						return View("Error429");

					case 500:
						ViewBag.ErrorCode = "500";
						ViewBag.ErrorTitle = "Internal Server Error";
						ViewBag.ErrorMessage = "An internal server error occurred.";
						ViewBag.ErrorDescription = "We are working to fix the problem. Please try again later.";
						return View("Error500");

					default:
						ViewBag.ErrorCode = statusCode.ToString();
						ViewBag.ErrorTitle = "Error";
						ViewBag.ErrorMessage = "An unexpected error occurred.";
						ViewBag.ErrorDescription = "Please try again or contact support if the problem persists.";
						return View("ErrorGeneric");
				}
			}

			// Default error view for cases without status code
			ViewBag.ErrorCode = "Unknown";
			ViewBag.ErrorTitle = "Error";
			ViewBag.ErrorMessage = "An error occurred while processing your request.";
			ViewBag.ErrorDescription = "Please try again or contact support if the problem persists.";
			return View();
		}
	}
}
