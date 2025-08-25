using System.ComponentModel.DataAnnotations;

namespace TeamWorkFlow.Core.Models.Admin.UserRole
{
    /// <summary>
    /// View model for displaying user role information
    /// </summary>
    public class UserRoleViewModel
    {
        /// <summary>
        /// User ID
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// User email address
        /// </summary>
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's full name (from Operator table if available)
        /// </summary>
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Current role of the user
        /// </summary>
        [Display(Name = "Current Role")]
        public string CurrentRole { get; set; } = string.Empty;

        /// <summary>
        /// Whether the user is an admin
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Whether the user is an operator
        /// </summary>
        public bool IsOperator { get; set; }

        /// <summary>
        /// Whether user is a guest
        /// </summary>
        public bool IsGuest { get; set; }

        /// <summary>
        /// Whether the user can be promoted to admin
        /// </summary>
        public bool CanPromoteToAdmin { get; set; }

        /// <summary>
        /// Whether the user can be demoted from admin
        /// </summary>
        public bool CanDemoteFromAdmin { get; set; }

        /// <summary>
        /// Whether the user can be assigned a role (Guest users)
        /// </summary>
        public bool CanAssignRole { get; set; }

        /// <summary>
        /// Whether the user can be demoted to guest (Operator users only)
        /// </summary>
        public bool CanDemoteToGuest { get; set; }

        /// <summary>
        /// User registration date
        /// </summary>
        [Display(Name = "Registered")]
        public DateTime? RegisteredDate { get; set; }

        /// <summary>
        /// Whether the user is active (has operator record)
        /// </summary>
        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Operator availability status (if applicable)
        /// </summary>
        [Display(Name = "Availability")]
        public string? AvailabilityStatus { get; set; }

        /// <summary>
        /// Phone number (if available from operator record)
        /// </summary>
        [Display(Name = "Phone")]
        public string? PhoneNumber { get; set; }
    }
}
