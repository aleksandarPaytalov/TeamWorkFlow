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

        /// <summary>
        /// Creates a demotion request for an admin user
        /// </summary>
        /// <param name="targetUserId">User ID to demote</param>
        /// <param name="requestingUserId">User ID of the admin making the request</param>
        /// <param name="reason">Reason for demotion</param>
        /// <returns>Success status and message</returns>
        Task<(bool Success, string Message)> CreateDemotionRequestAsync(string targetUserId, string requestingUserId, string reason);

        /// <summary>
        /// Gets all pending demotion requests
        /// </summary>
        /// <returns>List of pending demotion requests</returns>
        Task<IEnumerable<DemotionRequestViewModel>> GetPendingDemotionRequestsAsync();

        /// <summary>
        /// Gets all demotion requests (pending and processed)
        /// </summary>
        /// <returns>List of all demotion requests</returns>
        Task<IEnumerable<DemotionRequestViewModel>> GetAllDemotionRequestsAsync();

        /// <summary>
        /// Gets a specific demotion request by ID
        /// </summary>
        /// <param name="requestId">Request ID</param>
        /// <returns>Demotion request details</returns>
        Task<DemotionRequestViewModel?> GetDemotionRequestAsync(int requestId);

        /// <summary>
        /// Approves a demotion request
        /// </summary>
        /// <param name="requestId">Request ID</param>
        /// <param name="approvingUserId">User ID of the admin approving the request</param>
        /// <param name="comments">Optional comments</param>
        /// <returns>Success status and message</returns>
        Task<(bool Success, string Message)> ApproveDemotionRequestAsync(int requestId, string approvingUserId, string? comments = null);

        /// <summary>
        /// Rejects a demotion request
        /// </summary>
        /// <param name="requestId">Request ID</param>
        /// <param name="rejectingUserId">User ID of the admin rejecting the request</param>
        /// <param name="comments">Optional comments</param>
        /// <returns>Success status and message</returns>
        Task<(bool Success, string Message)> RejectDemotionRequestAsync(int requestId, string rejectingUserId, string? comments = null);

        /// <summary>
        /// Cancels a demotion request
        /// </summary>
        /// <param name="requestId">Request ID</param>
        /// <param name="cancellingUserId">User ID of the admin cancelling the request</param>
        /// <returns>Success status and message</returns>
        Task<(bool Success, string Message)> CancelDemotionRequestAsync(int requestId, string cancellingUserId);

        /// <summary>
        /// Gets the count of pending demotion requests
        /// </summary>
        /// <returns>Number of pending requests</returns>
        Task<int> GetPendingDemotionRequestCountAsync();

        /// <summary>
        /// Checks if a user can create a demotion request for another user
        /// </summary>
        /// <param name="targetUserId">User ID to demote</param>
        /// <param name="requestingUserId">User ID making the request</param>
        /// <returns>True if request can be created</returns>
        Task<bool> CanCreateDemotionRequestAsync(string targetUserId, string requestingUserId);

        /// <summary>
        /// Checks if a user can approve a specific demotion request
        /// </summary>
        /// <param name="requestId">Request ID</param>
        /// <param name="userId">User ID of potential approver</param>
        /// <returns>True if user can approve</returns>
        Task<bool> CanApproveDemotionRequestAsync(int requestId, string userId);
    }
}
