using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Admin.Operator;
using static TeamWorkFlow.Core.Constants.UsersConstants;

namespace TeamWorkFlow.Areas.Admin.Controllers
{
	public class OperatorController : AdminBaseController
	{
		private readonly IOperatorService _operatorService;
		private readonly IMemoryCache _memoryCache;

		public OperatorController(
			IOperatorService operatorService, 
			IMemoryCache memoryCache)
		{
			_operatorService = operatorService;
			_memoryCache = memoryCache;
		}

		[HttpGet]
		public async Task<IActionResult> All()
		{
			var operators = _memoryCache.Get<IEnumerable<OperatorAccessServiceModel>>(UserCacheKey);

			if (operators == null || operators.Any() == false)
			{
				//we read from dataBase
				operators = await _operatorService.GetAllOperatorsAsync();

				//setting the cache
				var cacheOptions = new MemoryCacheEntryOptions()
					.SetAbsoluteExpiration(TimeSpan.FromMinutes(1));

				_memoryCache.Set(UserCacheKey, operators, cacheOptions);
			}

			return View(operators);
		}

		[HttpGet]
		public async Task<IActionResult> Activate()
		{
			var model = await _operatorService.GetAllUnActiveOperatorsAsync();

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Activate(int id)
		{
			try
			{
				var operatorDetails = await _operatorService.GetOperatorDetailsByIdAsync(id);
				if (operatorDetails != null)
				{
					await _operatorService.ActivateOperatorAsync(id);
					TempData["SuccessMessage"] = $"Operator {operatorDetails.FullName} has been activated and status set to 'at work'.";
				}
				else
				{
					TempData["ErrorMessage"] = "Operator not found.";
				}
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = $"An error occurred while activating operator: {ex.Message}";
			}

			return RedirectToAction(nameof(Activate));
		}

		[HttpPost]
		public async Task<IActionResult> ToggleStatus(int id)
		{
			// Get the operator to check current status
			var operatorDetails = await _operatorService.GetOperatorDetailsByIdAsync(id);

			if (operatorDetails == null)
			{
				TempData["ErrorMessage"] = "Operator not found.";
				return RedirectToAction(nameof(All));
			}

			try
			{
				if (operatorDetails.IsActive)
				{
					// Deactivate operator (always allowed)
					await _operatorService.DeactivateOperatorAsync(id);
					TempData["SuccessMessage"] = $"Operator {operatorDetails.FullName} has been deactivated.";
				}
				else
				{
					// Activate operator (automatically sets status to "at work")
					await _operatorService.ActivateOperatorAsync(id);
					TempData["SuccessMessage"] = $"Operator {operatorDetails.FullName} has been activated and status set to 'at work'.";
				}

				// Clear cache to refresh data
				_memoryCache.Remove(UserCacheKey);
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = $"An error occurred while updating operator status: {ex.Message}";
			}

			return RedirectToAction(nameof(All));
		}
	}
}
