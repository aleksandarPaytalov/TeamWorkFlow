using Microsoft.AspNetCore.Mvc;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Operator;

namespace TeamWorkFlow.Controllers
{
    public class OperatorController : BaseController
    {
	    private readonly IOperatorService _service;

	    public OperatorController(IOperatorService service)
	    {
		    _service = service;
	    }

		[HttpGet]
	    public async Task <IActionResult> All()
	    {
		    var model = await _service.GetAllOperatorsAsync();

            return View(model);
        }

		[HttpGet]
	    public async Task<IActionResult> Add()
	    {
		    var operatorStatus = await _service.GetAllStatusesAsync();

		    var operatorModel = new OperatorServicesModel()
		    {
			    AvailabilityStatusModels = operatorStatus
		    };

		    return View(operatorModel);
	    }

        [HttpPost]
        public async Task<IActionResult> Add(OperatorServicesModel model)
        {
	        if (!ModelState.IsValid)
	        {
		        return View(model);
	        }

	        try
	        {
		        await _service.AddNewOperatorAsync(model);

		        return RedirectToAction(nameof(All));
			}
	        catch (Exception)
	        {
				ModelState.AddModelError("", "Something went wrong. Please try again");

		        return View(model);
	        }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
	        var model = await _service.GetOperatorForEditAsync(id);

	        return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(OperatorServicesModel model, int id)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _service.EditOperatorAsync(model, id);

            return RedirectToAction(nameof(All));
        }

    }
}
