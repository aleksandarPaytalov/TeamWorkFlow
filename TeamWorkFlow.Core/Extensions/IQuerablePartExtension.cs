using TeamWorkFlow.Core.Models.Part;
using TeamWorkFlow.Infrastructure.Data.Models;

namespace System.Linq
{
    public static class IQuerablePartExtension
    {
        public static IQueryable<PartServiceModel> ProjectToPartServiceModels(this IQueryable<Part> parts)
        {
            return parts
                .Select(p => new PartServiceModel()
                {
                    Id = p.Id,
                    ImageUrl = p.ImageUrl,
                    Name = p.Name,
                    PartArticleNumber = p.PartArticleNumber,
                    PartClientNumber = p.PartClientNumber,
                    PartModel = p.PartModel,
                    ProjectNumber = p.Project.ProjectNumber,
                    ToolNumber = p.ToolNumber
                });
        }
    }
}
