using System.ComponentModel.DataAnnotations;
using TeamWorkFlow.Core.Constants;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Infrastructure.Constants;

namespace TeamWorkFlow.Core.Models.Part
{
    public class PartServiceModel : IPartModel
    {
        public int Id { get; set; }

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
