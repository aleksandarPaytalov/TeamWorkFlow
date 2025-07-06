namespace TeamWorkFlow.Core.Models.Project
{
    public class ProjectQueryServiceModel
    {
        public int TotalProjectsCount { get; set; }

        public IEnumerable<ProjectServiceModel> Projects { get; set; } = new List<ProjectServiceModel>();
    }
}
