using Microsoft.AspNetCore.Mvc;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Admin.UserRole;
using TeamWorkFlow.Extensions;
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
                var pendingCount = await _userRoleService.GetPendingDemotionRequestCountAsync();

                ViewBag.Stats = stats;
                ViewBag.PendingCount = pendingCount;
                return View(users);
            }
            catch (Exception ex)
            {
                TempData[UserMessageError] = $"Error loading users: {ex.Message}";
                return View(new List<UserRoleViewModel>());
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
        /// Create a demotion request for an admin user (requires second admin approval)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Redirect to create demotion request form</returns>
        [HttpGet]
        public async Task<IActionResult> RequestDemotion(string id)
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

                if (!user.IsAdmin)
                {
                    TempData[UserMessageError] = "User is not an administrator.";
                    return RedirectToAction(nameof(Index));
                }

                var model = new CreateDemotionRequestViewModel
                {
                    TargetUserId = id,
                    TargetUserEmail = user.Email,
                    TargetUserFullName = user.FullName
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData[UserMessageError] = $"Error loading user details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Create a demotion request for an admin user
        /// </summary>
        /// <param name="model">Demotion request details</param>
        /// <returns>Redirect with result message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestDemotion(string id, CreateDemotionRequestViewModel model)
        {
            // Debug logging to see what values we're receiving
            var logger = HttpContext.RequestServices.GetRequiredService<ILogger<UserRoleController>>();
            logger.LogInformation($"RequestDemotion POST - AcknowledgeApprovalRequired: {model.AcknowledgeApprovalRequired}");
            logger.LogInformation($"RequestDemotion POST - ModelState.IsValid: {ModelState.IsValid}");

            // Debug: Log the checkbox value
            logger.LogInformation($"DEBUG: AcknowledgeApprovalRequired value: {model.AcknowledgeApprovalRequired}");

            // Custom validation for the acknowledgment checkbox
            if (!model.AcknowledgeApprovalRequired)
            {
                logger.LogInformation("DEBUG: Checkbox validation failed - value is false");
                ModelState.AddModelError(nameof(model.AcknowledgeApprovalRequired),
                    "You must acknowledge that this request requires approval from another administrator.");
            }
            else
            {
                logger.LogInformation("DEBUG: Checkbox validation passed - value is true");
            }

            if (!ModelState.IsValid)
            {
                // Log validation errors
                foreach (var error in ModelState)
                {
                    logger.LogInformation($"ModelState Error - Key: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }

                // Repopulate model with target user information
                var user = await _userRoleService.GetUserWithRoleAsync(id);
                if (user != null)
                {
                    model.TargetUserId = id;
                    model.TargetUserEmail = user.Email;
                    model.TargetUserFullName = user.FullName;
                }

                return View(model);
            }

            try
            {
                var currentUserId = User.Id(); // Using the extension method
                if (string.IsNullOrEmpty(currentUserId))
                {
                    TempData[UserMessageError] = "Unable to identify current user.";
                    return RedirectToAction(nameof(Index));
                }

                var (success, message) = await _userRoleService.CreateDemotionRequestAsync(
                    id,
                    currentUserId,
                    model.Reason);

                if (success)
                {
                    TempData[UserMessageSuccess] = message;
                    return RedirectToAction(nameof(DemotionRequests));
                }
                else
                {
                    TempData[UserMessageError] = message;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData[UserMessageError] = $"Error creating demotion request: {ex.Message}";
                return View(model);
            }
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

        /// <summary>
        /// Display all demotion requests
        /// </summary>
        /// <returns>View with demotion requests</returns>
        [HttpGet]
        public async Task<IActionResult> DemotionRequests()
        {
            try
            {
                var requests = await _userRoleService.GetAllDemotionRequestsAsync();
                var pendingCount = await _userRoleService.GetPendingDemotionRequestCountAsync();

                ViewBag.PendingCount = pendingCount;
                return View(requests);
            }
            catch (Exception ex)
            {
                TempData[UserMessageError] = $"Error loading demotion requests: {ex.Message}";
                return View(new List<DemotionRequestViewModel>());
            }
        }

        /// <summary>
        /// Display details for a specific demotion request
        /// </summary>
        /// <param name="id">Request ID</param>
        /// <returns>View with request details</returns>
        [HttpGet]
        public async Task<IActionResult> DemotionRequestDetails(int id)
        {
            try
            {
                var request = await _userRoleService.GetDemotionRequestAsync(id);
                if (request == null)
                {
                    TempData[UserMessageError] = "Demotion request not found.";
                    return RedirectToAction(nameof(DemotionRequests));
                }

                var currentUserId = User.Id();
                request.CanApprove = await _userRoleService.CanApproveDemotionRequestAsync(id, currentUserId);
                request.CanReject = request.CanApprove; // Same logic for reject
                request.CanCancel = request.RequestedByUserEmail == User.Identity?.Name && request.IsPendingAndValid;

                return View(request);
            }
            catch (Exception ex)
            {
                TempData[UserMessageError] = $"Error loading request details: {ex.Message}";
                return RedirectToAction(nameof(DemotionRequests));
            }
        }

        /// <summary>
        /// Approve a demotion request
        /// </summary>
        /// <param name="id">Request ID</param>
        /// <param name="comments">Optional comments</param>
        /// <returns>Redirect with result message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveDemotionRequest(int id, string? comments)
        {
            try
            {
                var currentUserId = User.Id();
                var (success, message) = await _userRoleService.ApproveDemotionRequestAsync(id, currentUserId, comments);

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
                TempData[UserMessageError] = $"Error approving request: {ex.Message}";
            }

            return RedirectToAction(nameof(DemotionRequests));
        }

        /// <summary>
        /// Reject a demotion request
        /// </summary>
        /// <param name="id">Request ID</param>
        /// <param name="comments">Optional comments</param>
        /// <returns>Redirect with result message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectDemotionRequest(int id, string? comments)
        {
            try
            {
                var currentUserId = User.Id();
                var (success, message) = await _userRoleService.RejectDemotionRequestAsync(id, currentUserId, comments);

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
                TempData[UserMessageError] = $"Error rejecting request: {ex.Message}";
            }

            return RedirectToAction(nameof(DemotionRequests));
        }

        /// <summary>
        /// Cancel a demotion request
        /// </summary>
        /// <param name="id">Request ID</param>
        /// <returns>Redirect with result message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelDemotionRequest(int id)
        {
            try
            {
                var currentUserId = User.Id();
                var (success, message) = await _userRoleService.CancelDemotionRequestAsync(id, currentUserId);

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
                TempData[UserMessageError] = $"Error cancelling request: {ex.Message}";
            }

            return RedirectToAction(nameof(DemotionRequests));
        }
    }
}
