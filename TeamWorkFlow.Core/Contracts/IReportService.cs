using TeamWorkFlow.Core.Models.Dashboard;

namespace TeamWorkFlow.Core.Contracts
{
    /// <summary>
    /// Service for generating reports from dashboard data
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        /// Generates a PDF report from dashboard data
        /// </summary>
        /// <param name="dashboardData">Dashboard data to include in the report</param>
        /// <param name="filters">Applied filters for the report</param>
        /// <returns>PDF file as byte array</returns>
        Task<byte[]> GeneratePdfReportAsync(PerformanceDashboardModel dashboardData, ReportFilterModel filters);

        /// <summary>
        /// Generates an Excel report from dashboard data
        /// </summary>
        /// <param name="dashboardData">Dashboard data to include in the report</param>
        /// <param name="filters">Applied filters for the report</param>
        /// <returns>Excel file as byte array</returns>
        Task<byte[]> GenerateExcelReportAsync(PerformanceDashboardModel dashboardData, ReportFilterModel filters);

        /// <summary>
        /// Generates a summary report with key metrics
        /// </summary>
        /// <param name="dashboardData">Dashboard data to summarize</param>
        /// <param name="filters">Applied filters for the report</param>
        /// <returns>Summary report as string</returns>
        Task<string> GenerateSummaryReportAsync(PerformanceDashboardModel dashboardData, ReportFilterModel filters);

        /// <summary>
        /// Validates that the dashboard data is suitable for report generation
        /// </summary>
        /// <param name="dashboardData">Dashboard data to validate</param>
        /// <returns>True if data is valid for reporting</returns>
        bool ValidateReportData(PerformanceDashboardModel dashboardData);
    }
}
