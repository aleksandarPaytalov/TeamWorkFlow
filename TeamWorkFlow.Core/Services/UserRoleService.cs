using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Admin.UserRole;
using TeamWorkFlow.Infrastructure.Common;
using TeamWorkFlow.Infrastructure.Data.Models;
using static TeamWorkFlow.Core.Constants.UsersConstants;

namespace TeamWorkFlow.Core.Services
{
    /// <summary>
    /// Service for managing user roles (Admin/Operator)
    /// </summary>
    public class UserRoleService : IUserRoleService
    {
        private readonly IRepository _repository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRoleService(
            IRepository repository,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _repository = repository;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<UserRoleViewModel>> GetAllUsersWithRolesAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRoles = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var operatorEntity = await _repository.AllReadOnly<Operator>()
                    .Include(o => o.AvailabilityStatus)
                    .FirstOrDefaultAsync(o => o.UserId == user.Id);

                var userRole = new UserRoleViewModel
                {
                    UserId = user.Id,
                    Email = user.Email ?? string.Empty,
                    FullName = operatorEntity?.FullName ?? user.Email?.Split('@')[0] ?? "Unknown",
                    CurrentRole = GetPrimaryRole(roles),
                    IsAdmin = roles.Contains(AdminRole),
                    IsOperator = roles.Contains(OperatorRole),
                    IsGuest = roles.Contains(GuestRole),
                    RegisteredDate = user.LockoutEnd?.DateTime, // Using available date field
                    IsActive = operatorEntity?.IsActive ?? false,
                    AvailabilityStatus = operatorEntity?.AvailabilityStatus?.Name,
                    PhoneNumber = operatorEntity?.PhoneNumber
                };

                userRole.CanPromoteToAdmin = await CanPromoteToAdminAsync(user.Id);
                userRole.CanDemoteFromAdmin = await CanDemoteFromAdminAsync(user.Id);
                userRole.CanAssignRole = await CanAssignRoleAsync(user.Id);

                userRoles.Add(userRole);
            }

            return userRoles.OrderBy(u => u.Email);
        }

        public async Task<UserRoleViewModel?> GetUserWithRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            var operatorEntity = await _repository.AllReadOnly<Operator>()
                .Include(o => o.AvailabilityStatus)
                .FirstOrDefaultAsync(o => o.UserId == userId);

            var userRole = new UserRoleViewModel
            {
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = operatorEntity?.FullName ?? user.Email?.Split('@')[0] ?? "Unknown",
                CurrentRole = GetPrimaryRole(roles),
                IsAdmin = roles.Contains(AdminRole),
                IsOperator = roles.Contains(OperatorRole),
                IsGuest = roles.Contains(GuestRole),
                RegisteredDate = user.LockoutEnd?.DateTime,
                IsActive = operatorEntity?.IsActive ?? false,
                AvailabilityStatus = operatorEntity?.AvailabilityStatus?.Name,
                PhoneNumber = operatorEntity?.PhoneNumber
            };

            userRole.CanPromoteToAdmin = await CanPromoteToAdminAsync(userId);
            userRole.CanDemoteFromAdmin = await CanDemoteFromAdminAsync(userId);
            userRole.CanAssignRole = await CanAssignRoleAsync(userId);

