using Microsoft.AspNetCore.Mvc;
using TeamWorkFlow.Core.Contracts;

namespace TeamWorkFlow.Controllers
{
    [Route("api/summary")]
	[ApiController]
	public class SummaryApiController : ControllerBase
	{
		private readonly ISummaryService _summaryService;

		public SummaryApiController(ISummaryService summaryService)
		{
			_summaryService = summaryService;
		}

		[HttpGet]
		public async Task<IActionResult> Summary()
		{
			var result = await _summaryService.SummaryAsync();

			return Ok(result);
		}
	}
}
