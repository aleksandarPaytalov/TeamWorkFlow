using System.ComponentModel.DataAnnotations;
using static TeamWorkFlow.Core.Constants.Messages;
using static TeamWorkFlow.Infrastructure.Constants.DataConstants;

namespace TeamWorkFlow.Core.Models.Part
{
    public class PartFormModel
    {
        [Required(ErrorMessage = RequiredMessage)]
        [StringLength(PartNameMaxLength,
            MinimumLength = PartNameMinLength,
            ErrorMessage = StringLength)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredMessage)]
        [StringLength(PartArticleNumberMaxLength,
            MinimumLength = PartArticleNumberMinLength,
            ErrorMessage = StringLength)]
        public string PartArticleNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredMessage)]
        [StringLength(PartClientNumberMaxLength,
            MinimumLength = PartClientNumberMinLength,
            ErrorMessage = StringLength)]
        public string PartClientNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredMessage)]
        [Range(PartToolNumberMinValue, 
            PartToolNumberMaxValue, 
            ErrorMessage = StringNumberRange)]
        public int ToolNumber { get; set; }

        public int PartStatusId { get; set; }

        public IEnumerable<PartStatusServiceModel> Statuses { get; set; } = 
            new List<PartStatusServiceModel>();

        [Required(ErrorMessage = RequiredMessage)]
        [StringLength(PartImageUrlMaxLength,
            MinimumLength = PartImageUrlMinLength,
            ErrorMessage = StringLength)]
        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredMessage)]
        [StringLength(PartModelMaxLength,
            MinimumLength = PartModelMinLength,
            ErrorMessage = StringLength)]
        public string PartModel { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredMessage)]
        [StringLength(ProjectNumberMaxLength,
            MinimumLength = ProjectNumberMinLength,
            ErrorMessage = StringLength)]
        public string ProjectNumber { get; set; } = string.Empty;
    }
}
