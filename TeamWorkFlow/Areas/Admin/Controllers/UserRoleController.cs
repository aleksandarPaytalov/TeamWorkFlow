using Microsoft.AspNetCore.Mvc;
using TeamWorkFlow.Core.Contracts;
using static TeamWorkFlow.Core.Constants.Messages;

namespace TeamWorkFlow.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for managing user roles in the admin area
    /// </summary>
    public class UserRoleController : AdminBaseController
    {
        private readonly IUserRoleService _userRoleService;

        public UserRoleController(IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        /// <summary>
        /// Display all users with their roles
        /// </summary>
        /// <returns>View with user role list</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var users = await _userRoleService.GetAllUsersWithRolesAsync();
                var stats = await _userRoleService.GetRoleStatsAsync();

                ViewBag.Stats = stats;
                return View(users);
            }
            catch (Exception ex)
            {
                TempData[UserMessageError] = $"Error loading users: {ex.Message}";
                return View(new List<TeamWorkFlow.Core.Models.Admin.UserRole.UserRoleViewModel>());
            }
        }

        /// <summary>
        /// Display details for a specific user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>View with user details</returns>
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData[UserMessageError] = "User ID is required.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var user = await _userRoleService.GetUserWithRoleAsync(id);
                if (user == null)
                {
                    TempData[UserMessageError] = "User not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(user);
            }
            catch (Exception ex)
            {
                TempData[UserMessageError] = $"Error loading user details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Promote a user to admin role
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Redirect to index with result message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PromoteToAdmin(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData[UserMessageError] = "User ID is required.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var (success, message) = await _userRoleService.PromoteToAdminAsync(id);
                
                if (success)
                {
                    TempData[UserMessageSuccess] = message;
                }
                else
                {
                    TempData[UserMessageError] = message;
                }
            }
            catch (Exception ex)
            {
                TempData[UserMessageError] = $"Error promoting user: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Demote a user from admin role
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Redirect to index with result message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DemoteFromAdmin(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData[UserMessageError] = "User ID is required.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var (success, message) = await _userRoleService.DemoteToOperatorAsync(id);
                
                if (success)
                {
                    TempData[UserMessageSuccess] = message;
                }
                else
                {
                    TempData[UserMessageError] = message;
                }
            }
            catch (Exception ex)
            {
                TempData[UserMessageError] = $"Error demoting user: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// AJAX endpoint to check if user can be promoted
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>JSON result</returns>
        [HttpGet]
        public async Task<IActionResult> CanPromote(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { canPromote = false, message = "User ID is required." });
            }

            try
            {
                var canPromote = await _userRoleService.CanPromoteToAdminAsync(id);
                return Json(new { canPromote, message = canPromote ? "User can be promoted." : "User cannot be promoted." });
            }
            catch (Exception ex)
            {
                return Json(new { canPromote = false, message = $"Error: {ex.Message}" });
            }
        }

        /// <summary>
        /// AJAX endpoint to check if user can be demoted
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>JSON result</returns>
        [HttpGet]
        public async Task<IActionResult> CanDemote(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { canDemote = false, message = "User ID is required." });
            }

            try
            {
                var canDemote = await _userRoleService.CanDemoteFromAdminAsync(id);
                return Json(new { canDemote, message = canDemote ? "User can be demoted." : "User cannot be demoted." });
            }
            catch (Exception ex)
            {
                return Json(new { canDemote = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}
