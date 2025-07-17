using System.ComponentModel.DataAnnotations;

namespace TeamWorkFlow.Core.Models.Admin.UserRole
{
    /// <summary>
    /// View model for displaying user role statistics
    /// </summary>
    public class UserRoleStatsViewModel
    {
        /// <summary>
        /// Total number of users
        /// </summary>
        [Display(Name = "Total Users")]
        public int TotalUsers { get; set; }

        /// <summary>
        /// Number of admin users
        /// </summary>
        [Display(Name = "Administrators")]
        public int AdminCount { get; set; }

        /// <summary>
        /// Number of operator users
        /// </summary>
        [Display(Name = "Operators")]
        public int OperatorCount { get; set; }

        /// <summary>
        /// Number of users with no specific role
        /// </summary>
        [Display(Name = "No Role Assigned")]
        public int NoRoleCount { get; set; }

        /// <summary>
        /// Number of active operators
        /// </summary>
        [Display(Name = "Active Operators")]
        public int ActiveOperatorCount { get; set; }

        /// <summary>
        /// Number of inactive operators
        /// </summary>
        [Display(Name = "Inactive Operators")]
        public int InactiveOperatorCount { get; set; }
    }
}
