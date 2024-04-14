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

		public async Task<IActionResult> All()
		{
			var operators = await _operatorService.GetAllOperatorsAsync();

			return View(operators);
		}
	}
}
