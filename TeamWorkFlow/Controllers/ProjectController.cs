using Microsoft.AspNetCore.Mvc;
using TeamWorkFlow.Core.Contracts;

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


	    public IActionResult Add()
	    {
		    return View();
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
