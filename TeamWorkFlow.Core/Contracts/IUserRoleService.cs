using TeamWorkFlow.Core.Models.Admin.UserRole;

namespace TeamWorkFlow.Core.Contracts
{
    /// <summary>
    /// Service for managing user roles (Admin/Operator)
    /// </summary>
    public interface IUserRoleService
    {
        /// <summary>
        /// Gets all users with their current roles
        /// </summary>
        /// <returns>List of users with role information</returns>
        Task<IEnumerable<UserRoleViewModel>> GetAllUsersWithRolesAsync();

        /// <summary>
        /// Gets a specific user with their role information
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User with role information</returns>
        Task<UserRoleViewModel?> GetUserWithRoleAsync(string userId);

        /// <summary>
        /// Promotes an operator to admin role
        /// </summary>
        /// <param name="userId">User ID to promote</param>
        /// <returns>Success status and message</returns>
        Task<(bool Success, string Message)> PromoteToAdminAsync(string userId);

        /// <summary>
        /// Demotes an admin to operator role
        /// </summary>
        /// <param name="userId">User ID to demote</param>
        /// <returns>Success status and message</returns>
        Task<(bool Success, string Message)> DemoteToOperatorAsync(string userId);

        /// <summary>
        /// Checks if a user can be promoted to admin
        /// </summary>
        /// <param name="userId">User ID to check</param>
        /// <returns>True if user can be promoted</returns>
        Task<bool> CanPromoteToAdminAsync(string userId);

        /// <summary>
        /// Checks if a user can be demoted from admin
        /// </summary>
        /// <param name="userId">User ID to check</param>
        /// <returns>True if user can be demoted</returns>
        Task<bool> CanDemoteFromAdminAsync(string userId);

        /// <summary>
        /// Gets the count of users by role
        /// </summary>
        /// <returns>Role statistics</returns>
        Task<UserRoleStatsViewModel> GetRoleStatsAsync();
    }
}
