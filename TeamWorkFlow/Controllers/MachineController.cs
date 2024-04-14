using Microsoft.AspNetCore.Mvc;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Extensions;
using TeamWorkFlow.Core.Models.Machine;
using TeamWorkFlow.Extensions;

namespace TeamWorkFlow.Controllers
{
	public class MachineController : BaseController
    {

	    private readonly IMachineService _machineService;

	    public MachineController(IMachineService machineService)
	    {
		    _machineService = machineService;
	    }

		[HttpGet]
	    public async Task<IActionResult> All()
	    {
		    var model = await _machineService.GetAllMachinesAsync();

            return View(model);
        }

	    [HttpGet]
	    public IActionResult Add()
	    {
		    var machineModel = new MachineFormModel();

		    return View(machineModel);
	    }

		[HttpPost]
	    public async Task<IActionResult> Add(MachineFormModel model)
	    {
		    if (User.IsAdmin() == false)
		    {
			    return Unauthorized();
		    }

		    if (!ModelState.IsValid)
		    {
			    return BadRequest();
		    }
			
			await _machineService.AddNewMachineAsync(model);

			return RedirectToAction(nameof(All));
	    }

		[HttpGet]
	    public async Task<IActionResult> Edit(int id, string extension)
	    {
		    if (User.IsAdmin() == false)
		    {
			    return Unauthorized();
		    }

			if (!await _machineService.MachineExistByIdAsync(id))
		    {
			    return BadRequest();
		    }

			if (!ModelState.IsValid)
		    {
			    return BadRequest();
		    }

		    var model = await _machineService.GetMachineForEditAsync(id);

		    if (extension != model?.GetMachineExtension())
		    {
			    return BadRequest();
		    }

			return View(model);
	    }

		[HttpPost]
	    public async Task<IActionResult> Edit(MachineFormModel model, int id)
	    {
		    if (!ModelState.IsValid)
		    {
			    return BadRequest();
		    }

		    await _machineService.EditMachineAsync(model, id);

		    return RedirectToAction(nameof(Details), new {id, extension = model.GetMachineExtension()});
	    }

	    public async Task<IActionResult> Details(int id, string extension)
	    {
		    if (User.IsAdmin() == false && User.IsOperator() == false)
		    {
			    return Unauthorized();
		    }

			if (!await _machineService.MachineExistByIdAsync(id))
		    {
			    return BadRequest();
		    }

		    if (!ModelState.IsValid)
		    {
			    return BadRequest();
			}

			var model = await _machineService.MachineDetailsAsync(id);

			if (extension != model?.GetMachineExtension())
			{
				return BadRequest();
			}

		    return View(model);
	    }

		[HttpGet]
	    public async Task<IActionResult> Delete(int id, string extension)
	    {
		    if (User.IsAdmin() == false)
		    {
			    return Unauthorized();
		    }

			if (!await _machineService.MachineExistByIdAsync(id))
		    {
			    return BadRequest();
		    }

		    if (!ModelState.IsValid)
		    {
			    return BadRequest();
		    }

			var model = await _machineService.GetMachineForDeleteByIdAsync(id);

			if (extension != model?.GetMachineExtension())
			{
				return BadRequest();
			}

			return View(model);
	    }

		[HttpPost]
	    public async Task<IActionResult> DeleteConfirmation(int id)
	    {
		    if (!await _machineService.MachineExistByIdAsync(id))
		    {
			    return BadRequest();
		    }

		    await _machineService.DeleteMachineAsync(id);

			return RedirectToAction(nameof(All));
		}
    }
}
