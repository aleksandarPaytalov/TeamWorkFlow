using System.ComponentModel.DataAnnotations;
using TeamWorkFlow.Core.Constants;
using TeamWorkFlow.Infrastructure.Constants;

namespace TeamWorkFlow.Core.Models.Part
{
    public class PartFormModel
    {
        [Required(ErrorMessage = Messages.RequiredMessage)]
        [StringLength(DataConstants.PartNameMaxLength,
            MinimumLength = DataConstants.PartNameMinLength,
            ErrorMessage = Messages.StringLength)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = Messages.RequiredMessage)]
        [StringLength(DataConstants.PartArticleNumberMaxLength,
            MinimumLength = DataConstants.PartArticleNumberMinLength,
            ErrorMessage = Messages.StringLength)]
        public string PartArticleNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = Messages.RequiredMessage)]
        [StringLength(DataConstants.PartClientNumberMaxLength,
            MinimumLength = DataConstants.PartClientNumberMinLength,
            ErrorMessage = Messages.StringLength)]
        public string PartClientNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = Messages.RequiredMessage)]
        [Range(DataConstants.PartToolNumberMinValue, 
            DataConstants.PartToolNumberMaxValue, 
            ErrorMessage = Messages.StringNumberRange)]
        public int ToolNumber { get; set; }

        public int PartStatusId { get; set; }

        public IEnumerable<PartStatusServiceModel> Statuses { get; set; } = 
            new List<PartStatusServiceModel>();

        [Required(ErrorMessage = Messages.RequiredMessage)]
        [StringLength(DataConstants.PartImageUrlMaxLength,
            MinimumLength = DataConstants.PartImageUrlMinLength,
            ErrorMessage = Messages.StringLength)]
        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = Messages.RequiredMessage)]
        [StringLength(DataConstants.PartModelMaxLength,
            MinimumLength = DataConstants.PartModelMinLength,
            ErrorMessage = Messages.StringLength)]
        public string PartModel { get; set; } = string.Empty;

        [Required(ErrorMessage = Messages.RequiredMessage)]
        [StringLength(DataConstants.ProjectNumberMaxLength,
            MinimumLength = DataConstants.ProjectNumberMinLength,
            ErrorMessage = Messages.StringLength)]
        public string ProjectNumber { get; set; } = string.Empty;

    }
}
