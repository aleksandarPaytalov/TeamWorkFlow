using Microsoft.AspNetCore.Mvc;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Project;

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
