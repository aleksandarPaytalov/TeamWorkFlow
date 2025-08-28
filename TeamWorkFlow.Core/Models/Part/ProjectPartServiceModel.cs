using TeamWorkFlow.Core.Contracts;

namespace TeamWorkFlow.Core.Models.Part
{
    public class ProjectPartServiceModel : IPartModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PartArticleNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
