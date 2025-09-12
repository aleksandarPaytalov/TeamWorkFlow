using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamWorkFlow.Core.Models.Dashboard
{
    /// <summary>
    /// Main dashboard container model for performance analytics
    /// </summary>
    public class PerformanceDashboardModel
    {
        /// <summary>
        /// Overall efficiency metrics for the dashboard
        /// </summary>
        [Display(Name = "Efficiency Metrics")]
        public EfficiencyMetricsModel EfficiencyMetrics { get; set; } = new EfficiencyMetricsModel();

        /// <summary>
        /// List of operator performance data with comparative analysis
        /// </summary>
        [Display(Name = "Operator Performance")]
        public List<OperatorPerformanceModel> OperatorPerformance { get; set; } = new List<OperatorPerformanceModel>();

        /// <summary>
        /// Bottleneck analysis data identifying delays and issues
        /// </summary>
        [Display(Name = "Bottleneck Analysis")]
        public BottleneckAnalysisModel BottleneckAnalysis { get; set; } = new BottleneckAnalysisModel();

        /// <summary>
        /// Trend chart data for completion and workload visualization
        /// </summary>
        [Display(Name = "Trend Charts")]
        public TrendChartModel TrendCharts { get; set; } = new TrendChartModel();

        /// <summary>
        /// Applied filter criteria for the dashboard
        /// </summary>
        [Display(Name = "Applied Filters")]
        public ReportFilterModel AppliedFilters { get; set; } = new ReportFilterModel();

        /// <summary>
        /// Overall KPI summary section
        /// </summary>
        [Display(Name = "KPI Summary")]
        public DashboardKpiSummaryModel KpiSummary { get; set; } = new DashboardKpiSummaryModel();

        /// <summary>
        /// Timestamp when dashboard data was last updated
        /// </summary>
        [Display(Name = "Last Updated")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Total number of tasks included in the analysis
        /// </summary>
        [Display(Name = "Total Tasks Analyzed")]
        public int TotalTasksAnalyzed { get; set; }

        /// <summary>
        /// Total number of operators included in the analysis
        /// </summary>
        [Display(Name = "Total Operators Analyzed")]
        public int TotalOperatorsAnalyzed { get; set; }

        /// <summary>
        /// Date range covered by the analysis
        /// </summary>
        [Display(Name = "Analysis Period")]
        public string AnalysisPeriod { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if the dashboard has sufficient data for meaningful analysis
        /// </summary>
        public bool HasSufficientData => TotalTasksAnalyzed >= 5 && TotalOperatorsAnalyzed >= 1;

        /// <summary>
        /// Warning message if data is insufficient
        /// </summary>
        public string DataSufficiencyWarning
        {
            get
            {
                if (TotalTasksAnalyzed < 5)
                    return "Insufficient task data for reliable analysis. At least 5 completed tasks are recommended.";
                if (TotalOperatorsAnalyzed < 1)
                    return "No operator data available for analysis.";
                return string.Empty;
            }
        }

        /// <summary>
        /// Formatted display of last updated time
        /// </summary>
        [Display(Name = "Last Updated")]
        public string LastUpdatedFormatted => LastUpdated.ToString("dd/MM/yyyy HH:mm");

        /// <summary>
        /// CSS class for data sufficiency indicator
        /// </summary>
        public string DataSufficiencyClass
        {
            get
            {
                if (!HasSufficientData) return "alert alert-warning";
                return "alert alert-info";
            }
        }

        /// <summary>
        /// Quick access to top performing operator
        /// </summary>
        public OperatorPerformanceModel? TopPerformingOperator
        {
            get
            {
                return OperatorPerformance
                    .Where(op => op.TasksCompleted > 0)
                    .OrderByDescending(op => op.EfficiencyRating)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Quick access to operator needing attention (lowest efficiency)
        /// </summary>
        public OperatorPerformanceModel? OperatorNeedingAttention
        {
            get
            {
                return OperatorPerformance
                    .Where(op => op.TasksCompleted > 0)
                    .OrderBy(op => op.EfficiencyRating)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Overall dashboard health score (0-100)
        /// </summary>
        public decimal OverallHealthScore
        {
            get
            {
                if (!HasSufficientData) return 0;

                var efficiencyScore = Math.Min(100, EfficiencyMetrics.OnTimeCompletionRate);
                var bottleneckScore = Math.Max(0, 100 - (BottleneckAnalysis.TotalBottlenecks * 10));
                var varianceScore = Math.Max(0, 100 - Math.Abs(EfficiencyMetrics.AverageTimeOverrunPercentage));

                return (efficiencyScore + bottleneckScore + varianceScore) / 3;
            }
        }

        /// <summary>
        /// CSS class for overall health score display
        /// </summary>
        public string HealthScoreClass
        {
            get
            {
                if (OverallHealthScore >= 80) return "text-success fw-bold";
                if (OverallHealthScore >= 60) return "text-warning fw-bold";
                return "text-danger fw-bold";
            }
        }

        /// <summary>
        /// Health score status text
        /// </summary>
        public string HealthScoreStatus
        {
            get
            {
                if (OverallHealthScore >= 80) return "Excellent";
                if (OverallHealthScore >= 60) return "Good";
                if (OverallHealthScore >= 40) return "Needs Improvement";
                return "Critical";
            }
        }
    }

    /// <summary>
    /// KPI summary model for dashboard overview
    /// </summary>
    public class DashboardKpiSummaryModel
    {
        /// <summary>
        /// Total tasks completed in the analysis period
        /// </summary>
        [Display(Name = "Tasks Completed")]
        public int TasksCompleted { get; set; }

        /// <summary>
        /// Total tasks currently in progress
        /// </summary>
        [Display(Name = "Tasks In Progress")]
        public int TasksInProgress { get; set; }

        /// <summary>
        /// Total tasks overdue
        /// </summary>
        [Display(Name = "Overdue Tasks")]
        public int OverdueTasks { get; set; }

        /// <summary>
        /// Average completion time across all tasks (hours)
        /// </summary>
        [Display(Name = "Avg Completion Time")]
        public decimal AverageCompletionTimeHours { get; set; }

        /// <summary>
        /// Total productive hours logged
        /// </summary>
        [Display(Name = "Total Productive Hours")]
        public decimal TotalProductiveHours { get; set; }

        /// <summary>
        /// Number of active operators
        /// </summary>
        [Display(Name = "Active Operators")]
        public int ActiveOperators { get; set; }

        /// <summary>
        /// Formatted display of average completion time
        /// </summary>
        public string AverageCompletionTimeFormatted
        {
            get
            {
                var totalMinutes = (int)(AverageCompletionTimeHours * 60);
                var hours = totalMinutes / 60;
                var minutes = totalMinutes % 60;
                return hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";
            }
        }

        /// <summary>
        /// Formatted display of total productive hours
        /// </summary>
        public string TotalProductiveHoursFormatted => $"{TotalProductiveHours:F1}h";
    }
}
