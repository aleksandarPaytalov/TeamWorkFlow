using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamWorkFlow.Core.Models.Dashboard
{
    /// <summary>
    /// Model for bottleneck identification and analysis visualization
    /// </summary>
    public class BottleneckAnalysisModel
    {
        /// <summary>
        /// List of frequently delayed tasks and patterns
        /// </summary>
        [Display(Name = "Frequent Delays")]
        public List<DelayPatternModel> FrequentDelayPatterns { get; set; } = new List<DelayPatternModel>();

        /// <summary>
        /// Average delay time across all bottlenecks (minutes)
        /// </summary>
        [Display(Name = "Average Delay Time")]
        public int AverageDelayTimeMinutes { get; set; }

        /// <summary>
        /// Root cause categories for delays
        /// </summary>
        [Display(Name = "Root Causes")]
        public List<RootCauseModel> RootCauseCategories { get; set; } = new List<RootCauseModel>();

        /// <summary>
        /// Improvement recommendations based on analysis
        /// </summary>
        [Display(Name = "Recommendations")]
        public List<ImprovementRecommendationModel> Recommendations { get; set; } = new List<ImprovementRecommendationModel>();

        /// <summary>
        /// Tasks currently experiencing delays
        /// </summary>
        [Display(Name = "Current Bottlenecks")]
        public List<CurrentBottleneckModel> CurrentBottlenecks { get; set; } = new List<CurrentBottleneckModel>();

        /// <summary>
        /// Operators most affected by bottlenecks
        /// </summary>
        [Display(Name = "Affected Operators")]
        public List<OperatorBottleneckModel> AffectedOperators { get; set; } = new List<OperatorBottleneckModel>();

        /// <summary>
        /// Total number of identified bottlenecks
        /// </summary>
        [Display(Name = "Total Bottlenecks")]
        public int TotalBottlenecks { get; set; }

        /// <summary>
        /// Percentage of tasks affected by bottlenecks
        /// </summary>
        [Display(Name = "Tasks Affected %")]
        public decimal TasksAffectedPercentage { get; set; }

        /// <summary>
        /// Total time lost due to bottlenecks (hours)
        /// </summary>
        [Display(Name = "Time Lost (Hours)")]
        public decimal TotalTimeLostHours { get; set; }

        /// <summary>
        /// Bottleneck severity score (0-100, higher is worse)
        /// </summary>
        [Display(Name = "Severity Score")]
        public decimal SeverityScore { get; set; }

        /// <summary>
        /// Analysis period for bottleneck data
        /// </summary>
        [Display(Name = "Analysis Period")]
        public string AnalysisPeriod { get; set; } = string.Empty;

        /// <summary>
        /// Trend in bottleneck frequency (positive = increasing, negative = decreasing)
        /// </summary>
        [Display(Name = "Bottleneck Trend")]
        public decimal BottleneckTrend { get; set; }

        /// <summary>
        /// Most problematic time periods for bottlenecks
        /// </summary>
        public List<ProblematicPeriodModel> ProblematicPeriods { get; set; } = new List<ProblematicPeriodModel>();

        /// <summary>
        /// Formatted display of average delay time
        /// </summary>
        [Display(Name = "Avg Delay")]
        public string AverageDelayTimeFormatted
        {
            get
            {
                var hours = AverageDelayTimeMinutes / 60;
                var minutes = AverageDelayTimeMinutes % 60;
                return hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";
            }
        }

        /// <summary>
        /// Formatted display of tasks affected percentage
        /// </summary>
        [Display(Name = "Affected")]
        public string TasksAffectedFormatted => $"{TasksAffectedPercentage:F1}%";

        /// <summary>
        /// Formatted display of total time lost
        /// </summary>
        [Display(Name = "Time Lost")]
        public string TotalTimeLostFormatted => $"{TotalTimeLostHours:F1}h";

        /// <summary>
        /// Formatted display of severity score
        /// </summary>
        [Display(Name = "Severity")]
        public string SeverityScoreFormatted => $"{SeverityScore:F0}/100";

        /// <summary>
        /// Formatted display of bottleneck trend
        /// </summary>
        [Display(Name = "Trend")]
        public string BottleneckTrendFormatted
        {
            get
            {
                var sign = BottleneckTrend >= 0 ? "+" : "";
                return $"{sign}{BottleneckTrend:F1}%";
            }
        }

        /// <summary>
        /// CSS class for severity score display
        /// </summary>
        public string SeverityClass
        {
            get
            {
                if (SeverityScore >= 80) return "text-danger fw-bold";
                if (SeverityScore >= 60) return "text-warning fw-bold";
                if (SeverityScore >= 40) return "text-warning";
                return "text-success";
            }
        }

        /// <summary>
        /// CSS class for bottleneck trend display
        /// </summary>
        public string TrendClass
        {
            get
            {
                if (BottleneckTrend > 10) return "text-danger fw-bold";
                if (BottleneckTrend > 0) return "text-warning";
                if (BottleneckTrend > -10) return "text-info";
                return "text-success fw-bold";
            }
        }

        /// <summary>
        /// Icon for bottleneck trend
        /// </summary>
        public string TrendIcon
        {
            get
            {
                if (BottleneckTrend > 0) return "fas fa-arrow-up";
                if (BottleneckTrend < 0) return "fas fa-arrow-down";
                return "fas fa-minus";
            }
        }

        /// <summary>
        /// Severity status description
        /// </summary>
        public string SeverityStatus
        {
            get
            {
                if (SeverityScore >= 80) return "Critical";
                if (SeverityScore >= 60) return "High";
                if (SeverityScore >= 40) return "Medium";
                if (SeverityScore >= 20) return "Low";
                return "Minimal";
            }
        }

        /// <summary>
        /// Badge CSS class for severity display
        /// </summary>
        public string SeverityBadgeClass
        {
            get
            {
                if (SeverityScore >= 80) return "badge bg-danger";
                if (SeverityScore >= 60) return "badge bg-warning";
                if (SeverityScore >= 40) return "badge bg-info";
                return "badge bg-success";
            }
        }

        /// <summary>
        /// Indicates if immediate attention is required
        /// </summary>
        public bool RequiresImmediateAttention => SeverityScore >= 70 || TotalBottlenecks >= 5;

        /// <summary>
        /// Priority level for addressing bottlenecks
        /// </summary>
        public string PriorityLevel
        {
            get
            {
                if (RequiresImmediateAttention) return "High";
                if (SeverityScore >= 40) return "Medium";
                return "Low";
            }
        }

        /// <summary>
        /// Most critical bottleneck (highest impact)
        /// </summary>
        public DelayPatternModel? MostCriticalBottleneck
        {
            get
            {
                return FrequentDelayPatterns
                    .OrderByDescending(p => p.ImpactScore)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Top recommendation for improvement
        /// </summary>
        public ImprovementRecommendationModel? TopRecommendation
        {
            get
            {
                return Recommendations
                    .OrderByDescending(r => r.Priority)
                    .ThenByDescending(r => r.ExpectedImpact)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Indicates if there are actionable recommendations
        /// </summary>
        public bool HasActionableRecommendations => Recommendations.Any(r => r.Priority >= 7);

        /// <summary>
        /// Summary of bottleneck impact
        /// </summary>
        public string ImpactSummary
        {
            get
            {
                if (TotalBottlenecks == 0) return "No significant bottlenecks identified.";
                if (RequiresImmediateAttention) return $"Critical: {TotalBottlenecks} bottlenecks affecting {TasksAffectedPercentage:F0}% of tasks.";
                return $"{TotalBottlenecks} bottlenecks identified with {SeverityStatus.ToLower()} impact.";
            }
        }
    }

    /// <summary>
    /// Model for delay patterns and frequent bottlenecks
    /// </summary>
    public class DelayPatternModel
    {
        /// <summary>
        /// Pattern identifier or description
        /// </summary>
        public string PatternName { get; set; } = string.Empty;

        /// <summary>
        /// Number of times this pattern occurred
        /// </summary>
        public int Occurrences { get; set; }

        /// <summary>
        /// Average delay time for this pattern (minutes)
        /// </summary>
        public int AverageDelayMinutes { get; set; }

        /// <summary>
        /// Total time lost due to this pattern (minutes)
        /// </summary>
        public int TotalTimeLostMinutes { get; set; }

        /// <summary>
        /// Tasks affected by this pattern
        /// </summary>
        public List<string> AffectedTasks { get; set; } = new List<string>();

        /// <summary>
        /// Impact score (0-100, higher is worse)
        /// </summary>
        public decimal ImpactScore { get; set; }

        /// <summary>
        /// Category of the delay pattern
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Suggested resolution for this pattern
        /// </summary>
        public string SuggestedResolution { get; set; } = string.Empty;

        /// <summary>
        /// Formatted display of average delay
        /// </summary>
        public string AverageDelayFormatted
        {
            get
            {
                var hours = AverageDelayMinutes / 60;
                var minutes = AverageDelayMinutes % 60;
                return hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";
            }
        }

        /// <summary>
        /// Formatted display of total time lost
        /// </summary>
        public string TotalTimeLostFormatted
        {
            get
            {
                var hours = TotalTimeLostMinutes / 60;
                var minutes = TotalTimeLostMinutes % 60;
                return hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";
            }
        }

        /// <summary>
        /// CSS class for impact score display
        /// </summary>
        public string ImpactClass
        {
            get
            {
                if (ImpactScore >= 80) return "text-danger fw-bold";
                if (ImpactScore >= 60) return "text-warning fw-bold";
                if (ImpactScore >= 40) return "text-warning";
                return "text-info";
            }
        }
    }

    /// <summary>
    /// Model for root cause analysis
    /// </summary>
    public class RootCauseModel
    {
        /// <summary>
        /// Root cause category name
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Number of incidents attributed to this cause
        /// </summary>
        public int IncidentCount { get; set; }

        /// <summary>
        /// Percentage of total delays caused by this category
        /// </summary>
        public decimal Percentage { get; set; }

        /// <summary>
        /// Average impact severity (1-10)
        /// </summary>
        public decimal AverageSeverity { get; set; }

        /// <summary>
        /// Specific examples of this root cause
        /// </summary>
        public List<string> Examples { get; set; } = new List<string>();

        /// <summary>
        /// Formatted percentage display
        /// </summary>
        public string PercentageFormatted => $"{Percentage:F1}%";
    }

    /// <summary>
    /// Model for improvement recommendations
    /// </summary>
    public class ImprovementRecommendationModel
    {
        /// <summary>
        /// Recommendation title
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of the recommendation
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Priority level (1-10, higher is more urgent)
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Expected impact if implemented (1-10)
        /// </summary>
        public int ExpectedImpact { get; set; }

        /// <summary>
        /// Estimated effort to implement (1-10)
        /// </summary>
        public int ImplementationEffort { get; set; }

        /// <summary>
        /// Category of the recommendation
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Estimated time to implement
        /// </summary>
        public string EstimatedTimeframe { get; set; } = string.Empty;

        /// <summary>
        /// CSS class for priority display
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

        /// <summary>
        /// Priority level text
        /// </summary>
        public string PriorityText
        {
            get
            {
                if (Priority >= 8) return "High";
                if (Priority >= 6) return "Medium";
                if (Priority >= 4) return "Low";
                return "Optional";
            }
        }
    }

    /// <summary>
    /// Model for current active bottlenecks
    /// </summary>
    public class CurrentBottleneckModel
    {
        /// <summary>
        /// Task identifier
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// Task name
        /// </summary>
        public string TaskName { get; set; } = string.Empty;

        /// <summary>
        /// Current delay in minutes
        /// </summary>
        public int DelayMinutes { get; set; }

        /// <summary>
        /// Assigned operator name
        /// </summary>
        public string OperatorName { get; set; } = string.Empty;

        /// <summary>
        /// Reason for the bottleneck
        /// </summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// Estimated resolution time
        /// </summary>
        public DateTime? EstimatedResolution { get; set; }

        /// <summary>
        /// Formatted delay display
        /// </summary>
        public string DelayFormatted
        {
            get
            {
                var hours = DelayMinutes / 60;
                var minutes = DelayMinutes % 60;
                return hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";
            }
        }
    }

    /// <summary>
    /// Model for operators affected by bottlenecks
    /// </summary>
    public class OperatorBottleneckModel
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
        /// Number of tasks affected by bottlenecks
        /// </summary>
        public int AffectedTasks { get; set; }

        /// <summary>
        /// Total delay time for this operator (minutes)
        /// </summary>
        public int TotalDelayMinutes { get; set; }

        /// <summary>
        /// Percentage of operator's tasks affected
        /// </summary>
        public decimal AffectedTasksPercentage { get; set; }

        /// <summary>
        /// Formatted total delay display
        /// </summary>
        public string TotalDelayFormatted
        {
            get
            {
                var hours = TotalDelayMinutes / 60;
                var minutes = TotalDelayMinutes % 60;
                return hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";
            }
        }
    }

    /// <summary>
    /// Model for problematic time periods
    /// </summary>
    public class ProblematicPeriodModel
    {
        /// <summary>
        /// Time period description
        /// </summary>
        public string Period { get; set; } = string.Empty;

        /// <summary>
        /// Number of bottlenecks in this period
        /// </summary>
        public int BottleneckCount { get; set; }

        /// <summary>
        /// Average delay time in this period
        /// </summary>
        public int AverageDelayMinutes { get; set; }

        /// <summary>
        /// Common causes during this period
        /// </summary>
        public List<string> CommonCauses { get; set; } = new List<string>();
    }
}
