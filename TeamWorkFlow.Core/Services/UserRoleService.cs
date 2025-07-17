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
                    RegisteredDate = user.LockoutEnd?.DateTime, // Using available date field
                    IsActive = operatorEntity?.IsActive ?? false,
                    AvailabilityStatus = operatorEntity?.AvailabilityStatus?.Name,
                    PhoneNumber = operatorEntity?.PhoneNumber
                };

                userRole.CanPromoteToAdmin = await CanPromoteToAdminAsync(user.Id);
                userRole.CanDemoteFromAdmin = await CanDemoteFromAdminAsync(user.Id);

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
                RegisteredDate = user.LockoutEnd?.DateTime,
                IsActive = operatorEntity?.IsActive ?? false,
                AvailabilityStatus = operatorEntity?.AvailabilityStatus?.Name,
                PhoneNumber = operatorEntity?.PhoneNumber
            };

            userRole.CanPromoteToAdmin = await CanPromoteToAdminAsync(userId);
            userRole.CanDemoteFromAdmin = await CanDemoteFromAdminAsync(userId);

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

        private static string GetPrimaryRole(IList<string> roles)
        {
            if (roles.Contains(AdminRole)) return AdminRole;
            if (roles.Contains(OperatorRole)) return OperatorRole;
            return "No Role";
        }
    }
}
