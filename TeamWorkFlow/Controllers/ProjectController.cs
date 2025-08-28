using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging.Signing;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Extensions;
using TeamWorkFlow.Core.Models.Project;
using TeamWorkFlow.Core.Models.BulkOperations;
using TeamWorkFlow.Extensions;
using static TeamWorkFlow.Core.Constants.Messages;

namespace TeamWorkFlow.Controllers
{
    public class ProjectController : BaseController
    {
	    private readonly IProjectService _projectService;
	    private readonly IPartService _partService;

	    public ProjectController(IProjectService projectService, IPartService partService)
	    {
		    _projectService = projectService;
		    _partService = partService;
	    }

	    [HttpGet]
        public async Task<IActionResult> All([FromQuery] AllProjectsQueryModel model)
        {
            if (!User.Identity?.IsAuthenticated == true || (User.IsAdmin() == false && User.IsOperator() == false && User.IsGuest() == false))
            {
                return Challenge();
            }

	        var projects = await _projectService.AllAsync(
		        model.Sorting,
		        model.Search,
		        model.ProjectsPerPage,
		        model.CurrentPage
	        );

	        model.TotalProjectsCount = projects.TotalProjectsCount;
	        model.Projects = projects.Projects;

            return View(model);
        }

		[HttpGet]
	    public async Task<IActionResult> Add()
	    {
            if (User.IsAdmin() == false)
            {
                return Unauthorized();
            }

		    var model = new ProjectFormModel()
		    {
			    ProjectStatuses = await _projectService.GetAllProjectStatusesAsync()
		    };

		    return View(model);
	    }
		[HttpPost]
		[ValidateAntiForgeryToken]
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

		    // Set default 1 hour for project setup/initialization time
		    model.TotalHoursSpent = 1;

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
            if (User.IsAdmin() == false)
            {
                return Unauthorized();
            }

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
		[ValidateAntiForgeryToken]
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
		    if (User.IsAdmin() == false && User.IsOperator() == false)
		    {
			    return Unauthorized();
		    }

			if (!await _projectService.ProjectExistByIdAsync(id))
		    {
			    return NotFound();
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

		    // Get parts for this project
		    if (projectToShow != null)
		    {
			    projectToShow.Parts = await _partService.GetPartsByProjectIdAsync(id);
		    }

		    return View(projectToShow);
		}

		[HttpGet]
	    public async Task<IActionResult> Delete(int id, string extension)
	    {
            if (User.IsAdmin() == false)
            {
                return Unauthorized();
            }

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
		[ValidateAntiForgeryToken]
	    public async Task<IActionResult> Confirmation(int id)
	    {
		    if (!await _projectService.ProjectExistByIdAsync(id))
		    {
			    return BadRequest();
		    }

		    await _projectService.ProjectDeleteAsync(id);

			return RedirectToAction(nameof(All));
	    }

	    [HttpPost]
	    [ValidateAntiForgeryToken]
	    public async Task<IActionResult> CalculateCost(int projectId, decimal hourlyRate, string currency = "USD")
	    {
		    if (!await _projectService.ProjectExistByIdAsync(projectId))
		    {
			    return Json(new { success = false, message = "Project not found." });
		    }

		    if (hourlyRate <= 0)
		    {
			    return Json(new { success = false, message = "Hourly rate must be greater than 0." });
		    }

		    // Validate currency
		    if (currency != "USD" && currency != "EUR")
		    {
			    return Json(new { success = false, message = "Invalid currency selected." });
		    }

		    try
		    {
			    var costCalculation = await _projectService.CalculateProjectCostAsync(projectId, hourlyRate);

			    // Format currency based on selection
			    var currencySymbol = currency == "EUR" ? "€" : "$";
			    var formattedTotalCost = $"{currencySymbol}{costCalculation.TotalLaborCost:F2}";
			    var formattedHourlyRate = $"{currencySymbol}{hourlyRate:F2}";

			    return Json(new
			    {
				    success = true,
				    totalLaborCost = formattedTotalCost,
				    calculatedTotalHours = costCalculation.FormattedCalculatedTotalHours,
				    hourlyRate = formattedHourlyRate,
				    currency = currency
			    });
		    }
		    catch (Exception ex)
		    {
			    return Json(new { success = false, message = ex.Message });
		    }
	    }

	    // Bulk operations - Admin only
	    [HttpPost]
	    [ValidateAntiForgeryToken]
	    public async Task<IActionResult> BulkDelete([FromBody] BulkDeleteRequest request)
	    {
		    if (!User.IsAdmin())
		    {
			    return Json(new { success = false, message = "Unauthorized. Admin access required." });
		    }

		    if (!ModelState.IsValid || request?.ItemIds == null || !request.ItemIds.Any())
		    {
			    return Json(new { success = false, message = "No projects selected for deletion" });
		    }

		    try
		    {
			    var result = await _projectService.BulkDeleteAsync(request.ItemIds);
			    return Json(new {
				    success = result.Success,
				    message = result.Message,
				    processedItems = result.ProcessedItems,
				    failedItems = result.FailedItems,
				    errors = result.ErrorMessages
			    });
		    }
		    catch (Exception ex)
		    {
			    return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
		    }
	    }
	}
}
