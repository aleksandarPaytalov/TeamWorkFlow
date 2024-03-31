using Microsoft.AspNetCore.Mvc;
using TeamWorkFlow.Core.Constants;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Part;
using TeamWorkFlow.Infrastructure.Data.Models;

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


            int? projectId = await _projectService.GetProjectIdAsync(model.ProjectNumber);
            int newPartId = await _partService.AddNewPartAsync(model, projectId ?? 0);

            return RedirectToAction(nameof(All), new {id = newPartId});
        }

        public IActionResult Details()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult Delete()
        {
            return View();
        }
    }
}
