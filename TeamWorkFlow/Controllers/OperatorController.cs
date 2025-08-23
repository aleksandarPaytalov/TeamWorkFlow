using Microsoft.AspNetCore.Mvc;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Extensions;
using TeamWorkFlow.Core.Models.Operator;
using TeamWorkFlow.Extensions;
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
	    public async Task <IActionResult> All([FromQuery] AllOperatorsQueryModel model)
	    {
            if (!User.Identity?.IsAuthenticated == true || (User.IsAdmin() == false && User.IsOperator() == false))
            {
                return Challenge();
            }

		    var operators = await _operatorService.AllAsync(
			    model.Sorting,
			    model.Search,
			    model.OperatorsPerPage,
			    model.CurrentPage
		    );

		    model.TotalOperatorsCount = operators.TotalOperatorsCount;
		    model.Operators = operators.Operators;

            return View(model);
        }

		[HttpGet]
	    public async Task<IActionResult> Add()
	    {
		    if (User.IsAdmin() == false)
		    {
			    return Unauthorized();
		    }

			var operatorStatus = await _operatorService.GetAllOperatorStatusesAsync();

		    var operatorModel = new OperatorFormModel()
		    {
			    AvailabilityStatusModels = operatorStatus
		    };


		    return View(operatorModel);
	    }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(OperatorFormModel model)
        {
            var userId = await _operatorService.GetUserIdByEmailAsync(model.Email.Normalize());
            if (userId != null)
            {
                model.UserId = userId;
            }
            else
            {
				ModelState.AddModelError(nameof(model.UserId), $"{UserWithEmailNotRegistered}");
            }

	        if (!bool.TryParse(model.IsActive, out bool result))
	        {
				ModelState.AddModelError(nameof(model.IsActive),$"{BooleanInput}");
	        }

			if (!await _operatorService.OperatorStatusExistAsync(model.AvailabilityStatusId))
	        {
		        ModelState.AddModelError(nameof(model.AvailabilityStatusId), $"{StatusNotExisting}");
			}

			// Validate business rule: Only "at work" status allows active operators
			// Note: Admin activation automatically sets status to "at work", but manual form entry still requires validation
			if (result && model.AvailabilityStatusId != 1) // 1 = "at work" status
			{
				ModelState.AddModelError(nameof(model.IsActive), "Operators can only be active when availability status is 'at work'. Use the admin toggle to automatically set the correct status.");
			}
			
	        if (!ModelState.IsValid)
	        {
		        model.AvailabilityStatusModels = await _operatorService.GetAllOperatorStatusesAsync();

				return View(model);
	        }

	        await _operatorService.AddNewOperatorAsync(model, userId!);

	        return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, string extension)
        {
	        if (User.IsAdmin() == false)
	        {
		        return Unauthorized();
	        }

			if (await _operatorService.OperatorExistByIdAsync(id) == false)
            {
                return BadRequest();
            }

            if (ModelState.IsValid == false)
	        {
		        return BadRequest();
	        }
	        
			var model = await _operatorService.GetOperatorForEditAsync(id);
			if (model != null)
			{
				model.AvailabilityStatusModels = await _operatorService.GetAllOperatorStatusesAsync();
			}

            if (extension != model?.GetOperatorExtension())
            {
                return BadRequest();
            }
			
			return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

			// Validate business rule: Only "at work" status allows active operators
			// Note: Admin activation automatically sets status to "at work", but manual form entry still requires validation
			if (result && model.AvailabilityStatusId != 1) // 1 = "at work" status
			{
				ModelState.AddModelError(nameof(model.IsActive), "Operators can only be active when availability status is 'at work'. Use the admin toggle to automatically set the correct status.");
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

            return RedirectToAction(nameof(All)); //, new {id = model.Id, extension = model.GetOperatorExtension()});
        }

		[HttpGet]
        public async Task<IActionResult> Details(int id, string extension)
        {
	        if (User.IsAdmin() == false && User.IsOperator() == false)
	        {
		        return Unauthorized();
	        }

			if (await _operatorService.OperatorExistByIdAsync(id) == false)
            {
                return NotFound();
            }

	        if (!ModelState.IsValid)
	        {
		        return BadRequest();
	        }

	        var operatorModel = await _operatorService.GetOperatorDetailsByIdAsync(id);

            if (extension != operatorModel?.GetOperatorExtension())
            {
                return BadRequest();
            }

	        return View(operatorModel);
        }

		[HttpGet]
        public async Task<IActionResult> Delete(int id, string extension)
        {
	        if (User.IsAdmin() == false)
	        {
		        return Unauthorized();
	        }

			if (await _operatorService.OperatorExistByIdAsync(id) == false)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
	        {
		        return BadRequest();
	        }

	        var operatorModel = await _operatorService.GetOperatorModelForDeleteByIdAsync(id);

            if (extension != operatorModel?.GetOperatorExtension())
            {
                return BadRequest();
            }

            return View(operatorModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmation(int id)
        {
            if (await _operatorService.OperatorExistByIdAsync(id) == false)
            {
                return BadRequest();
            }

			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			await _operatorService.DeleteOperatorByIdAsync(id);

			return RedirectToAction(nameof(All));
        }

	}
}
