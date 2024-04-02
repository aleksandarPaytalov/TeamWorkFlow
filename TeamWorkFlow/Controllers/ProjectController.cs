using Microsoft.AspNetCore.Mvc;
using TeamWorkFlow.Core.Contracts;
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

		    //return RedirectToAction(nameof(All));
		    return RedirectToAction(nameof(Details), new { id = projectId });
	    }

	    public IActionResult Edit()
	    {
		    return View();
	    }

	    public IActionResult Details()
	    {
		    return View();
		}

	    public IActionResult Delete()
	    {
		    return View();
	    }
	}
}
