using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TeamWorkFlow.Core.Constants;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Machine;
using TeamWorkFlow.Infrastructure.Data.Models;

namespace TeamWorkFlow.Controllers
{
	public class MachineController : BaseController
    {

	    private readonly IMachineService _service;

	    public MachineController(IMachineService service)
	    {
		    _service = service;
	    }

		[HttpGet]
	    public async Task<IActionResult> All()
	    {
		    var model = await _service.GetAllMachinesAsync();

            return View(model);
        }

	    [HttpGet]
	    public IActionResult Add()
	    {
		    var machineModel = new MachineServiceModel();

		    return View(machineModel);
	    }

		[HttpPost]
	    public async Task<IActionResult> Add(MachineServiceModel model)
	    {
		    if (!ModelState.IsValid)
		    {
			    return BadRequest();
		    }
			
			await _service.AddNewMachineAsync(model);

			return RedirectToAction(nameof(All));
	    }

		[HttpGet]
	    public async Task<IActionResult?> Edit(int id)
	    {
		    if (!ModelState.IsValid)
		    {
			    return BadRequest();
		    }

		    var model = await _service.GetMachineForEditAsync(id);

		    return View(model);
	    }

	    public async Task<IActionResult> Edit(MachineServiceModel model, int id)
	    {
		    if (!ModelState.IsValid)
		    {
			    return BadRequest();
		    }

		    await _service.EditMachineAsync(model, id);

		    return RedirectToAction(nameof(All));
	    }
    }
}
