using System.ComponentModel.DataAnnotations;
using TeamWorkFlow.Infrastructure.Data.Models;

namespace TeamWorkFlow.Core.Models.Admin.UserRole
{
    /// <summary>
    /// View model for processing (approving/rejecting) a demotion request
    /// </summary>
    public class ProcessDemotionRequestViewModel
    {
        /// <summary>
        /// Request ID
        /// </summary>
        public int RequestId { get; set; }

        /// <summary>
        /// Action to take (approve or reject)
        /// </summary>
        [Required]
        public DemotionRequestAction Action { get; set; }

        /// <summary>
        /// Comments from the processing admin
        /// </summary>
        [StringLength(500, ErrorMessage = "Comments cannot exceed 500 characters.")]
        [Display(Name = "Comments")]
        public string? Comments { get; set; }

        /// <summary>
        /// Request details for display
        /// </summary>
        public DemotionRequestViewModel? RequestDetails { get; set; }
    }

    /// <summary>
    /// Actions that can be taken on a demotion request
    /// </summary>
    public enum DemotionRequestAction
    {
        /// <summary>
        /// Approve the demotion request
        /// </summary>
        Approve = 1,

        /// <summary>
        /// Reject the demotion request
        /// </summary>
        Reject = 2
    }
}
