﻿using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using TeamWorkFlow.Models;

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
	        if (User.Identity != null && User.Identity.IsAuthenticated)
	        {
		        return RedirectToAction("All", "Task");
	        }

			return View();
		}
		
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}