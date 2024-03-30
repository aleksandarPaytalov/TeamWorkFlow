using Microsoft.AspNetCore.Mvc;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Part;

namespace TeamWorkFlow.Controllers
{
    public class PartController : BaseController
    {
        private readonly IPartService _partService;

        public PartController(IPartService partService)
        {
            _partService = partService;
        }


        [HttpGet]
        public async Task<IActionResult> All([FromQuery] AllPartsQueryModel model)
        {
            var parts = await _partService.AllAsync(
                model.PartsPerPage,
                model.CurrentPage,
                model.Sorting,
                model.Search,
                model.Status
            );

            model.TotalPartsCount = parts.TotalPartsCount;
            model.Parts = parts.Parts;
            model.Statuses = await _partService.AllStatusNamesAsync();
            
            return View(model);
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