            return userRole;
        }

        public async Task<(bool Success, string Message)> PromoteToAdminAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return (false, "User not found.");
                }

                if (await _userManager.IsInRoleAsync(user, AdminRole))
                {
                    return (false, "User is already an administrator.");
                }

                // Ensure admin role exists
                if (!await _roleManager.RoleExistsAsync(AdminRole))
                {
                    await _roleManager.CreateAsync(new IdentityRole(AdminRole));
                }

                var result = await _userManager.AddToRoleAsync(user, AdminRole);
                if (result.Succeeded)
                {
                    return (true, $"User {user.Email} has been promoted to Administrator.");
                }

                return (false, $"Failed to promote user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DemoteToOperatorAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return (false, "User not found.");
                }

                if (!await _userManager.IsInRoleAsync(user, AdminRole))
                {
                    return (false, "User is not an administrator.");
                }

                // Check if this is the last admin
                var adminUsers = await _userManager.GetUsersInRoleAsync(AdminRole);
                if (adminUsers.Count <= 1)
                {
                    return (false, "Cannot demote the last administrator. At least one admin must remain.");
                }

                var result = await _userManager.RemoveFromRoleAsync(user, AdminRole);
                if (result.Succeeded)
                {
                    // Ensure user has operator role if they have an operator record
                    var operatorEntity = await _repository.AllReadOnly<Operator>()
                        .FirstOrDefaultAsync(o => o.UserId == userId);

                    if (operatorEntity != null && !await _userManager.IsInRoleAsync(user, OperatorRole))
                    {
                        if (!await _roleManager.RoleExistsAsync(OperatorRole))
                        {
                            await _roleManager.CreateAsync(new IdentityRole(OperatorRole));
                        }
                        await _userManager.AddToRoleAsync(user, OperatorRole);
                    }

                    return (true, $"User {user.Email} has been demoted from Administrator.");
                }

                return (false, $"Failed to demote user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<bool> CanPromoteToAdminAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            // Can promote if user is not already an admin
            return !await _userManager.IsInRoleAsync(user, AdminRole);
        }

        public async Task<bool> CanDemoteFromAdminAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            // Can demote if user is admin and not the last admin
            if (!await _userManager.IsInRoleAsync(user, AdminRole)) return false;

            var adminUsers = await _userManager.GetUsersInRoleAsync(AdminRole);
            return adminUsers.Count > 1;
        }

        public async Task<UserRoleStatsViewModel> GetRoleStatsAsync()
        {
            var allUsers = await _userManager.Users.ToListAsync();
            var adminUsers = await _userManager.GetUsersInRoleAsync(AdminRole);
            var operatorUsers = await _userManager.GetUsersInRoleAsync(OperatorRole);

            var activeOperators = await _repository.AllReadOnly<Operator>()
                .Where(o => o.IsActive)
                .CountAsync();

            var inactiveOperators = await _repository.AllReadOnly<Operator>()
                .Where(o => !o.IsActive)
                .CountAsync();

            var usersWithRoles = adminUsers.Concat(operatorUsers).Distinct().Count();

            return new UserRoleStatsViewModel
            {
                TotalUsers = allUsers.Count,
                AdminCount = adminUsers.Count,
                OperatorCount = operatorUsers.Count,
                NoRoleCount = allUsers.Count - usersWithRoles,
                ActiveOperatorCount = activeOperators,
                InactiveOperatorCount = inactiveOperators
            };
        }

        public async Task<(bool Success, string Message)> CreateDemotionRequestAsync(string targetUserId, string requestingUserId, string reason)
        {
            try
            {
                // Validate that both users exist and are admins
                var targetUser = await _userManager.FindByIdAsync(targetUserId);
                var requestingUser = await _userManager.FindByIdAsync(requestingUserId);

                if (targetUser == null || requestingUser == null)
                {
                    return (false, "One or more users not found.");
                }

                if (!await _userManager.IsInRoleAsync(targetUser, AdminRole))
                {
                    return (false, "Target user is not an administrator.");
                }

                if (!await _userManager.IsInRoleAsync(requestingUser, AdminRole))
                {
                    return (false, "Only administrators can create demotion requests.");
                }

                if (targetUserId == requestingUserId)
                {
                    return (false, "You cannot create a demotion request for yourself.");
                }

                // Check if there's already a pending request for this user
                var existingRequest = await _repository.AllReadOnly<AdminDemotionRequest>()
                    .Where(r => r.TargetUserId == targetUserId && r.Status == DemotionRequestStatus.Pending)
                    .FirstOrDefaultAsync();

                if (existingRequest != null)
                {
                    return (false, "There is already a pending demotion request for this administrator.");
                }

                // Check if this would be the last admin
                var adminUsers = await _userManager.GetUsersInRoleAsync(AdminRole);
                if (adminUsers.Count <= 2) // Current admin + target admin = 2, so demoting would leave only 1
                {
                    return (false, "Cannot create demotion request. At least two administrators must remain in the system.");
                }

                // Create the demotion request
                var demotionRequest = new AdminDemotionRequest
                {
                    TargetUserId = targetUserId,
                    RequestedByUserId = requestingUserId,
                    Reason = reason,
                    RequestedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(7), // 7 days to approve
                    Status = DemotionRequestStatus.Pending
                };

                await _repository.AddAsync(demotionRequest);
                await _repository.SaveChangesAsync();

                return (true, $"Demotion request created successfully. Another administrator must approve this request within 7 days.");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<IEnumerable<DemotionRequestViewModel>> GetPendingDemotionRequestsAsync()
        {
            var requests = await _repository.AllReadOnly<AdminDemotionRequest>()
                .Include(r => r.TargetUser)
                .Include(r => r.RequestedByUser)
                .Include(r => r.ApprovedByUser)
                .Where(r => r.Status == DemotionRequestStatus.Pending && r.ExpiresAt > DateTime.UtcNow)
                .OrderBy(r => r.RequestedAt)
                .ToListAsync();

            return await MapDemotionRequestsToViewModels(requests);
        }

        public async Task<IEnumerable<DemotionRequestViewModel>> GetAllDemotionRequestsAsync()
        {
            var requests = await _repository.AllReadOnly<AdminDemotionRequest>()
                .Include(r => r.TargetUser)
                .Include(r => r.RequestedByUser)
                .Include(r => r.ApprovedByUser)
                .OrderByDescending(r => r.RequestedAt)
                .ToListAsync();

            return await MapDemotionRequestsToViewModels(requests);
        }

        public async Task<DemotionRequestViewModel?> GetDemotionRequestAsync(int requestId)
        {
            var request = await _repository.AllReadOnly<AdminDemotionRequest>()
                .Include(r => r.TargetUser)
                .Include(r => r.RequestedByUser)
                .Include(r => r.ApprovedByUser)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null) return null;

            var viewModels = await MapDemotionRequestsToViewModels(new[] { request });
            return viewModels.FirstOrDefault();
        }

        public async Task<(bool Success, string Message)> ApproveDemotionRequestAsync(int requestId, string approvingUserId, string? comments = null)
        {
            try
            {
                var request = await _repository.All<AdminDemotionRequest>()
                    .Include(r => r.TargetUser)
                    .Include(r => r.RequestedByUser)
                    .FirstOrDefaultAsync(r => r.Id == requestId);

                if (request == null)
                {
                    return (false, "Demotion request not found.");
                }

                if (request.Status != DemotionRequestStatus.Pending)
                {
                    return (false, "This request has already been processed.");
                }

                if (request.IsExpired)
                {
                    request.Status = DemotionRequestStatus.Expired;
                    await _repository.SaveChangesAsync();
                    return (false, "This request has expired.");
                }

                var approvingUser = await _userManager.FindByIdAsync(approvingUserId);
                if (approvingUser == null || !await _userManager.IsInRoleAsync(approvingUser, AdminRole))
                {
                    return (false, "Only administrators can approve demotion requests.");
                }

                if (request.RequestedByUserId == approvingUserId)
                {
                    return (false, "You cannot approve your own demotion request.");
                }

                if (request.TargetUserId == approvingUserId)
                {
                    return (false, "You cannot approve a demotion request targeting yourself.");
                }

                // Perform the actual demotion
                var demoteResult = await DemoteToOperatorAsync(request.TargetUserId);
                if (!demoteResult.Success)
                {
                    return (false, $"Failed to demote user: {demoteResult.Message}");
                }

                // Update the request
                request.Status = DemotionRequestStatus.Approved;
                request.ApprovedByUserId = approvingUserId;
                request.ProcessedAt = DateTime.UtcNow;
                request.ApprovalComments = comments;

                await _repository.SaveChangesAsync();

                return (true, $"Demotion request approved successfully. {request.TargetUser.Email} has been demoted from Administrator to Operator.");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> RejectDemotionRequestAsync(int requestId, string rejectingUserId, string? comments = null)
        {
            try
            {
                var request = await _repository.All<AdminDemotionRequest>()
                    .Include(r => r.TargetUser)
                    .FirstOrDefaultAsync(r => r.Id == requestId);

                if (request == null)
                {
                    return (false, "Demotion request not found.");
                }

                if (request.Status != DemotionRequestStatus.Pending)
                {
                    return (false, "This request has already been processed.");
                }

                var rejectingUser = await _userManager.FindByIdAsync(rejectingUserId);
                if (rejectingUser == null || !await _userManager.IsInRoleAsync(rejectingUser, AdminRole))
                {
                    return (false, "Only administrators can reject demotion requests.");
                }

                if (request.RequestedByUserId == rejectingUserId)
                {
                    return (false, "You cannot reject your own demotion request.");
                }

                if (request.TargetUserId == rejectingUserId)
                {
                    return (false, "You cannot reject a demotion request targeting yourself.");
                }

                // Update the request
                request.Status = DemotionRequestStatus.Rejected;
                request.ApprovedByUserId = rejectingUserId;
                request.ProcessedAt = DateTime.UtcNow;
                request.ApprovalComments = comments;

                await _repository.SaveChangesAsync();

                return (true, $"Demotion request rejected successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> CancelDemotionRequestAsync(int requestId, string cancellingUserId)
        {
            try
            {
                var request = await _repository.All<AdminDemotionRequest>()
                    .FirstOrDefaultAsync(r => r.Id == requestId);

                if (request == null)
                {
                    return (false, "Demotion request not found.");
                }

                if (request.Status != DemotionRequestStatus.Pending)
                {
                    return (false, "This request has already been processed.");
                }

                if (request.RequestedByUserId != cancellingUserId)
                {
                    return (false, "You can only cancel your own demotion requests.");
                }

                // Update the request
                request.Status = DemotionRequestStatus.Cancelled;
                request.ProcessedAt = DateTime.UtcNow;

                await _repository.SaveChangesAsync();

                return (true, "Demotion request cancelled successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<int> GetPendingDemotionRequestCountAsync()
        {
            return await _repository.AllReadOnly<AdminDemotionRequest>()
                .Where(r => r.Status == DemotionRequestStatus.Pending && r.ExpiresAt > DateTime.UtcNow)
                .CountAsync();
        }

        public async Task<bool> CanCreateDemotionRequestAsync(string targetUserId, string requestingUserId)
        {
            if (targetUserId == requestingUserId) return false;

            var targetUser = await _userManager.FindByIdAsync(targetUserId);
            var requestingUser = await _userManager.FindByIdAsync(requestingUserId);

            if (targetUser == null || requestingUser == null) return false;

            if (!await _userManager.IsInRoleAsync(targetUser, AdminRole)) return false;
            if (!await _userManager.IsInRoleAsync(requestingUser, AdminRole)) return false;

            // Check if there's already a pending request
            var existingRequest = await _repository.AllReadOnly<AdminDemotionRequest>()
                .Where(r => r.TargetUserId == targetUserId && r.Status == DemotionRequestStatus.Pending)
                .AnyAsync();

            if (existingRequest) return false;

            // Check if this would leave less than 2 admins
            var adminUsers = await _userManager.GetUsersInRoleAsync(AdminRole);
            return adminUsers.Count > 2;
        }

        public async Task<bool> CanApproveDemotionRequestAsync(int requestId, string userId)
        {
            var request = await _repository.AllReadOnly<AdminDemotionRequest>()
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null) return false;
            if (request.Status != DemotionRequestStatus.Pending) return false;
            if (request.IsExpired) return false;
            if (request.RequestedByUserId == userId) return false; // Can't approve own request
            if (request.TargetUserId == userId) return false; // Can't approve demotion targeting yourself

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            return await _userManager.IsInRoleAsync(user, AdminRole);
        }

        public async Task<bool> CanRejectDemotionRequestAsync(int requestId, string userId)
        {
            var request = await _repository.AllReadOnly<AdminDemotionRequest>()
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null) return false;
            if (request.Status != DemotionRequestStatus.Pending) return false;
            if (request.IsExpired) return false;
            if (request.RequestedByUserId == userId) return false; // Can't reject own request
            if (request.TargetUserId == userId) return false; // Can't reject demotion targeting yourself

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            return await _userManager.IsInRoleAsync(user, AdminRole);
        }

        private async Task<IEnumerable<DemotionRequestViewModel>> MapDemotionRequestsToViewModels(IEnumerable<AdminDemotionRequest> requests)
        {
            var viewModels = new List<DemotionRequestViewModel>();

            foreach (var request in requests)
            {
                var targetOperator = await _repository.AllReadOnly<Operator>()
                    .FirstOrDefaultAsync(o => o.UserId == request.TargetUserId);

                var requestingOperator = await _repository.AllReadOnly<Operator>()
                    .FirstOrDefaultAsync(o => o.UserId == request.RequestedByUserId);

                var approvingOperator = request.ApprovedByUserId != null
                    ? await _repository.AllReadOnly<Operator>()
                        .FirstOrDefaultAsync(o => o.UserId == request.ApprovedByUserId)
                    : null;

                var viewModel = new DemotionRequestViewModel
                {
                    Id = request.Id,
                    TargetUserId = request.TargetUserId,
                    TargetUserEmail = request.TargetUser.Email ?? string.Empty,
                    TargetUserFullName = targetOperator?.FullName ?? request.TargetUser.Email?.Split('@')[0] ?? "Unknown",
                    RequestedByUserEmail = request.RequestedByUser.Email ?? string.Empty,
                    RequestedByUserFullName = requestingOperator?.FullName ?? request.RequestedByUser.Email?.Split('@')[0] ?? "Unknown",
                    ApprovedByUserEmail = request.ApprovedByUser?.Email,
                    ApprovedByUserFullName = approvingOperator?.FullName ?? request.ApprovedByUser?.Email?.Split('@')[0],
                    Reason = request.Reason,
                    RequestedAt = request.RequestedAt,
                    ProcessedAt = request.ProcessedAt,
                    Status = request.Status,
                    ApprovalComments = request.ApprovalComments,
                    ExpiresAt = request.ExpiresAt,
                    IsExpired = request.IsExpired,
                    IsPendingAndValid = request.IsPendingAndValid
                };

                viewModels.Add(viewModel);
            }

            return viewModels;
        }

        public async Task<bool> CanAssignRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            // Can assign role if user is only a guest
            return await _userManager.IsInRoleAsync(user, GuestRole) &&
                   !await _userManager.IsInRoleAsync(user, AdminRole) &&
                   !await _userManager.IsInRoleAsync(user, OperatorRole);
        }

        public async Task<(bool Success, string Message)> AssignOperatorRoleAsync(string userId, string fullName, string phoneNumber)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return (false, "User not found.");
                }

                if (!await _userManager.IsInRoleAsync(user, GuestRole))
                {
                    return (false, "User must be a guest to be assigned operator role.");
                }

                if (await _userManager.IsInRoleAsync(user, OperatorRole))
                {
                    return (false, "User is already an operator.");
                }

                // Ensure operator role exists
                if (!await _roleManager.RoleExistsAsync(OperatorRole))
                {
                    await _roleManager.CreateAsync(new IdentityRole(OperatorRole));
                }

                // Remove guest role and add operator role
                await _userManager.RemoveFromRoleAsync(user, GuestRole);
                var result = await _userManager.AddToRoleAsync(user, OperatorRole);

                if (result.Succeeded)
                {
                    // Create operator record in database
                    var operatorModel = new Operator()
                    {
                        FullName = fullName,
                        Email = user.Email ?? string.Empty,
                        PhoneNumber = phoneNumber,
                        AvailabilityStatusId = 2, // Default to "In Sick Leave" - inactive status
                        IsActive = false, // Start as inactive
                        Capacity = 8, // Default capacity
                        UserId = userId
                    };

                    await _repository.AddAsync(operatorModel);
                    await _repository.SaveChangesAsync();

                    return (true, $"User {user.Email} has been assigned operator role and added to operator database.");
                }

                return (false, $"Failed to assign operator role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> AssignAdminRoleAsync(string userId, string fullName, string phoneNumber)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return (false, "User not found.");
                }

                if (!await _userManager.IsInRoleAsync(user, GuestRole))
                {
                    return (false, "User must be a guest to be assigned admin role.");
                }

                if (await _userManager.IsInRoleAsync(user, AdminRole))
                {
                    return (false, "User is already an administrator.");
                }

                // Ensure admin role exists
                if (!await _roleManager.RoleExistsAsync(AdminRole))
                {
                    await _roleManager.CreateAsync(new IdentityRole(AdminRole));
                }

                // Remove guest role and add admin role
                await _userManager.RemoveFromRoleAsync(user, GuestRole);
                var result = await _userManager.AddToRoleAsync(user, AdminRole);

                if (result.Succeeded)
                {
                    // Create operator record in database (admins are also operators)
                    var operatorModel = new Operator()
                    {
                        FullName = fullName,
                        Email = user.Email ?? string.Empty,
                        PhoneNumber = phoneNumber,
                        AvailabilityStatusId = 1, // "At work" status for admins
                        IsActive = true, // Admins start as active
                        Capacity = 8, // Default capacity
                        UserId = userId
                    };

                    await _repository.AddAsync(operatorModel);
                    await _repository.SaveChangesAsync();

                    return (true, $"User {user.Email} has been assigned administrator role and added to operator database.");
                }

                return (false, $"Failed to assign admin role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        private static string GetPrimaryRole(IList<string> roles)
        {
            if (roles.Contains(AdminRole)) return AdminRole;
            if (roles.Contains(OperatorRole)) return OperatorRole;
            if (roles.Contains(GuestRole)) return GuestRole;
            return "No Role";
        }
    }
}
