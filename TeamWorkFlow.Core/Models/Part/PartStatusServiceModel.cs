using System.ComponentModel.DataAnnotations;
using TeamWorkFlow.Core.Constants;
using TeamWorkFlow.Infrastructure.Constants;

namespace TeamWorkFlow.Core.Models.Part
{
    public class PartStatusServiceModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = Messages.RequiredMessage)]
        [StringLength(DataConstants.PartStatusNameMaxLength,
            MinimumLength = DataConstants.PartStatusNameMinLength,
            ErrorMessage = Messages.StringLength)]
        public string Name { get; set; } = string.Empty;
    }
}
