using TeamWorkFlow.Core.Contracts;

namespace TeamWorkFlow.Core.Models.Part
{
    public class PartDeleteServiceModel : IPartModel
	{
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string PartArticleNumber { get; set; } = string.Empty;

        public string ProjectNumber { get; set; } = string.Empty;
    }
}
