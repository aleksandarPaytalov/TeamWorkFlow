using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamWorkFlow.Core.Models.Dashboard;

namespace TeamWorkFlow.Core.Contracts
{
    /// <summary>
    /// Service interface for calculating performance metrics and analytics for the dashboard
    /// </summary>
    public interface ITaskAnalyticsService
    {
        /// <summary>
        /// Calculates efficiency metrics including on-time completion rates and time overrun analysis
        /// </summary>
        /// <param name="fromDate">Start date for analysis period (optional)</param>
        /// <param name="toDate">End date for analysis period (optional)</param>
        /// <param name="operatorIds">Filter by specific operators (optional)</param>
        /// <param name="projectIds">Filter by specific projects (optional)</param>
        /// <returns>Efficiency metrics model with completion rates and trend data</returns>
        Task<EfficiencyMetricsModel> GetEfficiencyMetricsAsync(
            DateTime? fromDate = null, 
            DateTime? toDate = null, 
            IEnumerable<int>? operatorIds = null, 
            IEnumerable<int>? projectIds = null);

        /// <summary>
        /// Retrieves performance data for all operators with comparative analysis
        /// </summary>
        /// <param name="fromDate">Start date for analysis period (optional)</param>
        /// <param name="toDate">End date for analysis period (optional)</param>
        /// <param name="projectIds">Filter by specific projects (optional)</param>
        /// <param name="sortBy">Sort criteria for operator ranking (default: efficiency)</param>
        /// <returns>List of operator performance models with comparative metrics</returns>
        Task<List<OperatorPerformanceModel>> GetOperatorPerformanceAsync(
            DateTime? fromDate = null, 
            DateTime? toDate = null, 
            IEnumerable<int>? projectIds = null,
            string sortBy = "efficiency");

        /// <summary>
        /// Analyzes tasks and processes to identify bottlenecks and delay patterns
        /// </summary>
        /// <param name="fromDate">Start date for analysis period (optional)</param>
        /// <param name="toDate">End date for analysis period (optional)</param>
        /// <param name="operatorIds">Filter by specific operators (optional)</param>
        /// <param name="projectIds">Filter by specific projects (optional)</param>
        /// <returns>Bottleneck analysis model with delay patterns and recommendations</returns>
        Task<BottleneckAnalysisModel> GetBottleneckAnalysisAsync(
            DateTime? fromDate = null, 
            DateTime? toDate = null, 
            IEnumerable<int>? operatorIds = null, 
            IEnumerable<int>? projectIds = null);

        /// <summary>
        /// Generates time-series data for completion trends and workload analysis
        /// </summary>
        /// <param name="fromDate">Start date for trend analysis (optional)</param>
        /// <param name="toDate">End date for trend analysis (optional)</param>
        /// <param name="operatorIds">Filter by specific operators (optional)</param>
        /// <param name="projectIds">Filter by specific projects (optional)</param>
        /// <param name="granularity">Time granularity for trend data (daily, weekly, monthly)</param>
        /// <returns>Trend chart model with time-series data for visualization</returns>
        Task<TrendChartModel> GetCompletionTrendsAsync(
            DateTime? fromDate = null, 
            DateTime? toDate = null, 
            IEnumerable<int>? operatorIds = null, 
            IEnumerable<int>? projectIds = null,
            string granularity = "weekly");

        /// <summary>
        /// Retrieves comprehensive dashboard data combining all analytics
        /// </summary>
        /// <param name="filters">Report filter criteria for dashboard customization</param>
        /// <returns>Complete performance dashboard model with all metrics</returns>
        Task<PerformanceDashboardModel> GetDashboardDataAsync(ReportFilterModel filters);

        /// <summary>
        /// Calculates task completion rate for a specific time period
        /// </summary>
        /// <param name="fromDate">Start date for calculation</param>
        /// <param name="toDate">End date for calculation</param>
        /// <param name="operatorIds">Filter by specific operators (optional)</param>
        /// <param name="projectIds">Filter by specific projects (optional)</param>
        /// <returns>Completion rate as percentage (0-100)</returns>
        Task<decimal> GetTaskCompletionRateAsync(
            DateTime fromDate, 
            DateTime toDate, 
            IEnumerable<int>? operatorIds = null, 
            IEnumerable<int>? projectIds = null);

        /// <summary>
        /// Calculates average time overrun percentage for completed tasks
        /// </summary>
        /// <param name="fromDate">Start date for calculation</param>
        /// <param name="toDate">End date for calculation</param>
        /// <param name="operatorIds">Filter by specific operators (optional)</param>
        /// <param name="projectIds">Filter by specific projects (optional)</param>
        /// <returns>Average overrun percentage (positive = over estimate, negative = under estimate)</returns>
        Task<decimal> GetAverageTimeOverrunAsync(
            DateTime fromDate, 
            DateTime toDate, 
            IEnumerable<int>? operatorIds = null, 
            IEnumerable<int>? projectIds = null);

        /// <summary>
        /// Identifies tasks that are frequently delayed or over estimate
        /// </summary>
        /// <param name="fromDate">Start date for analysis</param>
        /// <param name="toDate">End date for analysis</param>
        /// <param name="minOccurrences">Minimum number of occurrences to be considered frequent (default: 3)</param>
        /// <returns>List of task patterns that frequently cause delays</returns>
        Task<List<DelayPatternModel>> GetFrequentDelayPatternsAsync(
            DateTime fromDate, 
            DateTime toDate, 
            int minOccurrences = 3);

        /// <summary>
        /// Calculates productivity metrics for individual operators
        /// </summary>
        /// <param name="operatorId">Operator identifier</param>
        /// <param name="fromDate">Start date for analysis</param>
        /// <param name="toDate">End date for analysis</param>
        /// <returns>Detailed productivity metrics for the operator</returns>
        Task<OperatorProductivityModel> GetOperatorProductivityAsync(
            int operatorId, 
            DateTime fromDate, 
            DateTime toDate);

        /// <summary>
        /// Generates workload distribution analysis across operators and time periods
        /// </summary>
        /// <param name="fromDate">Start date for analysis</param>
        /// <param name="toDate">End date for analysis</param>
        /// <param name="projectIds">Filter by specific projects (optional)</param>
        /// <returns>Workload distribution model with capacity analysis</returns>
        Task<WorkloadDistributionModel> GetWorkloadDistributionAsync(
            DateTime fromDate, 
            DateTime toDate, 
            IEnumerable<int>? projectIds = null);

        /// <summary>
        /// Validates filter criteria and returns corrected/default values if needed
        /// </summary>
        /// <param name="filters">Filter model to validate</param>
        /// <returns>Validated and corrected filter model</returns>
        Task<ReportFilterModel> ValidateAndCorrectFiltersAsync(ReportFilterModel filters);
    }
}
