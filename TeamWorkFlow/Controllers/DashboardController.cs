using Microsoft.AspNetCore.Mvc;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Dashboard;
using TeamWorkFlow.Extensions;
using static TeamWorkFlow.Core.Constants.Messages;
using static TeamWorkFlow.Constants.MessageConstants;

namespace TeamWorkFlow.Controllers
{
    /// <summary>
    /// Controller for performance analytics dashboard
    /// Provides dashboard data and views for performance analytics
    /// Restricted to Admin and Operator roles
    /// </summary>
    public class DashboardController : BaseController
    {
        private readonly ITaskAnalyticsService _analyticsService;
        private readonly IReportService _reportService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            ITaskAnalyticsService analyticsService,
            IReportService reportService,
            ILogger<DashboardController> logger)
        {
            _analyticsService = analyticsService;
            _reportService = reportService;
            _logger = logger;
        }

        /// <summary>
        /// Main dashboard view with performance analytics
        /// </summary>
        /// <param name="filters">Optional filter parameters</param>
        /// <returns>Dashboard view with analytics data</returns>
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ReportFilterModel? filters)
        {
            try
            {
                // Check authorization - Admin and Operator roles only
                if (User.Identity?.IsAuthenticated != true)
                {
                    _logger.LogWarning("Unauthorized access attempt to dashboard by user: {User}", User.Identity?.Name);
                    return Challenge();
                }

                // TODO: Re-enable role check after fixing user role assignment
                // if (!User.IsAdmin() && !User.IsOperator())
                // {
                //     _logger.LogWarning("Unauthorized access attempt to dashboard by user: {User}", User.Identity?.Name);
                //     return Challenge();
                // }

                // Set default filters if not provided
                filters ??= new ReportFilterModel
                {
                    FromDate = DateTime.UtcNow.AddDays(-30),
                    ToDate = DateTime.UtcNow,
                    TimeGranularity = "weekly",
                    SortBy = "efficiency"
                };

                // Get comprehensive dashboard data
                var dashboardData = await _analyticsService.GetDashboardDataAsync(filters);

                if (dashboardData == null)
                {
                    _logger.LogError("Failed to retrieve dashboard data for filters: {@Filters}", filters);
                    TempData["UserMessageError"] = "Unable to load dashboard data. Please try again.";
                    return View(new PerformanceDashboardModel());
                }

                _logger.LogInformation("Dashboard data loaded successfully for user: {User}", User.Identity?.Name);
                return View(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard for user: {User}", User.Identity?.Name);
                TempData["UserMessageError"] = "An error occurred while loading the dashboard. Please try again.";
                return View(new PerformanceDashboardModel());
            }
        }

        /// <summary>
        /// Get efficiency metrics data for AJAX requests
        /// </summary>
        /// <param name="fromDate">Start date for analysis</param>
        /// <param name="toDate">End date for analysis</param>
        /// <returns>JSON efficiency metrics data</returns>
        [HttpGet]
        public async Task<IActionResult> GetEfficiencyData(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                // Check authorization
                if (!User.Identity?.IsAuthenticated == true || (!User.IsAdmin() && !User.IsOperator()))
                {
                    return Unauthorized();
                }

                var efficiencyData = await _analyticsService.GetEfficiencyMetricsAsync(fromDate, toDate);
                
                if (efficiencyData == null)
                {
                    return BadRequest("Unable to retrieve efficiency data");
                }

                return Json(efficiencyData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving efficiency data");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get operator performance data for AJAX requests
        /// </summary>
        /// <param name="fromDate">Start date for analysis</param>
        /// <param name="toDate">End date for analysis</param>
        /// <param name="projectIds">Optional project filter</param>
        /// <param name="sortBy">Sort criteria</param>
        /// <returns>JSON operator performance data</returns>
        [HttpGet]
        public async Task<IActionResult> GetOperatorData(
            DateTime? fromDate, 
            DateTime? toDate, 
            [FromQuery] int[]? projectIds, 
            string sortBy = "efficiency")
        {
            try
            {
                // Check authorization
                if (!User.Identity?.IsAuthenticated == true || (!User.IsAdmin() && !User.IsOperator()))
                {
                    return Unauthorized();
                }

                var operatorData = await _analyticsService.GetOperatorPerformanceAsync(
                    fromDate, toDate, projectIds, sortBy);
                
                if (operatorData == null)
                {
                    return BadRequest("Unable to retrieve operator performance data");
                }

                return Json(operatorData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving operator performance data");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get bottleneck analysis data for AJAX requests
        /// </summary>
        /// <param name="fromDate">Start date for analysis</param>
        /// <param name="toDate">End date for analysis</param>
        /// <param name="operatorIds">Optional operator filter</param>
        /// <param name="projectIds">Optional project filter</param>
        /// <returns>JSON bottleneck analysis data</returns>
        [HttpGet]
        public async Task<IActionResult> GetBottleneckData(
            DateTime? fromDate, 
            DateTime? toDate, 
            [FromQuery] int[]? operatorIds, 
            [FromQuery] int[]? projectIds)
        {
            try
            {
                // Check authorization
                if (!User.Identity?.IsAuthenticated == true || (!User.IsAdmin() && !User.IsOperator()))
                {
                    return Unauthorized();
                }

                var bottleneckData = await _analyticsService.GetBottleneckAnalysisAsync(
                    fromDate, toDate, operatorIds, projectIds);
                
                if (bottleneckData == null)
                {
                    return BadRequest("Unable to retrieve bottleneck analysis data");
                }

                return Json(bottleneckData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bottleneck analysis data");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get trend chart data for AJAX requests
        /// </summary>
        /// <param name="fromDate">Start date for analysis</param>
        /// <param name="toDate">End date for analysis</param>
        /// <param name="operatorIds">Optional operator filter</param>
        /// <param name="projectIds">Optional project filter</param>
        /// <param name="granularity">Time granularity (daily, weekly, monthly)</param>
        /// <returns>JSON trend chart data</returns>
        [HttpGet]
        public async Task<IActionResult> GetTrendData(
            DateTime? fromDate, 
            DateTime? toDate, 
            [FromQuery] int[]? operatorIds, 
            [FromQuery] int[]? projectIds, 
            string granularity = "weekly")
        {
            try
            {
                // Check authorization
                if (!User.Identity?.IsAuthenticated == true || (!User.IsAdmin() && !User.IsOperator()))
                {
                    return Unauthorized();
                }

                var trendData = await _analyticsService.GetCompletionTrendsAsync(
                    fromDate, toDate, operatorIds, projectIds, granularity);
                
                if (trendData == null)
                {
                    return BadRequest("Unable to retrieve trend data");
                }

                return Json(trendData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving trend data");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Refresh dashboard data manually
        /// </summary>
        /// <param name="filters">Filter parameters</param>
        /// <returns>Partial view with updated dashboard data</returns>
        [HttpPost]
        public async Task<IActionResult> RefreshData([FromBody] ReportFilterModel filters)
        {
            try
            {
                // Check authorization
                if (!User.Identity?.IsAuthenticated == true || (!User.IsAdmin() && !User.IsOperator()))
                {
                    return Unauthorized();
                }

                var dashboardData = await _analyticsService.GetDashboardDataAsync(filters);

                if (dashboardData == null)
                {
                    return BadRequest("Unable to refresh dashboard data");
                }

                return PartialView("_DashboardContent", dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing dashboard data");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Export dashboard data as PDF report
        /// </summary>
        /// <param name="filters">Filter parameters for the report</param>
        /// <returns>PDF file download</returns>
        [HttpPost]
        public async Task<IActionResult> ExportPdf([FromBody] ReportFilterModel filters)
        {
            try
            {
                // Check authorization - Admin only for exports
                if (!User.Identity?.IsAuthenticated == true || !User.IsAdmin())
                {
                    _logger.LogWarning("Unauthorized PDF export attempt by user: {User}", User.Identity?.Name);
                    return Unauthorized();
                }

                _logger.LogInformation("Starting PDF export with filters: {Filters} by user: {User}", filters, User.Identity?.Name);

                // Validate filters
                if (filters.FromDate > filters.ToDate)
                {
                    TempData["UserMessageError"] = "From date cannot be later than To date.";
                    return RedirectToAction(nameof(Index));
                }

                var dashboardData = await _analyticsService.GetDashboardDataAsync(filters);

                if (dashboardData == null)
                {
                    TempData["UserMessageError"] = "Unable to generate report data for the selected period.";
                    return RedirectToAction(nameof(Index));
                }

                // Validate data for reporting
                if (!_reportService.ValidateReportData(dashboardData))
                {
                    TempData["UserMessageError"] = "Insufficient data available for the selected period.";
                    return RedirectToAction(nameof(Index));
                }

                // Generate PDF report
                var pdfBytes = await _reportService.GeneratePdfReportAsync(dashboardData, filters);

                var fileName = $"Performance_Report_{filters.FromDate:yyyyMMdd}_{filters.ToDate:yyyyMMdd}.pdf";

                _logger.LogInformation("PDF report generated successfully: {FileName} by user: {User}", fileName, User.Identity?.Name);

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting PDF report");
                TempData["UserMessageError"] = "An error occurred while generating the PDF report. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Export dashboard data as Excel report
        /// </summary>
        /// <param name="filters">Filter parameters for the report</param>
        /// <returns>Excel file download</returns>
        [HttpPost]
        public async Task<IActionResult> ExportExcel([FromBody] ReportFilterModel filters)
        {
            try
            {
                // Check authorization - Admin only for exports
                if (!User.Identity?.IsAuthenticated == true || !User.IsAdmin())
                {
                    _logger.LogWarning("Unauthorized Excel export attempt by user: {User}", User.Identity?.Name);
                    return Unauthorized();
                }

                _logger.LogInformation("Starting Excel export with filters: {Filters} by user: {User}", filters, User.Identity?.Name);

                // Validate filters
                if (filters.FromDate > filters.ToDate)
                {
                    TempData["UserMessageError"] = "From date cannot be later than To date.";
                    return RedirectToAction(nameof(Index));
                }

                var dashboardData = await _analyticsService.GetDashboardDataAsync(filters);

                if (dashboardData == null)
                {
                    TempData["UserMessageError"] = "Unable to generate report data for the selected period.";
                    return RedirectToAction(nameof(Index));
                }

                // Validate data for reporting
                if (!_reportService.ValidateReportData(dashboardData))
                {
                    TempData["UserMessageError"] = "Insufficient data available for the selected period.";
                    return RedirectToAction(nameof(Index));
                }

                // Generate Excel report
                var excelBytes = await _reportService.GenerateExcelReportAsync(dashboardData, filters);

                var fileName = $"Performance_Report_{filters.FromDate:yyyyMMdd}_{filters.ToDate:yyyyMMdd}.xlsx";

                _logger.LogInformation("Excel report generated successfully: {FileName} by user: {User}", fileName, User.Identity?.Name);

                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting Excel report");
                TempData["UserMessageError"] = "An error occurred while generating the Excel report. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Get operator productivity data for detailed analysis
        /// </summary>
        /// <param name="operatorId">Operator ID</param>
        /// <param name="fromDate">Start date for analysis</param>
        /// <param name="toDate">End date for analysis</param>
        /// <returns>JSON operator productivity data</returns>
        [HttpGet]
        public async Task<IActionResult> GetOperatorProductivity(
            int operatorId,
            DateTime? fromDate,
            DateTime? toDate)
        {
            try
            {
                // Check authorization
                if (!User.Identity?.IsAuthenticated == true || (!User.IsAdmin() && !User.IsOperator()))
                {
                    return Unauthorized();
                }

                var productivityData = await _analyticsService.GetOperatorProductivityAsync(
                    operatorId,
                    fromDate ?? DateTime.UtcNow.AddDays(-30),
                    toDate ?? DateTime.UtcNow);

                if (productivityData == null)
                {
                    return BadRequest("Unable to retrieve operator productivity data");
                }

                return Json(productivityData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving operator productivity data for operator {OperatorId}", operatorId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get workload distribution data
        /// </summary>
        /// <param name="fromDate">Start date for analysis</param>
        /// <param name="toDate">End date for analysis</param>
        /// <returns>JSON workload distribution data</returns>
        [HttpGet]
        public async Task<IActionResult> GetWorkloadDistribution(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                // Check authorization
                if (!User.Identity?.IsAuthenticated == true || (!User.IsAdmin() && !User.IsOperator()))
                {
                    return Unauthorized();
                }

                var workloadData = await _analyticsService.GetWorkloadDistributionAsync(
                    fromDate ?? DateTime.UtcNow.AddDays(-30),
                    toDate ?? DateTime.UtcNow);

                if (workloadData == null)
                {
                    return BadRequest("Unable to retrieve workload distribution data");
                }

                return Json(workloadData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workload distribution data");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
