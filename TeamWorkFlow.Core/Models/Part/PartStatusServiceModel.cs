using System.ComponentModel.DataAnnotations;
using static TeamWorkFlow.Core.Constants.Messages;
using static TeamWorkFlow.Infrastructure.Constants.DataConstants;

namespace TeamWorkFlow.Core.Models.Part
{
    public class PartStatusServiceModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = RequiredMessage)]
        [StringLength(PartStatusNameMaxLength,
            MinimumLength = PartStatusNameMinLength,
            ErrorMessage = StringLength)]
        public string Name { get; set; } = string.Empty;
    }
}
