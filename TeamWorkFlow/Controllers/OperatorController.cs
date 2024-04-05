using Microsoft.AspNetCore.Mvc;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Operator;
using static TeamWorkFlow.Core.Constants.Messages;

namespace TeamWorkFlow.Controllers
{
    public class OperatorController : BaseController
    {
	    private readonly IOperatorService _operatorService;

	    public OperatorController(IOperatorService operatorService)
	    {
		    _operatorService = operatorService;
	    }

		[HttpGet]
	    public async Task <IActionResult> All()
	    {
		    var model = await _operatorService.GetAllOperatorsAsync();

            return View(model);
        }

		[HttpGet]
	    public async Task<IActionResult> Add()
	    {
		    var operatorStatus = await _operatorService.GetAllOperatorStatusesAsync();

		    var operatorModel = new OperatorFormModel()
		    {
			    AvailabilityStatusModels = operatorStatus
		    };

		    return View(operatorModel);
	    }

        [HttpPost]
        public async Task<IActionResult> Add(OperatorFormModel model)
        {
	        if (!bool.TryParse(model.IsActive, out bool result))
	        {
				ModelState.AddModelError(nameof(model.IsActive),$"{BooleanInput}");
	        }
			
			if (!await _operatorService.OperatorStatusExistAsync(model.AvailabilityStatusId))
	        {
		        ModelState.AddModelError(nameof(model.AvailabilityStatusId), $"{StatusNotExisting}");
			}
			
	        if (!ModelState.IsValid)
	        {
		        model.AvailabilityStatusModels = await _operatorService.GetAllOperatorStatusesAsync();

				return View(model);
	        }
			
	        await _operatorService.AddNewOperatorAsync(model);

	        return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
	        if (ModelState.IsValid == false)
	        {
		        return BadRequest();
	        }
	        
			var model = await _operatorService.GetOperatorForEditAsync(id);
			if (model != null)
			{
				model.AvailabilityStatusModels = await _operatorService.GetAllOperatorStatusesAsync();
			}
			
			return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(OperatorFormModel model, int id)
        {
	        if (!bool.TryParse(model.IsActive, out bool result))
	        {
		        ModelState.AddModelError(nameof(model.IsActive), $"{BooleanInput}");
	        }

	        if (!await _operatorService.OperatorStatusExistAsync(model.AvailabilityStatusId))
	        {
		        ModelState.AddModelError(nameof(model.AvailabilityStatusId), $"{StatusNotExisting}");
	        }

	        if (!await _operatorService.OperatorExistByIdAsync(id))
	        {
		        ModelState.AddModelError(nameof(model.Id), $"{OperatorWithIdDoNotExist}");
			}

			if (!ModelState.IsValid)
            {
	            model.AvailabilityStatusModels = await _operatorService.GetAllOperatorStatusesAsync();

				return View(model);
            }

            await _operatorService.EditOperatorAsync(model, id);

            return RedirectToAction(nameof(All));
        }

		[HttpGet]
        public async Task<IActionResult> Details(int id)
        {
	        if (!ModelState.IsValid)
	        {
		        BadRequest();
	        }

	        var operatorModel = await _operatorService.GetOperatorDetailsByIdAsync(id);

	        return View(operatorModel);
        }

		[HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
	        if (!ModelState.IsValid)
	        {
		        BadRequest();
	        }

	        var operatorModel = await _operatorService.GetOperatorModelForDeleteByIdAsync(id);

			return View(operatorModel);
		}

		[HttpPost]
        public async Task<IActionResult> DeleteConfirmation(int id)
        {
			if (!ModelState.IsValid)
			{
				BadRequest();
			}

			await _operatorService.DeleteOperatorByIdAsync(id);

			return RedirectToAction(nameof(All));
        }

	}
}
