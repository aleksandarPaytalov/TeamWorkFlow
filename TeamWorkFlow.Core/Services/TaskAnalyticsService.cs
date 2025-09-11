using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Dashboard;
using TeamWorkFlow.Infrastructure.Data;
using TeamWorkFlow.Infrastructure.Data.Models;

namespace TeamWorkFlow.Core.Services
{
    /// <summary>
    /// Service implementation for calculating performance metrics and analytics for the dashboard
    /// </summary>
    public class TaskAnalyticsService : ITaskAnalyticsService
    {
        private readonly TeamWorkFlowDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<TaskAnalyticsService> _logger;

        // Cache keys
        private const string EFFICIENCY_METRICS_CACHE_KEY = "efficiency_metrics_{0}_{1}_{2}_{3}";
        private const string OPERATOR_PERFORMANCE_CACHE_KEY = "operator_performance_{0}_{1}_{2}_{3}";
        private const string BOTTLENECK_ANALYSIS_CACHE_KEY = "bottleneck_analysis_{0}_{1}_{2}_{3}";
        private const string TREND_CHART_CACHE_KEY = "trend_chart_{0}_{1}_{2}_{3}_{4}";

        // Cache duration
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(15);

        public TaskAnalyticsService(
            TeamWorkFlowDbContext context,
            IMemoryCache cache,
            ILogger<TaskAnalyticsService> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Calculates efficiency metrics including on-time completion rates and time overrun analysis
        /// </summary>
        public async Task<EfficiencyMetricsModel> GetEfficiencyMetricsAsync(
            DateTime? fromDate = null,
            DateTime? toDate = null,
            IEnumerable<int>? operatorIds = null,
            IEnumerable<int>? projectIds = null)
        {
            try
            {
                var cacheKey = string.Format(EFFICIENCY_METRICS_CACHE_KEY,
                    fromDate?.ToString("yyyyMMdd") ?? "null",
                    toDate?.ToString("yyyyMMdd") ?? "null",
                    operatorIds != null ? string.Join(",", operatorIds) : "null",
                    projectIds != null ? string.Join(",", projectIds) : "null");

                if (_cache.TryGetValue(cacheKey, out EfficiencyMetricsModel? cachedResult))
                {
                    return cachedResult!;
                }

                // Set default date range if not provided (last 30 days)
                fromDate ??= DateTime.UtcNow.AddDays(-30);
                toDate ??= DateTime.UtcNow;

                // Get completed tasks with time tracking data
                var completedTasksQuery = _context.Tasks
                    .Include(t => t.TasksOperators)
                    .ThenInclude(to => to.Operator)
                    .Include(t => t.Project)
                    .Where(t => t.TaskStatusId == 3) // Finished status
                    .Where(t => t.EndDate.HasValue && t.EndDate >= fromDate && t.EndDate <= toDate);

                // Apply operator filter
                if (operatorIds != null && operatorIds.Any())
                {
                    completedTasksQuery = completedTasksQuery
                        .Where(t => t.TasksOperators.Any(to => operatorIds.Contains(to.OperatorId)));
                }

                // Apply project filter
                if (projectIds != null && projectIds.Any())
                {
                    completedTasksQuery = completedTasksQuery
                        .Where(t => projectIds.Contains(t.ProjectId));
                }

                var completedTasks = await completedTasksQuery.ToListAsync();

                var result = await CalculateEfficiencyMetrics(completedTasks, fromDate.Value, toDate.Value);

                // Cache the result
                _cache.Set(cacheKey, result, _cacheExpiration);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating efficiency metrics");
                return new EfficiencyMetricsModel();
            }
        }

        /// <summary>
        /// Retrieves performance data for all operators with comparative analysis
        /// </summary>
        public async Task<List<OperatorPerformanceModel>> GetOperatorPerformanceAsync(
            DateTime? fromDate = null,
            DateTime? toDate = null,
            IEnumerable<int>? projectIds = null,
            string sortBy = "efficiency")
        {
            try
            {
                var cacheKey = string.Format(OPERATOR_PERFORMANCE_CACHE_KEY,
                    fromDate?.ToString("yyyyMMdd") ?? "null",
                    toDate?.ToString("yyyyMMdd") ?? "null",
                    projectIds != null ? string.Join(",", projectIds) : "null",
                    sortBy);

                if (_cache.TryGetValue(cacheKey, out List<OperatorPerformanceModel>? cachedResult))
                {
                    return cachedResult!;
                }

                // Set default date range if not provided (last 30 days)
                fromDate ??= DateTime.UtcNow.AddDays(-30);
                toDate ??= DateTime.UtcNow;

                var result = await CalculateOperatorPerformance(fromDate.Value, toDate.Value, projectIds, sortBy);

                // Cache the result
                _cache.Set(cacheKey, result, _cacheExpiration);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating operator performance");
                return new List<OperatorPerformanceModel>();
            }
        }

        /// <summary>
        /// Analyzes tasks and processes to identify bottlenecks and delay patterns
        /// </summary>
        public async Task<BottleneckAnalysisModel> GetBottleneckAnalysisAsync(
            DateTime? fromDate = null,
            DateTime? toDate = null,
            IEnumerable<int>? operatorIds = null,
            IEnumerable<int>? projectIds = null)
        {
            try
            {
                var cacheKey = string.Format(BOTTLENECK_ANALYSIS_CACHE_KEY,
                    fromDate?.ToString("yyyyMMdd") ?? "null",
                    toDate?.ToString("yyyyMMdd") ?? "null",
                    operatorIds != null ? string.Join(",", operatorIds) : "null",
                    projectIds != null ? string.Join(",", projectIds) : "null");

                if (_cache.TryGetValue(cacheKey, out BottleneckAnalysisModel? cachedResult))
                {
                    return cachedResult!;
                }

                // Set default date range if not provided (last 30 days)
                fromDate ??= DateTime.UtcNow.AddDays(-30);
                toDate ??= DateTime.UtcNow;

                var result = await CalculateBottleneckAnalysis(fromDate.Value, toDate.Value, operatorIds, projectIds);

                // Cache the result
                _cache.Set(cacheKey, result, _cacheExpiration);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating bottleneck analysis");
                return new BottleneckAnalysisModel();
            }
        }

        /// <summary>
        /// Generates time-series data for completion trends and workload analysis
        /// </summary>
        public async Task<TrendChartModel> GetCompletionTrendsAsync(
            DateTime? fromDate = null,
            DateTime? toDate = null,
            IEnumerable<int>? operatorIds = null,
            IEnumerable<int>? projectIds = null,
            string granularity = "weekly")
        {
            try
            {
                var cacheKey = string.Format(TREND_CHART_CACHE_KEY,
                    fromDate?.ToString("yyyyMMdd") ?? "null",
                    toDate?.ToString("yyyyMMdd") ?? "null",
                    operatorIds != null ? string.Join(",", operatorIds) : "null",
                    projectIds != null ? string.Join(",", projectIds) : "null",
                    granularity);

                if (_cache.TryGetValue(cacheKey, out TrendChartModel? cachedResult))
                {
                    return cachedResult!;
                }

                // Set default date range if not provided (last 90 days for trends)
                fromDate ??= DateTime.UtcNow.AddDays(-90);
                toDate ??= DateTime.UtcNow;

                var result = await CalculateCompletionTrends(fromDate.Value, toDate.Value, operatorIds, projectIds, granularity);

                // Cache the result
                _cache.Set(cacheKey, result, _cacheExpiration);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating completion trends");
                return new TrendChartModel();
            }
        }

        /// <summary>
        /// Retrieves comprehensive dashboard data combining all analytics
        /// </summary>
        public async Task<PerformanceDashboardModel> GetDashboardDataAsync(ReportFilterModel filters)
        {
            try
            {
                var validatedFilters = await ValidateAndCorrectFiltersAsync(filters);

                var dashboard = new PerformanceDashboardModel
                {
                    AppliedFilters = validatedFilters,
                    LastUpdated = DateTime.UtcNow
                };

                // Calculate KPI summary first (to avoid DbContext concurrency issues)
                dashboard.KpiSummary = await CalculateKpiSummary(validatedFilters);

                // Get all analytics data sequentially to avoid DbContext concurrency issues
                try
                {
                    dashboard.EfficiencyMetrics = await GetEfficiencyMetricsAsync(
                        validatedFilters.FromDate,
                        validatedFilters.ToDate,
                        validatedFilters.SelectedOperatorIds,
                        validatedFilters.SelectedProjectIds);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error calculating efficiency metrics");
                    dashboard.EfficiencyMetrics = new EfficiencyMetricsModel();
                }

                try
                {
                    dashboard.OperatorPerformance = await GetOperatorPerformanceAsync(
                        validatedFilters.FromDate,
                        validatedFilters.ToDate,
                        validatedFilters.SelectedProjectIds);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error calculating operator performance");
                    dashboard.OperatorPerformance = new List<OperatorPerformanceModel>();
                }

                try
                {
                    dashboard.BottleneckAnalysis = await GetBottleneckAnalysisAsync(
                        validatedFilters.FromDate,
                        validatedFilters.ToDate,
                        validatedFilters.SelectedOperatorIds,
                        validatedFilters.SelectedProjectIds);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error calculating bottleneck analysis");
                    dashboard.BottleneckAnalysis = new BottleneckAnalysisModel();
                }

                try
                {
                    dashboard.TrendCharts = await GetCompletionTrendsAsync(
                        validatedFilters.FromDate,
                        validatedFilters.ToDate,
                        validatedFilters.SelectedOperatorIds,
                        validatedFilters.SelectedProjectIds,
                        validatedFilters.TimeGranularity);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error calculating trend charts");
                    dashboard.TrendCharts = new TrendChartModel();
                }

                // Calculate summary data
                dashboard.TotalTasksAnalyzed = dashboard.EfficiencyMetrics?.TotalTasksCompleted ?? 0;
                dashboard.TotalOperatorsAnalyzed = dashboard.OperatorPerformance?.Count ?? 0;
                dashboard.AnalysisPeriod = $"{validatedFilters.FromDate:dd/MM/yyyy} - {validatedFilters.ToDate:dd/MM/yyyy}";

                return dashboard;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating dashboard data");

                // Return a dashboard with at least the KPI summary if possible
                try
                {
                    var validatedFilters = await ValidateAndCorrectFiltersAsync(filters);
                    var fallbackDashboard = new PerformanceDashboardModel
                    {
                        AppliedFilters = validatedFilters,
                        LastUpdated = DateTime.UtcNow,
                        KpiSummary = await CalculateKpiSummary(validatedFilters)
                    };
                    return fallbackDashboard;
                }
                catch
                {
                    return new PerformanceDashboardModel();
                }
            }
        }

        /// <summary>
        /// Calculates task completion rate for a specific time period
        /// </summary>
        public async Task<decimal> GetTaskCompletionRateAsync(
            DateTime fromDate,
            DateTime toDate,
            IEnumerable<int>? operatorIds = null,
            IEnumerable<int>? projectIds = null)
        {
            try
            {
                var tasksQuery = _context.Tasks.AsQueryable();

                // Apply date filter
                tasksQuery = tasksQuery.Where(t => t.StartDate >= fromDate && t.StartDate <= toDate);

                // Apply operator filter
                if (operatorIds != null && operatorIds.Any())
                {
                    tasksQuery = tasksQuery.Where(t => t.TasksOperators.Any(to => operatorIds.Contains(to.OperatorId)));
                }

                // Apply project filter
                if (projectIds != null && projectIds.Any())
                {
                    tasksQuery = tasksQuery.Where(t => projectIds.Contains(t.ProjectId));
                }

                var totalTasks = await tasksQuery.CountAsync();
                var completedTasks = await tasksQuery.Where(t => t.TaskStatusId == 3).CountAsync();

                return totalTasks > 0 ? (decimal)completedTasks / totalTasks * 100 : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating task completion rate");
                return 0;
            }
        }

        /// <summary>
        /// Calculates average overtime hours per task for completed tasks
        /// </summary>
        public async Task<decimal> GetAverageTimeOverrunAsync(
            DateTime fromDate,
            DateTime toDate,
            IEnumerable<int>? operatorIds = null,
            IEnumerable<int>? projectIds = null)
        {
            try
            {
                var completedTasksQuery = _context.Tasks
                    .Where(t => t.TaskStatusId == 3) // Finished status
                    .Where(t => t.EndDate.HasValue && t.EndDate >= fromDate && t.EndDate <= toDate)
                    .Where(t => t.ActualTime.HasValue && t.EstimatedTime > 0);

                // Apply operator filter
                if (operatorIds != null && operatorIds.Any())
                {
                    completedTasksQuery = completedTasksQuery
                        .Where(t => t.TasksOperators.Any(to => operatorIds.Contains(to.OperatorId)));
                }

                // Apply project filter
                if (projectIds != null && projectIds.Any())
                {
                    completedTasksQuery = completedTasksQuery
                        .Where(t => projectIds.Contains(t.ProjectId));
                }

                var tasks = await completedTasksQuery.ToListAsync();

                if (!tasks.Any())
                    return 0;

                // Calculate total overtime hours (only for tasks that went over estimate)
                var totalOvertimeHours = tasks
                    .Where(t => t.ActualTime > t.EstimatedTime) // Only tasks that went over
                    .Sum(t => (decimal)t.ActualTime!.Value - t.EstimatedTime);

                // Return average overtime hours per task (total overtime / total number of tasks)
                return totalOvertimeHours / tasks.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating average time overrun");
                return 0;
            }
        }

        /// <summary>
        /// Identifies tasks that are frequently delayed or over estimate
        /// </summary>
        public async Task<List<DelayPatternModel>> GetFrequentDelayPatternsAsync(
            DateTime fromDate,
            DateTime toDate,
            int minOccurrences = 3)
        {
            try
            {
                var delayedTasks = await _context.Tasks
                    .Include(t => t.Project)
                    .Where(t => t.TaskStatusId == 3) // Finished status
                    .Where(t => t.EndDate.HasValue && t.EndDate >= fromDate && t.EndDate <= toDate)
                    .Where(t => t.ActualTime.HasValue && t.EstimatedTime > 0)
                    .Where(t => t.ActualTime > t.EstimatedTime) // Over estimate
                    .ToListAsync();

                var patterns = delayedTasks
                    .GroupBy(t => new { t.Project.ProjectName, TaskType = GetTaskTypeFromName(t.Name) })
                    .Where(g => g.Count() >= minOccurrences)
                    .Select(g => new DelayPatternModel
                    {
                        PatternName = $"{g.Key.ProjectName} - {g.Key.TaskType}",
                        Occurrences = g.Count(),
                        AverageDelayMinutes = (int)g.Average(t => (t.ActualTime!.Value - t.EstimatedTime) * 60),
                        TotalTimeLostMinutes = (int)g.Sum(t => (t.ActualTime!.Value - t.EstimatedTime) * 60),
                        AffectedTasks = g.Select(t => t.Name).ToList()
                    })
                    .OrderByDescending(p => p.TotalTimeLostMinutes)
                    .ToList();

                return patterns;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error identifying frequent delay patterns");
                return new List<DelayPatternModel>();
            }
        }

        /// <summary>
        /// Calculates productivity metrics for individual operators
        /// </summary>
        public async Task<OperatorProductivityModel> GetOperatorProductivityAsync(
            int operatorId,
            DateTime fromDate,
            DateTime toDate)
        {
            try
            {
                var operatorData = await _context.Operators
                    .Include(o => o.TasksOperators)
                    .ThenInclude(to => to.Task)
                    .FirstOrDefaultAsync(o => o.Id == operatorId);

                if (operatorData == null)
                    return new OperatorProductivityModel();

                var operatorTasks = operatorData.TasksOperators
                    .Where(to => to.Task.EndDate.HasValue &&
                                to.Task.EndDate >= fromDate &&
                                to.Task.EndDate <= toDate)
                    .Select(to => to.Task)
                    .ToList();

                var timeEntries = await _context.TaskTimeEntries
                    .Where(tte => tte.OperatorId == operatorId)
                    .Where(tte => tte.StartTime >= fromDate && tte.EndTime <= toDate)
                    .ToListAsync();

                return CalculateOperatorProductivity(operatorData, operatorTasks, timeEntries, fromDate, toDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating operator productivity for operator {OperatorId}", operatorId);
                return new OperatorProductivityModel();
            }
        }

        /// <summary>
        /// Generates workload distribution analysis across operators and time periods
        /// </summary>
        public async Task<WorkloadDistributionModel> GetWorkloadDistributionAsync(
            DateTime fromDate,
            DateTime toDate,
            IEnumerable<int>? projectIds = null)
        {
            try
            {
                var workloadData = await CalculateWorkloadDistribution(fromDate, toDate, projectIds);
                return workloadData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating workload distribution");
                return new WorkloadDistributionModel();
            }
        }

        /// <summary>
        /// Validates filter criteria and returns corrected/default values if needed
        /// </summary>
        public async Task<ReportFilterModel> ValidateAndCorrectFiltersAsync(ReportFilterModel filters)
        {
            try
            {
                // Determine default date range based on time period if no dates are provided
                DateTime defaultFromDate;
                DateTime defaultToDate = DateTime.UtcNow;

                if (filters.FromDate == null && filters.ToDate == null)
                {
                    // Set default date range based on TimeGranularity
                    switch (filters.TimeGranularity?.ToLower())
                    {
                        case "daily":
                            defaultFromDate = DateTime.UtcNow.AddDays(-1); // Last 1 day
                            break;
                        case "weekly":
                            defaultFromDate = DateTime.UtcNow.AddDays(-7); // Last 1 week
                            break;
                        case "monthly":
                            defaultFromDate = DateTime.UtcNow.AddDays(-30); // Last 1 month
                            break;
                        default:
                            defaultFromDate = DateTime.UtcNow.AddDays(-7); // Default to weekly
                            break;
                    }
                }
                else
                {
                    // Use provided dates or fall back to 30 days if only one is provided
                    defaultFromDate = filters.FromDate ?? DateTime.UtcNow.AddDays(-30);
                    defaultToDate = filters.ToDate ?? DateTime.UtcNow;
                }

                var validatedFilters = new ReportFilterModel
                {
                    FromDate = filters.FromDate ?? defaultFromDate,
                    ToDate = filters.ToDate ?? defaultToDate,
                    SelectedOperatorIds = filters.SelectedOperatorIds ?? new List<int>(),
                    SelectedProjectIds = filters.SelectedProjectIds ?? new List<int>(),
                    SelectedTaskStatuses = filters.SelectedTaskStatuses ?? new List<string>(),
                    TimeGranularity = filters.TimeGranularity ?? "weekly",
                    SortBy = filters.SortBy ?? "efficiency"
                };

                // Ensure FromDate is not after ToDate
                if (validatedFilters.FromDate > validatedFilters.ToDate)
                {
                    var temp = validatedFilters.FromDate;
                    validatedFilters.FromDate = validatedFilters.ToDate;
                    validatedFilters.ToDate = temp;
                }

                // Limit date range to maximum 1 year
                if ((validatedFilters.ToDate!.Value - validatedFilters.FromDate!.Value).TotalDays > 365)
                {
                    validatedFilters.FromDate = validatedFilters.ToDate!.Value.AddDays(-365);
                }

                // Validate operator IDs exist
                if (validatedFilters.SelectedOperatorIds != null && validatedFilters.SelectedOperatorIds.Any())
                {
                    var existingOperatorIds = await _context.Operators
                        .Where(o => validatedFilters.SelectedOperatorIds.Contains(o.Id))
                        .Select(o => o.Id)
                        .ToListAsync();
                    validatedFilters.SelectedOperatorIds = existingOperatorIds;
                }

                // Validate project IDs exist
                if (validatedFilters.SelectedProjectIds != null && validatedFilters.SelectedProjectIds.Any())
                {
                    var existingProjectIds = await _context.Projects
                        .Where(p => validatedFilters.SelectedProjectIds.Contains(p.Id))
                        .Select(p => p.Id)
                        .ToListAsync();
                    validatedFilters.SelectedProjectIds = existingProjectIds;
                }

                return validatedFilters;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating filters");
                return new ReportFilterModel
                {
                    FromDate = DateTime.UtcNow.AddDays(-30),
                    ToDate = DateTime.UtcNow,
                    TimeGranularity = "weekly",
                    SortBy = "efficiency"
                };
            }
        }

        #region Private Helper Methods

        /// <summary>
        /// Calculates efficiency metrics from completed tasks
        /// </summary>
        private async Task<EfficiencyMetricsModel> CalculateEfficiencyMetrics(
            List<Infrastructure.Data.Models.Task> tasks,
            DateTime fromDate,
            DateTime toDate)
        {
            var result = new EfficiencyMetricsModel
            {
                TotalTasksCompleted = tasks.Count,
                TrendPeriod = $"{fromDate:dd/MM/yyyy} - {toDate:dd/MM/yyyy}"
            };

            if (!tasks.Any())
                return result;

            // Calculate on-time completion
            var tasksWithValidData = tasks.Where(t => t.ActualTime.HasValue && t.EstimatedTime > 0).ToList();
            result.TasksCompletedOnTime = tasksWithValidData.Count(t => t.ActualTime <= t.EstimatedTime);
            result.TasksOverEstimate = tasksWithValidData.Count(t => t.ActualTime > t.EstimatedTime);
            result.TasksUnderEstimate = tasksWithValidData.Count(t => t.ActualTime < t.EstimatedTime);

            if (tasksWithValidData.Any())
            {
                result.OnTimeCompletionRate = (decimal)result.TasksCompletedOnTime / tasksWithValidData.Count * 100;
                result.AverageActualTimeHours = (decimal)tasksWithValidData.Average(t => t.ActualTime!.Value);
                result.AverageEstimatedTimeHours = (decimal)tasksWithValidData.Average(t => t.EstimatedTime);

                // Calculate average overtime hours per task (total overtime / total tasks)
                var totalOvertimeHours = tasksWithValidData
                    .Where(t => t.ActualTime > t.EstimatedTime) // Only tasks that went over
                    .Sum(t => (decimal)t.ActualTime!.Value - t.EstimatedTime);

                // Average overtime hours per task (total overtime / total number of tasks)
                result.AverageTimeOverrunPercentage = totalOvertimeHours / tasksWithValidData.Count;

                // Calculate high variance tasks (>20% variance) with safety check for division by zero
                var highVarianceTasks = tasksWithValidData
                    .Where(t => t.EstimatedTime > 0) // Additional safety check to prevent division by zero
                    .Count(t => Math.Abs(((decimal)t.ActualTime!.Value - t.EstimatedTime) / t.EstimatedTime * 100) > 20);

                if (tasksWithValidData.Count > 0)
                {
                    result.HighVarianceTasksPercentage = (decimal)highVarianceTasks / tasksWithValidData.Count * 100;
                }
            }

            // Calculate trend data (compare with previous period)
            var previousPeriodStart = fromDate.AddDays(-(toDate - fromDate).TotalDays);
            var previousPeriodTasks = await _context.Tasks
                .Where(t => t.TaskStatusId == 3)
                .Where(t => t.EndDate.HasValue && t.EndDate >= previousPeriodStart && t.EndDate < fromDate)
                .Where(t => t.ActualTime.HasValue && t.EstimatedTime > 0)
                .ToListAsync();

            if (previousPeriodTasks.Any())
            {
                var previousOnTimeRate = (decimal)previousPeriodTasks.Count(t => t.ActualTime <= t.EstimatedTime) /
                                       previousPeriodTasks.Count * 100;
                result.EfficiencyTrend = result.OnTimeCompletionRate - previousOnTimeRate;
            }

            // Generate trend data points for visualization
            result.TrendData = await GenerateEfficiencyTrendData(fromDate, toDate);

            // Calculate common delay reasons (simplified)
            result.CommonDelayReasons = CalculateCommonDelayReasons(tasksWithValidData);

            return result;
        }

        /// <summary>
        /// Calculates operator performance metrics
        /// </summary>
        private async Task<List<OperatorPerformanceModel>> CalculateOperatorPerformance(
            DateTime fromDate,
            DateTime toDate,
            IEnumerable<int>? projectIds,
            string sortBy)
        {
            var operatorsQuery = _context.Operators
                .Include(o => o.TasksOperators)
                .ThenInclude(to => to.Task)
                .Where(o => o.IsActive);

            var operators = await operatorsQuery.ToListAsync();
            var result = new List<OperatorPerformanceModel>();

            foreach (var op in operators)
            {
                var operatorTasks = op.TasksOperators
                    .Where(to => to.Task.EndDate.HasValue &&
                                to.Task.EndDate >= fromDate &&
                                to.Task.EndDate <= toDate &&
                                to.Task.TaskStatusId == 3) // Finished tasks only
                    .Select(to => to.Task);

                // Apply project filter if specified
                if (projectIds != null && projectIds.Any())
                {
                    operatorTasks = operatorTasks.Where(t => projectIds.Contains(t.ProjectId));
                }

                var tasksList = operatorTasks.ToList();

                if (!tasksList.Any())
                    continue;

                var performance = new OperatorPerformanceModel
                {
                    OperatorId = op.Id,
                    OperatorName = op.FullName,
                    OperatorEmail = op.Email,
                    TasksCompleted = tasksList.Count,
                    ReportingPeriod = $"{fromDate:dd/MM/yyyy} - {toDate:dd/MM/yyyy}"
                };

                // Calculate performance metrics
                var tasksWithTime = tasksList.Where(t => t.ActualTime.HasValue && t.EstimatedTime > 0).ToList();
                if (tasksWithTime.Any())
                {
                    performance.AverageCompletionTimeHours = (decimal)tasksWithTime.Average(t => t.ActualTime!.Value);
                    performance.OnTimeCompletionRate = (decimal)tasksWithTime.Count(t => t.ActualTime <= t.EstimatedTime) /
                                                     tasksWithTime.Count * 100;

                    // Calculate average overtime hours per task (total overtime / total tasks)
                    var totalOvertimeHours = tasksWithTime
                        .Where(t => t.ActualTime > t.EstimatedTime) // Only tasks that went over
                        .Sum(t => (decimal)t.ActualTime!.Value - t.EstimatedTime);

                    // Average overtime hours per task (total overtime / total number of tasks)
                    performance.AverageOverrunPercentage = totalOvertimeHours / tasksWithTime.Count;

                    // Calculate efficiency rating (weighted score)
                    performance.EfficiencyRating = CalculateEfficiencyRating(
                        performance.OnTimeCompletionRate,
                        performance.AverageOverrunPercentage);
                }

                // Get time tracking data for additional metrics
                var timeEntries = await _context.TaskTimeEntries
                    .Where(tte => tte.OperatorId == op.Id)
                    .Where(tte => tte.StartTime >= fromDate && tte.EndTime <= toDate)
                    .ToListAsync();

                if (timeEntries.Any())
                {
                    performance.TotalProductiveHours = (decimal)timeEntries.Sum(te => te.DurationMinutes) / 60;
                    performance.AverageSessionDurationMinutes = (decimal)timeEntries.Average(te => te.DurationMinutes);
                    performance.TotalSessions = timeEntries.Count;
                }

                result.Add(performance);
            }

            // Sort results first
            result = SortOperatorPerformance(result, sortBy);

            // Calculate rankings based on the sorted order
            CalculateOperatorRankings(result, sortBy);

            return result;
        }

        /// <summary>
        /// Calculates bottleneck analysis
        /// </summary>
        private async Task<BottleneckAnalysisModel> CalculateBottleneckAnalysis(
            DateTime fromDate,
            DateTime toDate,
            IEnumerable<int>? operatorIds,
            IEnumerable<int>? projectIds)
        {
            var result = new BottleneckAnalysisModel
            {
                AnalysisPeriod = $"{fromDate:dd/MM/yyyy} - {toDate:dd/MM/yyyy}"
            };

            // Get tasks that are delayed or over estimate
            var delayedTasksQuery = _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.TasksOperators)
                .ThenInclude(to => to.Operator)
                .Where(t => t.EndDate.HasValue && t.EndDate >= fromDate && t.EndDate <= toDate)
                .Where(t => t.ActualTime.HasValue && t.EstimatedTime > 0)
                .Where(t => t.ActualTime > t.EstimatedTime);

            // Apply filters
            if (operatorIds != null && operatorIds.Any())
            {
                delayedTasksQuery = delayedTasksQuery
                    .Where(t => t.TasksOperators.Any(to => operatorIds.Contains(to.OperatorId)));
            }

            if (projectIds != null && projectIds.Any())
            {
                delayedTasksQuery = delayedTasksQuery
                    .Where(t => projectIds.Contains(t.ProjectId));
            }

            var delayedTasks = await delayedTasksQuery.ToListAsync();

            // Get total tasks in the same period for percentage calculation
            var totalTasksQuery = _context.Tasks
                .Where(t => t.EndDate.HasValue && t.EndDate >= fromDate && t.EndDate <= toDate)
                .Where(t => t.ActualTime.HasValue && t.EstimatedTime > 0);

            // Apply same filters to total tasks
            if (operatorIds != null && operatorIds.Any())
            {
                totalTasksQuery = totalTasksQuery
                    .Where(t => t.TasksOperators.Any(to => operatorIds.Contains(to.OperatorId)));
            }

            if (projectIds != null && projectIds.Any())
            {
                totalTasksQuery = totalTasksQuery
                    .Where(t => projectIds.Contains(t.ProjectId));
            }

            var totalTasksCount = await totalTasksQuery.CountAsync();

            // Calculate bottleneck metrics
            result.TotalBottlenecks = delayedTasks.Count;
            result.AverageDelayHours = delayedTasks.Any() ?
                (decimal)delayedTasks.Average(t => t.ActualTime!.Value - t.EstimatedTime) : 0;

            // Calculate tasks affected percentage
            result.TasksAffectedPercentage = totalTasksCount > 0 ?
                (decimal)delayedTasks.Count / totalTasksCount * 100 : 0;

            // Calculate total time lost due to bottlenecks
            result.TotalTimeLostHours = delayedTasks.Any() ?
                (decimal)delayedTasks.Sum(t => t.ActualTime!.Value - t.EstimatedTime) : 0;

            // Calculate severity score (0-100 based on multiple factors)
            result.SeverityScore = CalculateSeverityScore(
                result.TotalBottlenecks,
                result.TasksAffectedPercentage,
                result.AverageDelayHours,
                result.TotalTimeLostHours);

            // Identify frequent bottleneck tasks
            result.FrequentBottleneckTasks = delayedTasks
                .GroupBy(t => new { t.Name, t.Project.ProjectName })
                .Where(g => g.Count() >= 2)
                .Select(g => new BottleneckTaskModel
                {
                    TaskName = g.Key.Name,
                    ProjectName = g.Key.ProjectName,
                    Occurrences = g.Count(),
                    AverageDelayHours = (decimal)g.Average(t => t.ActualTime!.Value - t.EstimatedTime),
                    TotalDelayHours = (decimal)g.Sum(t => t.ActualTime!.Value - t.EstimatedTime)
                })
                .OrderByDescending(bt => bt.TotalDelayHours)
                .Take(10)
                .ToList();

            // Calculate delay by category (project)
            var delaysByCategory = delayedTasks
                .GroupBy(t => t.Project.ProjectName)
                .Select(g => new DelayCategoryModel
                {
                    CategoryName = g.Key,
                    DelayCount = g.Count(),
                    TotalDelayHours = (decimal)g.Sum(t => t.ActualTime!.Value - t.EstimatedTime),
                    AverageDelayHours = (decimal)g.Average(t => t.ActualTime!.Value - t.EstimatedTime)
                })
                .OrderByDescending(dc => dc.TotalDelayHours)
                .ToList();

            // Calculate percentages for each category
            var totalDelays = delaysByCategory.Sum(dc => dc.DelayCount);
            if (totalDelays > 0)
            {
                foreach (var category in delaysByCategory)
                {
                    category.DelayPercentage = (decimal)category.DelayCount / totalDelays * 100;
                }
            }

            result.DelaysByCategory = delaysByCategory;

            // Generate improvement recommendations
            result.ImprovementRecommendations = GenerateImprovementRecommendations(result);

            return result;
        }

        /// <summary>
        /// Calculates severity score based on multiple bottleneck factors
        /// </summary>
        private static decimal CalculateSeverityScore(
            int totalBottlenecks,
            decimal tasksAffectedPercentage,
            decimal averageDelayHours,
            decimal totalTimeLostHours)
        {
            decimal score = 0;

            // Factor 1: Number of bottlenecks (0-30 points)
            // 0 bottlenecks = 0 points, 10+ bottlenecks = 30 points
            var bottleneckScore = Math.Min(totalBottlenecks * 3, 30);
            score += bottleneckScore;

            // Factor 2: Percentage of tasks affected (0-25 points)
            // 0% = 0 points, 100% = 25 points
            var affectedScore = Math.Min(tasksAffectedPercentage * 0.25m, 25);
            score += affectedScore;

            // Factor 3: Average delay severity (0-25 points)
            // 0 hours = 0 points, 8+ hours = 25 points
            var delayScore = Math.Min(averageDelayHours * 3.125m, 25);
            score += delayScore;

            // Factor 4: Total time lost impact (0-20 points)
            // 0 hours = 0 points, 40+ hours = 20 points
            var timeLostScore = Math.Min(totalTimeLostHours * 0.5m, 20);
            score += timeLostScore;

            // Ensure score is within 0-100 range
            return Math.Min(Math.Max(score, 0), 100);
        }

        /// <summary>
        /// Calculates completion trends
        /// </summary>
        private async Task<TrendChartModel> CalculateCompletionTrends(
            DateTime fromDate,
            DateTime toDate,
            IEnumerable<int>? operatorIds,
            IEnumerable<int>? projectIds,
            string granularity)
        {
            var result = new TrendChartModel
            {
                StartDate = fromDate,
                EndDate = toDate,
                Granularity = granularity
            };

            // Generate time periods based on granularity
            var periods = GenerateTimePeriods(fromDate, toDate, granularity);

            foreach (var period in periods)
            {
                var tasksQuery = _context.Tasks
                    .Where(t => t.EndDate.HasValue &&
                               t.EndDate >= period.Start &&
                               t.EndDate < period.End);

                // Apply filters
                if (operatorIds != null && operatorIds.Any())
                {
                    tasksQuery = tasksQuery
                        .Where(t => t.TasksOperators.Any(to => operatorIds.Contains(to.OperatorId)));
                }

                if (projectIds != null && projectIds.Any())
                {
                    tasksQuery = tasksQuery
                        .Where(t => projectIds.Contains(t.ProjectId));
                }

                var periodTasks = await tasksQuery.ToListAsync();
                var completedTasks = periodTasks.Where(t => t.TaskStatusId == 3).ToList();

                var dataPoint = new TrendDataPoint
                {
                    Date = period.Start,
                    Label = period.Label,
                    Value = completedTasks.Count
                };

                // Store additional metadata
                dataPoint.Metadata["TotalTasks"] = periodTasks.Count;
                dataPoint.Metadata["CompletionPercentage"] = periodTasks.Any() ? (decimal)completedTasks.Count / periodTasks.Count * 100 : 0;

                result.CompletionTrendData.Add(dataPoint);
            }

            // Calculate trend direction and confidence
            if (result.CompletionTrendData.Count >= 2)
            {
                var firstHalf = result.CompletionTrendData.Take(result.CompletionTrendData.Count / 2);
                var secondHalf = result.CompletionTrendData.Skip(result.CompletionTrendData.Count / 2);

                var firstHalfAvg = firstHalf.Average(d => d.Value);
                var secondHalfAvg = secondHalf.Average(d => d.Value);

                result.OverallTrendDirection = secondHalfAvg > firstHalfAvg ? "Improving" :
                                      secondHalfAvg < firstHalfAvg ? "Declining" : "Stable";
                result.TrendConfidence = CalculateTrendConfidence(result.CompletionTrendData);
            }

            return result;
        }

        /// <summary>
        /// Calculates KPI summary for dashboard
        /// </summary>
        public async Task<int> GetActiveOperatorsCountAsync()
        {
            try
            {
                return await _context.Operators.CountAsync(o => o.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active operators count");
                return 0;
            }
        }

        private async Task<DashboardKpiSummaryModel> CalculateKpiSummary(ReportFilterModel filters)
        {
            try
            {
                // Get active operators count - this should show current total regardless of date filters
                var activeOperators = await _context.Operators.CountAsync(o => o.IsActive);

                var result = new DashboardKpiSummaryModel
                {
                    TasksCompleted = 0,
                    TasksInProgress = 0,
                    OverdueTasks = 0,
                    AverageCompletionTimeHours = 0,
                    TotalProductiveHours = 0,
                    ActiveOperators = activeOperators
                };

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating KPI summary");
                return new DashboardKpiSummaryModel();
            }
        }

        /// <summary>
        /// Calculates operator productivity metrics
        /// </summary>
        private OperatorProductivityModel CalculateOperatorProductivity(
            Operator operatorData,
            List<Infrastructure.Data.Models.Task> tasks,
            List<TaskTimeEntry> timeEntries,
            DateTime fromDate,
            DateTime toDate)
        {
            var result = new OperatorProductivityModel
            {
                OperatorId = operatorData.Id,
                OperatorName = operatorData.FullName,
                OperatorEmail = operatorData.Email,
                PeriodStart = fromDate,
                PeriodEnd = toDate
            };

            // Calculate task-based metrics
            var completedTasks = tasks.Where(t => t.TaskStatusId == 3).ToList();
            result.TasksCompleted = completedTasks.Count;
            result.TasksInProgress = tasks.Count(t => t.TaskStatusId == 2);

            if (completedTasks.Any())
            {
                var tasksWithTime = completedTasks.Where(t => t.ActualTime.HasValue && t.EstimatedTime > 0).ToList();
                if (tasksWithTime.Any())
                {
                    result.AverageHoursPerTask = (decimal)tasksWithTime.Average(t => t.ActualTime!.Value);
                    result.OnTimeDeliveryRate = (decimal)tasksWithTime.Count(t => t.ActualTime <= t.EstimatedTime) /
                                                tasksWithTime.Count * 100;

                    // Calculate average overtime hours per task (total overtime / total tasks)
                    var totalOvertimeHours = tasksWithTime
                        .Where(t => t.ActualTime > t.EstimatedTime) // Only tasks that went over
                        .Sum(t => (decimal)t.ActualTime!.Value - t.EstimatedTime);

                    // Average overtime hours per task (total overtime / total number of tasks)
                    result.AverageTimeVariance = totalOvertimeHours / tasksWithTime.Count;
                }
            }

            // Calculate time tracking metrics
            if (timeEntries.Any())
            {
                result.TotalProductiveHours = (decimal)timeEntries.Sum(te => te.DurationMinutes) / 60;
                result.TotalWorkSessions = timeEntries.Count;
                result.AverageSessionDurationMinutes = (decimal)timeEntries.Average(te => te.DurationMinutes);

                // Calculate efficiency rating based on productivity
                var workingDays = (toDate - fromDate).TotalDays;
                if (workingDays > 0)
                {
                    result.TasksPerDay = result.TasksCompleted / (decimal)workingDays;
                }
            }

            return result;
        }

        /// <summary>
        /// Calculates workload distribution
        /// </summary>
        private async Task<WorkloadDistributionModel> CalculateWorkloadDistribution(
            DateTime fromDate,
            DateTime toDate,
            IEnumerable<int>? projectIds)
        {
            var result = new WorkloadDistributionModel
            {
                PeriodStart = fromDate,
                PeriodEnd = toDate
            };

            var operators = await _context.Operators
                .Include(o => o.TasksOperators)
                .ThenInclude(to => to.Task)
                .Where(o => o.IsActive)
                .ToListAsync();

            foreach (var op in operators)
            {
                var operatorTasks = op.TasksOperators
                    .Where(to => to.Task.StartDate >= fromDate && to.Task.StartDate <= toDate)
                    .Select(to => to.Task);

                if (projectIds != null && projectIds.Any())
                {
                    operatorTasks = operatorTasks.Where(t => projectIds.Contains(t.ProjectId));
                }

                var tasksList = operatorTasks.ToList();
                var timeEntries = await _context.TaskTimeEntries
                    .Where(tte => tte.OperatorId == op.Id)
                    .Where(tte => tte.StartTime >= fromDate && tte.EndTime <= toDate)
                    .ToListAsync();

                var workload = new OperatorWorkloadModel
                {
                    OperatorId = op.Id,
                    OperatorName = op.FullName,
                    TasksAssigned = tasksList.Count,
                    TasksCompleted = tasksList.Count(t => t.TaskStatusId == 3),
                    TasksInProgress = tasksList.Count(t => t.TaskStatusId == 2),
                    HoursWorked = (decimal)timeEntries.Sum(te => te.DurationMinutes) / 60
                };

                var workingDays = (toDate - fromDate).TotalDays;
                var expectedHours = (decimal)(workingDays * op.Capacity);
                workload.WorkloadPercentage = expectedHours > 0 ? (workload.HoursWorked / expectedHours) * 100 : 0;

                result.OperatorWorkloads.Add(workload);
            }

            // Calculate balance metrics
            if (result.OperatorWorkloads.Any())
            {
                result.BalanceMetrics = new WorkloadBalanceMetricsModel
                {
                    AverageWorkloadPercentage = result.OperatorWorkloads.Average(w => w.WorkloadPercentage),
                    WorkloadStandardDeviation = CalculateStandardDeviation(result.OperatorWorkloads.Select(w => w.WorkloadPercentage)),
                    OverloadedOperators = result.OperatorWorkloads.Count(w => w.WorkloadPercentage > 100),
                    UnderloadedOperators = result.OperatorWorkloads.Count(w => w.WorkloadPercentage < 70)
                };
            }

            return result;
        }

        #endregion

        #region Utility Helper Methods

        /// <summary>
        /// Generates efficiency trend data points
        /// </summary>
        private async Task<List<EfficiencyTrendPoint>> GenerateEfficiencyTrendData(DateTime fromDate, DateTime toDate)
        {
            var trendData = new List<EfficiencyTrendPoint>();
            var totalDays = (toDate - fromDate).TotalDays;
            var intervalDays = Math.Max(1, totalDays / 10); // Create ~10 data points

            for (var date = fromDate; date < toDate; date = date.AddDays(intervalDays))
            {
                var periodEnd = date.AddDays(intervalDays);
                if (periodEnd > toDate) periodEnd = toDate;

                var periodTasks = await _context.Tasks
                    .Where(t => t.TaskStatusId == 3)
                    .Where(t => t.EndDate.HasValue && t.EndDate >= date && t.EndDate < periodEnd)
                    .Where(t => t.ActualTime.HasValue && t.EstimatedTime > 0)
                    .ToListAsync();

                if (periodTasks.Any())
                {
                    var onTimeCount = periodTasks.Count(t => t.ActualTime <= t.EstimatedTime);
                    var onTimeRate = (decimal)onTimeCount / periodTasks.Count * 100;

                    var overrunPercentages = periodTasks
                        .Where(t => t.EstimatedTime > 0) // Safety check to prevent division by zero
                        .Select(t => ((decimal)t.ActualTime!.Value - t.EstimatedTime) / t.EstimatedTime * 100);
                    var avgOverrun = overrunPercentages.Any() ? overrunPercentages.Average() : 0;

                    trendData.Add(new EfficiencyTrendPoint
                    {
                        Date = date,
                        OnTimeRate = onTimeRate,
                        OverrunPercentage = avgOverrun,
                        TasksCompleted = periodTasks.Count,
                        PeriodLabel = $"{date:dd/MM}"
                    });
                }
            }

            return trendData;
        }

        /// <summary>
        /// Calculates common delay reasons
        /// </summary>
        private static List<DelayReasonModel> CalculateCommonDelayReasons(List<Infrastructure.Data.Models.Task> tasks)
        {
            var delayedTasks = tasks.Where(t => t.ActualTime > t.EstimatedTime).ToList();

            if (!delayedTasks.Any())
                return new List<DelayReasonModel>();

            // Group by project and calculate delay patterns
            var delayReasons = delayedTasks
                .GroupBy(t => GetTaskTypeFromName(t.Name))
                .Select(g => new DelayReasonModel
                {
                    Reason = g.Key,
                    Occurrences = g.Count(),
                    Percentage = (decimal)g.Count() / delayedTasks.Count * 100,
                    AverageDelayMinutes = (int)g.Average(t => (t.ActualTime!.Value - t.EstimatedTime) * 60)
                })
                .OrderByDescending(dr => dr.Occurrences)
                .Take(5)
                .ToList();

            return delayReasons;
        }

        /// <summary>
        /// Extracts task type from task name for pattern analysis
        /// </summary>
        private static string GetTaskTypeFromName(string taskName)
        {
            // Simple pattern matching - can be enhanced based on naming conventions
            if (taskName.ToLower().Contains("test")) return "Testing";
            if (taskName.ToLower().Contains("review")) return "Review";
            if (taskName.ToLower().Contains("design")) return "Design";
            if (taskName.ToLower().Contains("development") || taskName.ToLower().Contains("dev")) return "Development";
            if (taskName.ToLower().Contains("documentation") || taskName.ToLower().Contains("doc")) return "Documentation";
            if (taskName.ToLower().Contains("analysis")) return "Analysis";
            if (taskName.ToLower().Contains("planning")) return "Planning";

            return "General";
        }

        /// <summary>
        /// Calculates efficiency rating for operators
        /// </summary>
        private static decimal CalculateEfficiencyRating(decimal onTimeRate, decimal avgOverrun)
        {
            // Weighted scoring: 70% on-time rate, 30% time accuracy
            var onTimeScore = onTimeRate;
            var accuracyScore = Math.Max(0, 100 - Math.Abs(avgOverrun));

            return (onTimeScore * 0.7m) + (accuracyScore * 0.3m);
        }

        /// <summary>
        /// Calculates operator rankings based on current sort order
        /// </summary>
        private static void CalculateOperatorRankings(List<OperatorPerformanceModel> operators, string sortBy)
        {
            // Assign ranks based on current order (operators list is already sorted)
            for (int i = 0; i < operators.Count; i++)
            {
                operators[i].Rank = i + 1;

                // Calculate relative performance (always based on efficiency for comparison)
                if (operators.Count > 1)
                {
                    var avgEfficiency = operators.Average(o => o.EfficiencyRating);
                    operators[i].RelativeToTeamAverage = operators[i].EfficiencyRating - avgEfficiency;
                }
            }
        }

        /// <summary>
        /// Sorts operator performance based on criteria
        /// </summary>
        private static List<OperatorPerformanceModel> SortOperatorPerformance(List<OperatorPerformanceModel> operators, string sortBy)
        {
            return sortBy.ToLower() switch
            {
                "efficiency" => operators.OrderByDescending(o => o.EfficiencyRating)
                                        .ThenByDescending(o => o.TasksCompleted)
                                        .ThenBy(o => o.OperatorName).ToList(),
                "tasks" => operators.OrderByDescending(o => o.TasksCompleted)
                                   .ThenByDescending(o => o.EfficiencyRating)
                                   .ThenBy(o => o.OperatorName).ToList(),
                "hours" => operators.OrderByDescending(o => o.TotalProductiveHours)
                                   .ThenByDescending(o => o.EfficiencyRating)
                                   .ThenBy(o => o.OperatorName).ToList(),
                "ontime" => operators.OrderByDescending(o => o.OnTimeCompletionRate)
                                    .ThenByDescending(o => o.EfficiencyRating)
                                    .ThenBy(o => o.OperatorName).ToList(),
                "name" => operators.OrderBy(o => o.OperatorName)
                                  .ThenByDescending(o => o.EfficiencyRating).ToList(),
                _ => operators.OrderByDescending(o => o.EfficiencyRating)
                             .ThenByDescending(o => o.TasksCompleted)
                             .ThenBy(o => o.OperatorName).ToList()
            };
        }

        /// <summary>
        /// Generates improvement recommendations
        /// </summary>
        private static List<string> GenerateImprovementRecommendations(BottleneckAnalysisModel analysis)
        {
            var recommendations = new List<string>();

            if (analysis.TotalBottlenecks > 0)
            {
                if (analysis.AverageDelayHours > 8)
                {
                    recommendations.Add("Consider reviewing task estimation processes - average delays exceed 8 hours");
                }

                if (analysis.FrequentBottleneckTasks.Any())
                {
                    var topBottleneck = analysis.FrequentBottleneckTasks.First();
                    recommendations.Add($"Focus on '{topBottleneck.TaskName}' tasks - they account for {topBottleneck.TotalDelayHours:F1} hours of delays");
                }

                if (analysis.DelaysByCategory.Any())
                {
                    var topCategory = analysis.DelaysByCategory.First();
                    recommendations.Add($"Review processes in '{topCategory.CategoryName}' project - highest delay category");
                }

                if (analysis.TotalBottlenecks > 10)
                {
                    recommendations.Add("Consider implementing more frequent progress reviews to catch delays early");
                }
            }
            else
            {
                recommendations.Add("Excellent performance! No significant bottlenecks detected in this period");
            }

            return recommendations;
        }

        /// <summary>
        /// Generates time periods for trend analysis
        /// </summary>
        private static List<(DateTime Start, DateTime End, string Label)> GenerateTimePeriods(DateTime fromDate, DateTime toDate, string granularity)
        {
            var periods = new List<(DateTime Start, DateTime End, string Label)>();

            switch (granularity.ToLower())
            {
                case "daily":
                    for (var date = fromDate.Date; date < toDate; date = date.AddDays(1))
                    {
                        periods.Add((date, date.AddDays(1), date.ToString("dd/MM")));
                    }
                    break;

                case "weekly":
                    var weekStart = fromDate.Date.AddDays(-(int)fromDate.DayOfWeek);
                    for (var date = weekStart; date < toDate; date = date.AddDays(7))
                    {
                        var weekEnd = date.AddDays(7);
                        periods.Add((date, weekEnd, $"W{GetWeekOfYear(date)}"));
                    }
                    break;

                case "monthly":
                    var monthStart = new DateTime(fromDate.Year, fromDate.Month, 1);
                    for (var date = monthStart; date < toDate; date = date.AddMonths(1))
                    {
                        var monthEnd = date.AddMonths(1);
                        periods.Add((date, monthEnd, date.ToString("MMM yyyy")));
                    }
                    break;
            }

            return periods;
        }

        /// <summary>
        /// Calculates trend confidence score
        /// </summary>
        private static decimal CalculateTrendConfidence(List<TrendDataPoint> dataPoints)
        {
            if (dataPoints.Count < 3) return 0;

            // Simple confidence calculation based on data consistency
            var values = dataPoints.Select(d => (double)d.Value).ToList();
            var mean = values.Average();
            var variance = values.Sum(v => Math.Pow(v - mean, 2)) / values.Count;
            var standardDeviation = Math.Sqrt(variance);

            // Lower standard deviation = higher confidence
            var confidenceScore = Math.Max(0, 100 - (standardDeviation / mean * 100));
            return Math.Min(100, (decimal)confidenceScore);
        }

        /// <summary>
        /// Calculates standard deviation
        /// </summary>
        private static decimal CalculateStandardDeviation(IEnumerable<decimal> values)
        {
            var valuesList = values.ToList();
            if (valuesList.Count <= 1) return 0;

            var mean = valuesList.Average();
            var variance = valuesList.Sum(v => (decimal)Math.Pow((double)(v - mean), 2)) / valuesList.Count;
            return (decimal)Math.Sqrt((double)variance);
        }

        /// <summary>
        /// Gets week of year
        /// </summary>
        private static int GetWeekOfYear(DateTime date)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            var calendar = culture.Calendar;
            return calendar.GetWeekOfYear(date, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
        }

        #endregion
    }
}
