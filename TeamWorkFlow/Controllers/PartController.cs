using Microsoft.AspNetCore.Mvc;
using TeamWorkFlow.Core.Constants;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Part;

namespace TeamWorkFlow.Controllers
{
	public class PartController : BaseController
    {
        private readonly IPartService _partService;
        private readonly IProjectService _projectService;

        public PartController(IPartService partService, 
            IProjectService projectService)
        {
            _partService = partService;
            _projectService = projectService;
        }


        [HttpGet]
        public async Task<IActionResult> All([FromQuery] AllPartsQueryModel model)
        {
            var parts = await _partService.AllAsync(
                model.Sorting,
                model.Search,
                model.Status,
                model.PartsPerPage,
                model.CurrentPage
            );

            model.TotalPartsCount = parts.TotalPartsCount;
            model.Parts = parts.Parts;
            model.Statuses = await _partService.AllStatusNamesAsync();
            
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var partModel = new PartFormModel()
            {
                Statuses = await _partService.AllStatusesAsync()
            };

            return View(partModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(PartFormModel model)
        {
            if (await _partService.StatusExistAsync(model.PartStatusId) == false)
            {
                ModelState.AddModelError(nameof(model.PartStatusId), $"{Messages.PartStatusNotExisting}");
            }

            if (await _projectService.ExistByProjectNumberAsync(model.ProjectNumber) == false)
            {
                ModelState.AddModelError(nameof(model.ProjectNumber), $"{Messages.ProjectWithGivenNumberDoNotExist}");
            }

            if (ModelState.IsValid == false)
            {
                model.Statuses = await _partService.AllStatusesAsync();

                return View(model);
            }


            int? projectId = await _projectService.GetProjectIdByProjectNumberAsync(model.ProjectNumber);
            int validProjectId = projectId ?? -1;

            if (validProjectId == -1)
            {
	            ModelState.AddModelError(nameof(model.ProjectNumber), $"{Messages.ProjectWithGivenNumberDoNotExist}");
            }

            int newPartId = await _partService.AddNewPartAsync(model, validProjectId);

			return RedirectToAction(nameof(All), new {id = newPartId});
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (!await _partService.PartExistAsync(id))
            {
                return BadRequest();
            }

            var partModel = await _partService.PartDetailsByIdAsync(id);

            return View(partModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!await _partService.PartExistAsync(id))
            {
                return BadRequest();
            }

            var model = await _partService.GetPartFormModelForEditAsync(id);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, PartFormModel model)
        {
	        if (!await _partService.PartExistAsync(id))
	        {
		        return BadRequest();
	        }

	        if (!await _partService.StatusExistAsync(model.PartStatusId))
	        {
                ModelState.AddModelError(nameof(model.PartStatusId), $"{Messages.PartStatusNotExisting}");
	        }

	        if (!await _projectService.ExistByProjectNumberAsync(model.ProjectNumber))
	        {
		        ModelState.AddModelError(nameof(model.ProjectNumber), $"{Messages.ProjectWithGivenNumberDoNotExist}");
			}

	        if (ModelState.IsValid == false)
	        {
		        model.Statuses = await _partService.AllStatusesAsync();

		        return View(model);
	        }

			int? projectId = await _projectService.GetProjectIdByProjectNumberAsync(model.ProjectNumber);
			int validProjectId = projectId ?? -1;

			if (validProjectId == -1)
			{
				ModelState.AddModelError(nameof(model.ProjectNumber), $"{Messages.ProjectWithGivenNumberDoNotExist}");
			}

			int statusId = model.PartStatusId;

	        await _partService.EditAsync(id, model, validProjectId, statusId);

	        return RedirectToAction(nameof(Details), new {id});
        }
		[HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
	        if (!await _partService.PartExistAsync(id))
	        {
		        return BadRequest();
	        }

	        var partModel = await _partService.GetPartForDeletingByIdAsync(id);
            return View(partModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmation(int id)
        {
	        if (!await _partService.PartExistAsync(id))
	        {
		        return BadRequest();
	        }

	        await _partService.DeletePartByIdAsync(id);

			return RedirectToAction(nameof(All));
        }
    }
}
