using Microsoft.AspNetCore.Mvc;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Machine;

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
		    if (!ModelState.IsValid)
		    {
			    return BadRequest();
		    }
			
			await _machineService.AddNewMachineAsync(model);

			return RedirectToAction(nameof(All));
	    }

		[HttpGet]
	    public async Task<IActionResult?> Edit(int id)
	    {
		    if (!ModelState.IsValid)
		    {
			    return BadRequest();
		    }

		    var model = await _machineService.GetMachineForEditAsync(id);

		    return View(model);
	    }

	    public async Task<IActionResult> Edit(MachineFormModel model, int id)
	    {
		    if (!ModelState.IsValid)
		    {
			    return BadRequest();
		    }

		    await _machineService.EditMachineAsync(model, id);

		    return RedirectToAction(nameof(All));
	    }

	    public async Task<IActionResult> Details(int id)
	    {
		    if (!await _machineService.MachineExistByIdAsync(id))
		    {
			    return BadRequest();
		    }

			var model = await _machineService.MachineDetailsAsync(id);

		    return View(model);
	    }

	    public async Task<IActionResult> Delete(int id)
	    {
		    if (!await _machineService.MachineExistByIdAsync(id))
		    {
			    return BadRequest();
		    }

		    var model = await _machineService.GetMachineForDeleteByIdAsync(id);

			return View(model);
	    }

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
