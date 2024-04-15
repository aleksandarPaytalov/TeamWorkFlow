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

		public async Task<IActionResult> Activate(int id)
		{
			await _operatorService.ActivateOperatorAsync(id);

			return RedirectToAction(nameof(Activate));
		}
	}
}
