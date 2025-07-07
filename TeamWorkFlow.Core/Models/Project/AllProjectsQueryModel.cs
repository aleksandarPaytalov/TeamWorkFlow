using System.ComponentModel.DataAnnotations;
using TeamWorkFlow.Core.Enumerations;

namespace TeamWorkFlow.Core.Models.Project
{
    public class AllProjectsQueryModel
    {
        public int ProjectsPerPage { get; } = 10;

        [Display(Name = "Search by project name or number")]
        public string Search { get; init; } = null!;

        public ProjectSorting Sorting { get; init; }
        
        public int CurrentPage { get; init; } = 1;

        public int TotalProjectsCount { get; set; }

        public IEnumerable<ProjectServiceModel> Projects { get; set; } = new List<ProjectServiceModel>();
    }
}
