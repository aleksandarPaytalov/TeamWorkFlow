using Microsoft.AspNetCore.Mvc;
using TeamWorkFlow.Core.Contracts;

namespace TeamWorkFlow.Areas.Admin.Controllers
{
	public class OperatorController : AdminBaseController
	{
		private readonly IOperatorService _operatorService;

		public OperatorController(IOperatorService operatorService)
		{
			_operatorService = operatorService;
		}

		[HttpGet]
		public async Task<IActionResult> All()
		{
			var operators = await _operatorService.GetAllOperatorsAsync();

			return View(operators);
		}

		[HttpGet]
		public async Task<IActionResult> Activate()
		{
			var model = await _operatorService.GetAllUnActiveOperatorsAsync();

			return View(model);
		}

		public async Task<IActionResult> Activate(int id)
		{
			await _operatorService.ActivateOperatorAsync(id);

			return RedirectToAction(nameof(Activate));
		}
	}
}
