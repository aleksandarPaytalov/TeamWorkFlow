using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamWorkFlow.Core.Models.Dashboard
{
    /// <summary>
    /// Model for report filter criteria and dashboard customization
    /// </summary>
    public class ReportFilterModel
    {
        /// <summary>
        /// Start date for the analysis period
        /// </summary>
        [Display(Name = "From Date")]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// End date for the analysis period
        /// </summary>
        [Display(Name = "To Date")]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Selected operator IDs for filtering
        /// </summary>
        [Display(Name = "Operators")]
        public List<int> SelectedOperatorIds { get; set; } = new List<int>();

        /// <summary>
        /// Selected project IDs for filtering
        /// </summary>
        [Display(Name = "Projects")]
        public List<int> SelectedProjectIds { get; set; } = new List<int>();

        /// <summary>
        /// Selected task status filters
        /// </summary>
        [Display(Name = "Task Status")]
        public List<string> SelectedTaskStatuses { get; set; } = new List<string>();

        /// <summary>
        /// Selected task priority filters
        /// </summary>
        [Display(Name = "Task Priority")]
        public List<string> SelectedTaskPriorities { get; set; } = new List<string>();

        /// <summary>
        /// Time granularity for trend analysis (daily, weekly, monthly)
        /// </summary>
        [Display(Name = "Time Granularity")]
        public string TimeGranularity { get; set; } = "weekly";

        /// <summary>
        /// Sort criteria for operator performance ranking
        /// </summary>
        [Display(Name = "Sort By")]
        public string SortBy { get; set; } = "efficiency";

        /// <summary>
        /// Sort direction (asc, desc)
        /// </summary>
        [Display(Name = "Sort Direction")]
        public string SortDirection { get; set; } = "desc";

        /// <summary>
        /// Include completed tasks only
        /// </summary>
        [Display(Name = "Completed Tasks Only")]
        public bool CompletedTasksOnly { get; set; } = false;

        /// <summary>
        /// Include tasks with time tracking data only
        /// </summary>
        [Display(Name = "With Time Tracking Only")]
        public bool WithTimeTrackingOnly { get; set; } = true;

        /// <summary>
        /// Minimum task duration filter (hours)
        /// </summary>
        [Display(Name = "Min Duration (Hours)")]
        [Range(0, 1000, ErrorMessage = "Duration must be between 0 and 1000 hours")]
        public decimal? MinDurationHours { get; set; }

        /// <summary>
        /// Maximum task duration filter (hours)
        /// </summary>
        [Display(Name = "Max Duration (Hours)")]
        [Range(0, 1000, ErrorMessage = "Duration must be between 0 and 1000 hours")]
        public decimal? MaxDurationHours { get; set; }

        /// <summary>
        /// Include only tasks with significant variance (>20%)
        /// </summary>
        [Display(Name = "High Variance Tasks Only")]
        public bool HighVarianceTasksOnly { get; set; } = false;

        /// <summary>
        /// Exclude outliers from analysis
        /// </summary>
        [Display(Name = "Exclude Outliers")]
        public bool ExcludeOutliers { get; set; } = false;

        /// <summary>
        /// Group results by project
        /// </summary>
        [Display(Name = "Group by Project")]
        public bool GroupByProject { get; set; } = false;

        /// <summary>
        /// Group results by operator
        /// </summary>
        [Display(Name = "Group by Operator")]
        public bool GroupByOperator { get; set; } = false;

        /// <summary>
        /// Include trend analysis
        /// </summary>
        [Display(Name = "Include Trends")]
        public bool IncludeTrends { get; set; } = true;

        /// <summary>
        /// Include bottleneck analysis
        /// </summary>
        [Display(Name = "Include Bottlenecks")]
        public bool IncludeBottlenecks { get; set; } = true;

        /// <summary>
        /// Include operator performance comparison
        /// </summary>
        [Display(Name = "Include Performance Comparison")]
        public bool IncludePerformanceComparison { get; set; } = true;

        /// <summary>
        /// Custom filter name for saving/loading
        /// </summary>
        [Display(Name = "Filter Name")]
        [StringLength(100, ErrorMessage = "Filter name cannot exceed 100 characters")]
        public string? FilterName { get; set; }

        /// <summary>
        /// Available operators for selection
        /// </summary>
        public List<OperatorSelectionModel> AvailableOperators { get; set; } = new List<OperatorSelectionModel>();

        /// <summary>
        /// Available projects for selection
        /// </summary>
        public List<ProjectSelectionModel> AvailableProjects { get; set; } = new List<ProjectSelectionModel>();

        /// <summary>
        /// Available task statuses for selection
        /// </summary>
        public List<string> AvailableTaskStatuses { get; set; } = new List<string> { "Open", "In Progress", "Done", "Cancelled" };

        /// <summary>
        /// Available task priorities for selection
        /// </summary>
        public List<string> AvailableTaskPriorities { get; set; } = new List<string> { "Low", "Medium", "High", "Critical" };

        /// <summary>
        /// Available time granularity options
        /// </summary>
        public List<string> AvailableTimeGranularities { get; set; } = new List<string> { "daily", "weekly", "monthly" };

        /// <summary>
        /// Available sort options
        /// </summary>
        public List<SortOptionModel> AvailableSortOptions { get; set; } = new List<SortOptionModel>
        {
            new SortOptionModel { Value = "efficiency", Text = "Efficiency Rating" },
            new SortOptionModel { Value = "completion_rate", Text = "Completion Rate" },
            new SortOptionModel { Value = "tasks_completed", Text = "Tasks Completed" },
            new SortOptionModel { Value = "avg_time", Text = "Average Time" },
            new SortOptionModel { Value = "total_hours", Text = "Total Hours" }
        };

        /// <summary>
        /// Formatted display of date range
        /// </summary>
        [Display(Name = "Date Range")]
        public string DateRangeFormatted
        {
            get
            {
                if (FromDate.HasValue && ToDate.HasValue)
                    return $"{FromDate.Value:dd/MM/yyyy} - {ToDate.Value:dd/MM/yyyy}";
                if (FromDate.HasValue)
                    return $"From {FromDate.Value:dd/MM/yyyy}";
                if (ToDate.HasValue)
                    return $"Until {ToDate.Value:dd/MM/yyyy}";
                return "All Time";
            }
        }

        /// <summary>
        /// Number of days in the selected date range
        /// </summary>
        public int DateRangeDays
        {
            get
            {
                if (FromDate.HasValue && ToDate.HasValue)
                    return (ToDate.Value - FromDate.Value).Days + 1;
                return 0;
            }
        }

        /// <summary>
        /// Indicates if date range is valid
        /// </summary>
        public bool IsDateRangeValid
        {
            get
            {
                if (!FromDate.HasValue || !ToDate.HasValue)
                    return true; // No range specified is valid
                return FromDate.Value <= ToDate.Value;
            }
        }

        /// <summary>
        /// Indicates if duration range is valid
        /// </summary>
        public bool IsDurationRangeValid
        {
            get
            {
                if (!MinDurationHours.HasValue || !MaxDurationHours.HasValue)
                    return true; // No range specified is valid
                return MinDurationHours.Value <= MaxDurationHours.Value;
            }
        }

        /// <summary>
        /// Indicates if any filters are applied
        /// </summary>
        public bool HasFiltersApplied
        {
            get
            {
                return FromDate.HasValue ||
                       ToDate.HasValue ||
                       SelectedOperatorIds.Any() ||
                       SelectedProjectIds.Any() ||
                       SelectedTaskStatuses.Any() ||
                       SelectedTaskPriorities.Any() ||
                       CompletedTasksOnly ||
                       !WithTimeTrackingOnly ||
                       MinDurationHours.HasValue ||
                       MaxDurationHours.HasValue ||
                       HighVarianceTasksOnly ||
                       ExcludeOutliers;
            }
        }

        /// <summary>
        /// Summary of applied filters
        /// </summary>
        public string FilterSummary
        {
            get
            {
                var filters = new List<string>();

                if (FromDate.HasValue || ToDate.HasValue)
                    filters.Add($"Date: {DateRangeFormatted}");

                if (SelectedOperatorIds.Any())
                    filters.Add($"Operators: {SelectedOperatorIds.Count} selected");

                if (SelectedProjectIds.Any())
                    filters.Add($"Projects: {SelectedProjectIds.Count} selected");

                if (SelectedTaskStatuses.Any())
                    filters.Add($"Status: {string.Join(", ", SelectedTaskStatuses)}");

                if (SelectedTaskPriorities.Any())
                    filters.Add($"Priority: {string.Join(", ", SelectedTaskPriorities)}");

                if (CompletedTasksOnly)
                    filters.Add("Completed tasks only");

                if (HighVarianceTasksOnly)
                    filters.Add("High variance tasks only");

                if (ExcludeOutliers)
                    filters.Add("Outliers excluded");

                return filters.Any() ? string.Join(" | ", filters) : "No filters applied";
            }
        }

        /// <summary>
        /// Validates the filter model and returns validation errors
        /// </summary>
        /// <returns>List of validation error messages</returns>
        public List<string> Validate()
        {
            var errors = new List<string>();

            if (!IsDateRangeValid)
                errors.Add("From date must be earlier than or equal to To date.");

            if (!IsDurationRangeValid)
                errors.Add("Minimum duration must be less than or equal to maximum duration.");

            if (FromDate.HasValue && FromDate.Value > DateTime.Today)
                errors.Add("From date cannot be in the future.");

            if (ToDate.HasValue && ToDate.Value > DateTime.Today)
                errors.Add("To date cannot be in the future.");

            if (DateRangeDays > 365)
                errors.Add("Date range cannot exceed 365 days.");

            if (MinDurationHours.HasValue && MinDurationHours.Value < 0)
                errors.Add("Minimum duration cannot be negative.");

            if (MaxDurationHours.HasValue && MaxDurationHours.Value > 1000)
                errors.Add("Maximum duration cannot exceed 1000 hours.");

            return errors;
        }

        /// <summary>
        /// Applies default values for unspecified filters
        /// </summary>
        public void ApplyDefaults()
        {
            // Set default date range to last 30 days if not specified
            if (!FromDate.HasValue && !ToDate.HasValue)
            {
                ToDate = DateTime.Today;
                FromDate = DateTime.Today.AddDays(-30);
            }

            // Ensure time granularity is valid
            if (!AvailableTimeGranularities.Contains(TimeGranularity))
                TimeGranularity = "weekly";

            // Ensure sort criteria is valid
            if (!AvailableSortOptions.Any(s => s.Value == SortBy))
                SortBy = "efficiency";

            // Ensure sort direction is valid
            if (SortDirection != "asc" && SortDirection != "desc")
                SortDirection = "desc";
        }

        /// <summary>
        /// Creates a copy of the filter model
        /// </summary>
        /// <returns>Deep copy of the filter model</returns>
        public ReportFilterModel Clone()
        {
            return new ReportFilterModel
            {
                FromDate = FromDate,
                ToDate = ToDate,
                SelectedOperatorIds = new List<int>(SelectedOperatorIds),
                SelectedProjectIds = new List<int>(SelectedProjectIds),
                SelectedTaskStatuses = new List<string>(SelectedTaskStatuses),
                SelectedTaskPriorities = new List<string>(SelectedTaskPriorities),
                TimeGranularity = TimeGranularity,
                SortBy = SortBy,
                SortDirection = SortDirection,
                CompletedTasksOnly = CompletedTasksOnly,
                WithTimeTrackingOnly = WithTimeTrackingOnly,
                MinDurationHours = MinDurationHours,
                MaxDurationHours = MaxDurationHours,
                HighVarianceTasksOnly = HighVarianceTasksOnly,
                ExcludeOutliers = ExcludeOutliers,
                GroupByProject = GroupByProject,
                GroupByOperator = GroupByOperator,
                IncludeTrends = IncludeTrends,
                IncludeBottlenecks = IncludeBottlenecks,
                IncludePerformanceComparison = IncludePerformanceComparison,
                FilterName = FilterName
            };
        }
    }

    /// <summary>
    /// Model for operator selection in filters
    /// </summary>
    public class OperatorSelectionModel
    {
        /// <summary>
        /// Operator ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Operator name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Operator email
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if operator is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Number of tasks assigned to this operator
        /// </summary>
        public int TaskCount { get; set; }

        /// <summary>
        /// Display text for selection lists
        /// </summary>
        public string DisplayText => $"{Name} ({TaskCount} tasks)";
    }

    /// <summary>
    /// Model for project selection in filters
    /// </summary>
    public class ProjectSelectionModel
    {
        /// <summary>
        /// Project ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Project name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Project number
        /// </summary>
        public string ProjectNumber { get; set; } = string.Empty;

        /// <summary>
        /// Project status
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Number of tasks in this project
        /// </summary>
        public int TaskCount { get; set; }

        /// <summary>
        /// Display text for selection lists
        /// </summary>
        public string DisplayText => $"{ProjectNumber} - {Name} ({TaskCount} tasks)";
    }

    /// <summary>
    /// Model for sort options
    /// </summary>
    public class SortOptionModel
    {
        /// <summary>
        /// Sort option value
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Sort option display text
        /// </summary>
        public string Text { get; set; } = string.Empty;
    }
}
