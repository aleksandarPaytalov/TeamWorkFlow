﻿using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging.Signing;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Extensions;
using TeamWorkFlow.Core.Models.Project;
using static TeamWorkFlow.Core.Constants.Messages;

namespace TeamWorkFlow.Controllers
{
    public class ProjectController : BaseController
    {
	    private readonly IProjectService _projectService;

	    public ProjectController(IProjectService projectService)
	    {
		    _projectService = projectService;
	    }

	    [HttpGet]
        public async Task<IActionResult> All()
        {
	        var projectsToDisplay = await _projectService.GetAllProjectsAsync();
			
            return View(projectsToDisplay);
        }

		[HttpGet]
	    public async Task<IActionResult> Add()
	    {
		    var model = new ProjectFormModel()
		    {
			    ProjectStatuses = await _projectService.GetAllProjectStatusesAsync()
		    };

		    return View(model);
	    }
		[HttpPost]
	    public async Task<IActionResult> Add(ProjectFormModel model)
	    {
		    if (await _projectService.ExistByProjectNumberAsync(model.ProjectNumber))
		    {
				ModelState.AddModelError(nameof(model.ProjectNumber), $"{ProjectWithThisNumberAlreadyCreated}");
		    }

		    if (!await _projectService.ProjectStatusExistAsync(model.ProjectStatusId))
		    {
				ModelState.AddModelError(nameof(model.ProjectStatusId), $"{StatusNotExisting}");
		    }

		    if (model.TotalHoursSpent < 0)
		    {
				ModelState.AddModelError(nameof(model.TotalHoursSpent), $"{StringNumberRange}");
		    }

		    if (!ModelState.IsValid)
		    {
				model.ProjectStatuses = await _projectService.GetAllProjectStatusesAsync();

				return View(model);
		    }

		    var projectId = await _projectService.AddNewProjectsAsync(model);

		    return RedirectToAction(nameof(Details), new { id = projectId });
	    }

		[HttpGet]
	    public async Task<IActionResult> Edit(int id, string extension)
	    {
		    if (!await _projectService.ProjectExistByIdAsync(id))
		    {
			    return BadRequest();
		    }

			if (!ModelState.IsValid)
		    {
			    return BadRequest();
		    }

			var projectModel = await _projectService.GetProjectForEditByIdAsync(id);

			if (extension != projectModel?.GetProjectExtension())
			{
				return BadRequest();
			}

			return View(projectModel);
	    }
	    
		[HttpPost]
	    public async Task<IActionResult> Edit(ProjectFormModel model, int id)
	    {
		    if (!await _projectService.ProjectExistByIdAsync(id))
		    {
			    return BadRequest();
		    }
		    
			//checking if there is another projects with same ProjectNumber and different IDs.
			var collectionOfProjectsId = await _projectService.GetAllProjectIdsByProjectNumberAsync(model.ProjectNumber);

			foreach (var pId in collectionOfProjectsId)
			{
				if (pId != id)
				{
					ModelState.AddModelError(nameof(model.ProjectNumber), $"{ProjectWithThisNumberAlreadyCreated}");
				}
			}
			
		    if (!await _projectService.ProjectStatusExistAsync(model.ProjectStatusId))
		    {
			    ModelState.AddModelError(nameof(model.ProjectStatusId), $"{StatusNotExisting}");
		    }

		    if (model.TotalHoursSpent < 0)
		    {
			    ModelState.AddModelError(nameof(model.TotalHoursSpent), $"{StringNumberRange}");
		    }

		    if (!ModelState.IsValid)
		    {
			    model.ProjectStatuses = await _projectService.GetAllProjectStatusesAsync();

			    return View(model);
		    }

		    await _projectService.EditProjectAsync(model, id);

		    return RedirectToAction(nameof(Details), new {id, extension = model.GetProjectExtension()});
	    }

		[HttpGet]
	    public async Task<IActionResult> Details(int id, string extension)
	    {
		    if (!await _projectService.ProjectExistByIdAsync(id))
		    {
			    return BadRequest();
		    }

		    if (!ModelState.IsValid)
		    {
			    return BadRequest();
			}

		    var projectToShow = await _projectService.GetProjectDetailsByIdAsync(id);

		    if (extension != projectToShow?.GetProjectExtension())
		    {
			    return BadRequest();
		    }

		    return View(projectToShow);
		}

		[HttpGet]
	    public async Task<IActionResult> Delete(int id, string extension)
	    {
		    if (!await _projectService.ProjectExistByIdAsync(id))
		    {
			    return BadRequest();
		    }

		    if (!ModelState.IsValid)
		    {
			    return BadRequest();
		    }

			var model = await _projectService.GetProjectForDeleteByIdAsync(id);

			if (extension != model?.GetProjectExtension())
			{
				return BadRequest();
			}

			return View(model);
	    }

		[HttpPost]
	    public async Task<IActionResult> Confirmation(int id)
	    {
		    if (!await _projectService.ProjectExistByIdAsync(id))
		    {
			    return BadRequest();
		    }

		    await _projectService.ProjectDeleteAsync(id);

			return RedirectToAction(nameof(All));
	    }
	}
}
