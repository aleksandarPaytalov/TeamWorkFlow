using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamWorkFlow.Core.Models.Dashboard
{
    /// <summary>
    /// Model for workload distribution analysis across operators and time periods
    /// </summary>
    public class WorkloadDistributionModel
    {
        /// <summary>
        /// Analysis period start date
        /// </summary>
        [Display(Name = "Period Start")]
        public DateTime PeriodStart { get; set; }

        /// <summary>
        /// Analysis period end date
        /// </summary>
        [Display(Name = "Period End")]
        public DateTime PeriodEnd { get; set; }

        /// <summary>
        /// Workload distribution by operator
        /// </summary>
        [Display(Name = "Operator Workload")]
        public List<OperatorWorkloadModel> OperatorWorkloads { get; set; } = new List<OperatorWorkloadModel>();

        /// <summary>
        /// Workload distribution by project
        /// </summary>
        [Display(Name = "Project Workload")]
        public List<ProjectWorkloadModel> ProjectWorkloads { get; set; } = new List<ProjectWorkloadModel>();

        /// <summary>
        /// Daily workload distribution
        /// </summary>
        [Display(Name = "Daily Distribution")]
        public List<DailyWorkloadModel> DailyWorkloads { get; set; } = new List<DailyWorkloadModel>();

        /// <summary>
        /// Capacity analysis for operators
        /// </summary>
        [Display(Name = "Capacity Analysis")]
        public List<OperatorCapacityModel> CapacityAnalysis { get; set; } = new List<OperatorCapacityModel>();

        /// <summary>
        /// Workload balance metrics
        /// </summary>
        [Display(Name = "Balance Metrics")]
        public WorkloadBalanceMetricsModel BalanceMetrics { get; set; } = new WorkloadBalanceMetricsModel();

        /// <summary>
        /// Peak workload periods
        /// </summary>
        [Display(Name = "Peak Periods")]
        public List<PeakWorkloadPeriodModel> PeakPeriods { get; set; } = new List<PeakWorkloadPeriodModel>();

        /// <summary>
        /// Workload forecasting data
        /// </summary>
        [Display(Name = "Forecast")]
        public WorkloadForecastModel Forecast { get; set; } = new WorkloadForecastModel();

        /// <summary>
        /// Resource allocation recommendations
        /// </summary>
        [Display(Name = "Recommendations")]
        public List<ResourceAllocationRecommendationModel> Recommendations { get; set; } = new List<ResourceAllocationRecommendationModel>();

        /// <summary>
        /// Total tasks in the analysis period
        /// </summary>
        [Display(Name = "Total Tasks")]
        public int TotalTasks { get; set; }

        /// <summary>
        /// Total hours worked in the analysis period
        /// </summary>
        [Display(Name = "Total Hours")]
        public decimal TotalHours { get; set; }

        /// <summary>
        /// Average tasks per operator
        /// </summary>
        [Display(Name = "Avg Tasks per Operator")]
        public decimal AverageTasksPerOperator { get; set; }

        /// <summary>
        /// Average hours per operator
        /// </summary>
        [Display(Name = "Avg Hours per Operator")]
        public decimal AverageHoursPerOperator { get; set; }

        /// <summary>
        /// Workload distribution variance (measure of imbalance)
        /// </summary>
        [Display(Name = "Distribution Variance")]
        public decimal DistributionVariance { get; set; }

        /// <summary>
        /// Number of active operators
        /// </summary>
        [Display(Name = "Active Operators")]
        public int ActiveOperators { get; set; }

        /// <summary>
        /// Formatted analysis period
        /// </summary>
        [Display(Name = "Analysis Period")]
        public string AnalysisPeriodFormatted => $"{PeriodStart:dd/MM/yyyy} - {PeriodEnd:dd/MM/yyyy}";

        /// <summary>
        /// Formatted total hours
        /// </summary>
        [Display(Name = "Total Hours")]
        public string TotalHoursFormatted => $"{TotalHours:F1}h";

        /// <summary>
        /// Formatted average tasks per operator
        /// </summary>
        [Display(Name = "Avg Tasks")]
        public string AverageTasksPerOperatorFormatted => $"{AverageTasksPerOperator:F1}";

        /// <summary>
        /// Formatted average hours per operator
        /// </summary>
        [Display(Name = "Avg Hours")]
        public string AverageHoursPerOperatorFormatted => $"{AverageHoursPerOperator:F1}h";

        /// <summary>
        /// Workload balance status
        /// </summary>
        public string WorkloadBalanceStatus
        {
            get
            {
                if (DistributionVariance <= 0.1m) return "Well Balanced";
                if (DistributionVariance <= 0.25m) return "Moderately Balanced";
                if (DistributionVariance <= 0.5m) return "Imbalanced";
                return "Severely Imbalanced";
            }
        }

        /// <summary>
        /// CSS class for workload balance status
        /// </summary>
        public string WorkloadBalanceClass
        {
            get
            {
                return WorkloadBalanceStatus switch
                {
                    "Well Balanced" => "text-success fw-bold",
                    "Moderately Balanced" => "text-success",
                    "Imbalanced" => "text-warning",
                    "Severely Imbalanced" => "text-danger fw-bold",
                    _ => "text-muted"
                };
            }
        }

        /// <summary>
        /// Most overloaded operator
        /// </summary>
        public OperatorWorkloadModel? MostOverloadedOperator
        {
            get
            {
                return OperatorWorkloads
                    .Where(o => o.OverloadPercentage > 0)
                    .OrderByDescending(o => o.OverloadPercentage)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Most underutilized operator
        /// </summary>
        public OperatorWorkloadModel? MostUnderutilizedOperator
        {
            get
            {
                return OperatorWorkloads
                    .Where(o => o.UtilizationPercentage < 80)
                    .OrderBy(o => o.UtilizationPercentage)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Indicates if workload redistribution is needed
        /// </summary>
        public bool NeedsRedistribution => DistributionVariance > 0.25m || OperatorWorkloads.Any(o => o.OverloadPercentage > 20);

        /// <summary>
        /// Overall capacity utilization percentage
        /// </summary>
        public decimal OverallCapacityUtilization
        {
            get
            {
                if (!CapacityAnalysis.Any()) return 0;
                return CapacityAnalysis.Average(c => c.UtilizationPercentage);
            }
        }

        /// <summary>
        /// Formatted overall capacity utilization
        /// </summary>
        public string OverallCapacityUtilizationFormatted => $"{OverallCapacityUtilization:F1}%";
    }

    /// <summary>
    /// Model for individual operator workload data
    /// </summary>
    public class OperatorWorkloadModel
    {
        /// <summary>
        /// Operator identifier
        /// </summary>
        public int OperatorId { get; set; }

        /// <summary>
        /// Operator name
        /// </summary>
        [Display(Name = "Operator")]
        public string OperatorName { get; set; } = string.Empty;

        /// <summary>
        /// Number of tasks assigned
        /// </summary>
        [Display(Name = "Tasks Assigned")]
        public int TasksAssigned { get; set; }

        /// <summary>
        /// Number of tasks completed
        /// </summary>
        [Display(Name = "Tasks Completed")]
        public int TasksCompleted { get; set; }

        /// <summary>
        /// Number of tasks in progress
        /// </summary>
        [Display(Name = "Tasks In Progress")]
        public int TasksInProgress { get; set; }

        /// <summary>
        /// Total hours worked
        /// </summary>
        [Display(Name = "Hours Worked")]
        public decimal HoursWorked { get; set; }

        /// <summary>
        /// Percentage of total workload
        /// </summary>
        [Display(Name = "Workload %")]
        public decimal WorkloadPercentage { get; set; }

        /// <summary>
        /// Utilization percentage based on capacity
        /// </summary>
        [Display(Name = "Utilization %")]
        public decimal UtilizationPercentage { get; set; }

        /// <summary>
        /// Overload percentage (if over capacity)
        /// </summary>
        [Display(Name = "Overload %")]
        public decimal OverloadPercentage { get; set; }

        /// <summary>
        /// Average task complexity score
        /// </summary>
        [Display(Name = "Avg Complexity")]
        public decimal AverageComplexity { get; set; }

        /// <summary>
        /// Workload trend (increasing, decreasing, stable)
        /// </summary>
        [Display(Name = "Trend")]
        public string WorkloadTrend { get; set; } = "Stable";

        /// <summary>
        /// Workload distribution by project
        /// </summary>
        public List<ProjectWorkloadBreakdownModel> ProjectBreakdown { get; set; } = new List<ProjectWorkloadBreakdownModel>();

        /// <summary>
        /// Formatted hours worked
        /// </summary>
        [Display(Name = "Hours")]
        public string HoursWorkedFormatted => $"{HoursWorked:F1}h";

        /// <summary>
        /// Formatted workload percentage
        /// </summary>
        [Display(Name = "Workload")]
        public string WorkloadPercentageFormatted => $"{WorkloadPercentage:F1}%";

        /// <summary>
        /// Formatted utilization percentage
        /// </summary>
        [Display(Name = "Utilization")]
        public string UtilizationPercentageFormatted => $"{UtilizationPercentage:F1}%";

        /// <summary>
        /// Task completion rate
        /// </summary>
        public decimal CompletionRate
        {
            get
            {
                if (TasksAssigned == 0) return 0;
                return (decimal)TasksCompleted / TasksAssigned * 100;
            }
        }

        /// <summary>
        /// Formatted completion rate
        /// </summary>
        public string CompletionRateFormatted => $"{CompletionRate:F1}%";

        /// <summary>
        /// Workload status
        /// </summary>
        public string WorkloadStatus
        {
            get
            {
                if (OverloadPercentage > 20) return "Overloaded";
                if (OverloadPercentage > 0) return "At Capacity";
                if (UtilizationPercentage >= 80) return "Well Utilized";
                if (UtilizationPercentage >= 60) return "Moderately Utilized";
                return "Underutilized";
            }
        }

        /// <summary>
        /// CSS class for workload status
        /// </summary>
        public string WorkloadStatusClass
        {
            get
            {
                return WorkloadStatus switch
                {
                    "Overloaded" => "badge bg-danger",
                    "At Capacity" => "badge bg-warning",
                    "Well Utilized" => "badge bg-success",
                    "Moderately Utilized" => "badge bg-info",
                    "Underutilized" => "badge bg-secondary",
                    _ => "badge bg-light"
                };
            }
        }

        /// <summary>
        /// CSS class for utilization percentage
        /// </summary>
        public string UtilizationClass
        {
            get
            {
                if (UtilizationPercentage > 100) return "text-danger fw-bold";
                if (UtilizationPercentage >= 90) return "text-warning fw-bold";
                if (UtilizationPercentage >= 70) return "text-success";
                return "text-info";
            }
        }
    }

    /// <summary>
    /// Model for project workload data
    /// </summary>
    public class ProjectWorkloadModel
    {
        /// <summary>
        /// Project identifier
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// Project name
        /// </summary>
        [Display(Name = "Project")]
        public string ProjectName { get; set; } = string.Empty;

        /// <summary>
        /// Project number
        /// </summary>
        [Display(Name = "Project Number")]
        public string ProjectNumber { get; set; } = string.Empty;

        /// <summary>
        /// Total tasks in project
        /// </summary>
        [Display(Name = "Total Tasks")]
        public int TotalTasks { get; set; }

        /// <summary>
        /// Total hours allocated to project
        /// </summary>
        [Display(Name = "Total Hours")]
        public decimal TotalHours { get; set; }

        /// <summary>
        /// Number of operators working on project
        /// </summary>
        [Display(Name = "Operators")]
        public int OperatorCount { get; set; }

        /// <summary>
        /// Percentage of total organizational workload
        /// </summary>
        [Display(Name = "Workload %")]
        public decimal WorkloadPercentage { get; set; }

        /// <summary>
        /// Project priority level
        /// </summary>
        [Display(Name = "Priority")]
        public string Priority { get; set; } = string.Empty;

        /// <summary>
        /// Project status
        /// </summary>
        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Formatted total hours
        /// </summary>
        public string TotalHoursFormatted => $"{TotalHours:F1}h";

        /// <summary>
        /// Formatted workload percentage
        /// </summary>
        public string WorkloadPercentageFormatted => $"{WorkloadPercentage:F1}%";
    }

    /// <summary>
    /// Model for daily workload distribution
    /// </summary>
    public class DailyWorkloadModel
    {
        /// <summary>
        /// Date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Total tasks worked on this date
        /// </summary>
        public int TotalTasks { get; set; }

        /// <summary>
        /// Total hours worked on this date
        /// </summary>
        public decimal TotalHours { get; set; }

        /// <summary>
        /// Number of active operators on this date
        /// </summary>
        public int ActiveOperators { get; set; }

        /// <summary>
        /// Average workload intensity (0-100)
        /// </summary>
        public decimal WorkloadIntensity { get; set; }

        /// <summary>
        /// Day of week
        /// </summary>
        public string DayOfWeek => Date.ToString("dddd");

        /// <summary>
        /// Formatted date
        /// </summary>
        public string DateFormatted => Date.ToString("dd/MM/yyyy");

        /// <summary>
        /// Formatted total hours
        /// </summary>
        public string TotalHoursFormatted => $"{TotalHours:F1}h";
    }

    /// <summary>
    /// Model for operator capacity analysis
    /// </summary>
    public class OperatorCapacityModel
    {
        /// <summary>
        /// Operator identifier
        /// </summary>
        public int OperatorId { get; set; }

        /// <summary>
        /// Operator name
        /// </summary>
        public string OperatorName { get; set; } = string.Empty;

        /// <summary>
        /// Maximum capacity (hours per period)
        /// </summary>
        public decimal MaxCapacityHours { get; set; }

        /// <summary>
        /// Current utilization (hours)
        /// </summary>
        public decimal CurrentUtilizationHours { get; set; }

        /// <summary>
        /// Available capacity (hours)
        /// </summary>
        public decimal AvailableCapacityHours { get; set; }

        /// <summary>
        /// Utilization percentage
        /// </summary>
        public decimal UtilizationPercentage { get; set; }

        /// <summary>
        /// Recommended additional tasks
        /// </summary>
        public int RecommendedAdditionalTasks { get; set; }

        /// <summary>
        /// Capacity status
        /// </summary>
        public string CapacityStatus { get; set; } = string.Empty;

        /// <summary>
        /// Formatted utilization percentage
        /// </summary>
        public string UtilizationPercentageFormatted => $"{UtilizationPercentage:F1}%";

        /// <summary>
        /// Formatted available capacity
        /// </summary>
        public string AvailableCapacityFormatted => $"{AvailableCapacityHours:F1}h";
    }

    /// <summary>
    /// Model for workload balance metrics
    /// </summary>
    public class WorkloadBalanceMetricsModel
    {
        /// <summary>
        /// Standard deviation of workload distribution
        /// </summary>
        public decimal StandardDeviation { get; set; }

        /// <summary>
        /// Coefficient of variation
        /// </summary>
        public decimal CoefficientOfVariation { get; set; }

        /// <summary>
        /// Gini coefficient (0 = perfect equality, 1 = perfect inequality)
        /// </summary>
        public decimal GiniCoefficient { get; set; }

        /// <summary>
        /// Balance score (0-100, higher is better)
        /// </summary>
        public decimal BalanceScore { get; set; }

        /// <summary>
        /// Number of operators above average workload
        /// </summary>
        public int OperatorsAboveAverage { get; set; }

        /// <summary>
        /// Number of operators below average workload
        /// </summary>
        public int OperatorsBelowAverage { get; set; }

        /// <summary>
        /// Formatted balance score
        /// </summary>
        public string BalanceScoreFormatted => $"{BalanceScore:F1}/100";

        /// <summary>
        /// Balance status description
        /// </summary>
        public string BalanceStatus
        {
            get
            {
                if (BalanceScore >= 80) return "Well Balanced";
                if (BalanceScore >= 60) return "Moderately Balanced";
                if (BalanceScore >= 40) return "Imbalanced";
                return "Severely Imbalanced";
            }
        }
    }

    /// <summary>
    /// Model for peak workload periods
    /// </summary>
    public class PeakWorkloadPeriodModel
    {
        /// <summary>
        /// Start date of peak period
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of peak period
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Peak workload intensity
        /// </summary>
        public decimal PeakIntensity { get; set; }

        /// <summary>
        /// Duration in days
        /// </summary>
        public int DurationDays { get; set; }

        /// <summary>
        /// Contributing factors
        /// </summary>
        public List<string> ContributingFactors { get; set; } = new List<string>();

        /// <summary>
        /// Formatted period
        /// </summary>
        public string PeriodFormatted => $"{StartDate:dd/MM/yyyy} - {EndDate:dd/MM/yyyy}";
    }

    /// <summary>
    /// Model for workload forecasting
    /// </summary>
    public class WorkloadForecastModel
    {
        /// <summary>
        /// Forecast period start
        /// </summary>
        public DateTime ForecastStart { get; set; }

        /// <summary>
        /// Forecast period end
        /// </summary>
        public DateTime ForecastEnd { get; set; }

        /// <summary>
        /// Predicted workload data points
        /// </summary>
        public List<WorkloadForecastPoint> ForecastPoints { get; set; } = new List<WorkloadForecastPoint>();

        /// <summary>
        /// Confidence level of forecast (0-100)
        /// </summary>
        public decimal ConfidenceLevel { get; set; }

        /// <summary>
        /// Predicted peak periods
        /// </summary>
        public List<PredictedPeakPeriod> PredictedPeaks { get; set; } = new List<PredictedPeakPeriod>();

        /// <summary>
        /// Capacity warnings
        /// </summary>
        public List<CapacityWarning> CapacityWarnings { get; set; } = new List<CapacityWarning>();
    }

    /// <summary>
    /// Model for workload forecast data points
    /// </summary>
    public class WorkloadForecastPoint
    {
        /// <summary>
        /// Date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Predicted workload
        /// </summary>
        public decimal PredictedWorkload { get; set; }

        /// <summary>
        /// Confidence interval lower bound
        /// </summary>
        public decimal ConfidenceLower { get; set; }

        /// <summary>
        /// Confidence interval upper bound
        /// </summary>
        public decimal ConfidenceUpper { get; set; }
    }

    /// <summary>
    /// Model for predicted peak periods
    /// </summary>
    public class PredictedPeakPeriod
    {
        /// <summary>
        /// Start date
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Predicted intensity
        /// </summary>
        public decimal PredictedIntensity { get; set; }

        /// <summary>
        /// Probability of occurrence (0-100)
        /// </summary>
        public decimal Probability { get; set; }
    }

    /// <summary>
    /// Model for capacity warnings
    /// </summary>
    public class CapacityWarning
    {
        /// <summary>
        /// Warning date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Warning message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Severity level
        /// </summary>
        public string Severity { get; set; } = string.Empty;

        /// <summary>
        /// Affected operators
        /// </summary>
        public List<string> AffectedOperators { get; set; } = new List<string>();
    }

    /// <summary>
    /// Model for project workload breakdown
    /// </summary>
    public class ProjectWorkloadBreakdownModel
    {
        /// <summary>
        /// Project identifier
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// Project name
        /// </summary>
        public string ProjectName { get; set; } = string.Empty;

        /// <summary>
        /// Hours allocated to this project
        /// </summary>
        public decimal Hours { get; set; }

        /// <summary>
        /// Percentage of operator's total workload
        /// </summary>
        public decimal Percentage { get; set; }

        /// <summary>
        /// Number of tasks in this project
        /// </summary>
        public int TaskCount { get; set; }
    }

    /// <summary>
    /// Model for resource allocation recommendations
    /// </summary>
    public class ResourceAllocationRecommendationModel
    {
        /// <summary>
        /// Recommendation title
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Recommendation description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Recommendation type
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Priority level (1-10)
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Expected impact
        /// </summary>
        public string ExpectedImpact { get; set; } = string.Empty;

        /// <summary>
        /// Affected operators
        /// </summary>
        public List<string> AffectedOperators { get; set; } = new List<string>();

        /// <summary>
        /// Implementation steps
        /// </summary>
        public List<string> ImplementationSteps { get; set; } = new List<string>();

        /// <summary>
        /// CSS class for priority
        /// </summary>
        public string PriorityClass
        {
            get
            {
                if (Priority >= 8) return "badge bg-danger";
                if (Priority >= 6) return "badge bg-warning";
                if (Priority >= 4) return "badge bg-info";
                return "badge bg-secondary";
            }
        }
    }
}
