using Microsoft.AspNetCore.Mvc;

namespace TeamWorkFlow.Controllers
{
    /// <summary>
    /// Controller for testing error pages in development environment
    /// Remove this controller in production
    /// </summary>
    public class ErrorTestController : Controller
    {
        /// <summary>
        /// Test page to trigger different error status codes
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Trigger 400 Bad Request error
        /// </summary>
        public IActionResult Test400()
        {
            return StatusCode(400);
        }

        /// <summary>
        /// Trigger 401 Unauthorized error
        /// </summary>
        public IActionResult Test401()
        {
            return StatusCode(401);
        }

        /// <summary>
        /// Trigger 403 Forbidden error
        /// </summary>
        public IActionResult Test403()
        {
            return StatusCode(403);
        }

        /// <summary>
        /// Trigger 404 Not Found error
        /// </summary>
        public IActionResult Test404()
        {
            return StatusCode(404);
        }

        /// <summary>
        /// Trigger 408 Request Timeout error
        /// </summary>
        public IActionResult Test408()
        {
            return StatusCode(408);
        }

        /// <summary>
        /// Trigger 429 Too Many Requests error
        /// </summary>
        public IActionResult Test429()
        {
            return StatusCode(429);
        }

        /// <summary>
        /// Trigger 500 Internal Server Error
        /// </summary>
        public IActionResult Test500()
        {
            return StatusCode(500);
        }

        /// <summary>
        /// Trigger a custom error code for testing generic error page
        /// </summary>
        public IActionResult TestCustom(int code = 418)
        {
            return StatusCode(code);
        }
    }
}
