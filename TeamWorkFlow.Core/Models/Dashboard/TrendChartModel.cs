using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamWorkFlow.Core.Models.Dashboard
{
    /// <summary>
    /// Model for time-series trend visualization and chart data
    /// </summary>
    public class TrendChartModel
    {
        /// <summary>
        /// Chart data points for completion trends
        /// </summary>
        [Display(Name = "Completion Trends")]
        public List<TrendDataPoint> CompletionTrendData { get; set; } = new List<TrendDataPoint>();

        /// <summary>
        /// Chart data points for workload trends
        /// </summary>
        [Display(Name = "Workload Trends")]
        public List<TrendDataPoint> WorkloadTrendData { get; set; } = new List<TrendDataPoint>();

        /// <summary>
        /// Chart data points for efficiency trends
        /// </summary>
        [Display(Name = "Efficiency Trends")]
        public List<TrendDataPoint> EfficiencyTrendData { get; set; } = new List<TrendDataPoint>();

        /// <summary>
        /// Chart data points for time variance trends
        /// </summary>
        [Display(Name = "Variance Trends")]
        public List<TrendDataPoint> VarianceTrendData { get; set; } = new List<TrendDataPoint>();

        /// <summary>
        /// Labels for chart x-axis (time periods)
        /// </summary>
        [Display(Name = "Time Labels")]
        public List<string> TimeLabels { get; set; } = new List<string>();

        /// <summary>
        /// Trend line data for completion rate
        /// </summary>
        [Display(Name = "Completion Trend Line")]
        public TrendLineModel CompletionTrendLine { get; set; } = new TrendLineModel();

        /// <summary>
        /// Trend line data for efficiency
        /// </summary>
        [Display(Name = "Efficiency Trend Line")]
        public TrendLineModel EfficiencyTrendLine { get; set; } = new TrendLineModel();

        /// <summary>
        /// Period comparison data (current vs previous period)
        /// </summary>
        [Display(Name = "Period Comparison")]
        public PeriodComparisonModel PeriodComparison { get; set; } = new PeriodComparisonModel();

        /// <summary>
        /// Chart configuration for different visualization types
        /// </summary>
        [Display(Name = "Chart Configurations")]
        public Dictionary<string, ChartConfigurationModel> ChartConfigurations { get; set; } = new Dictionary<string, ChartConfigurationModel>();

        /// <summary>
        /// Time granularity for the trend data (daily, weekly, monthly)
        /// </summary>
        [Display(Name = "Granularity")]
        public string Granularity { get; set; } = "weekly";

        /// <summary>
        /// Start date for trend analysis
        /// </summary>
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date for trend analysis
        /// </summary>
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Total number of data points in the trend
        /// </summary>
        [Display(Name = "Data Points")]
        public int TotalDataPoints { get; set; }

        /// <summary>
        /// Indicates if there's sufficient data for trend analysis
        /// </summary>
        public bool HasSufficientData => TotalDataPoints >= 2;

        /// <summary>
        /// Analysis period description
        /// </summary>
        [Display(Name = "Analysis Period")]
        public string AnalysisPeriod => $"{StartDate:dd/MM/yyyy} - {EndDate:dd/MM/yyyy}";

        /// <summary>
        /// Overall trend direction (Improving, Declining, Stable)
        /// </summary>
        [Display(Name = "Overall Trend")]
        public string OverallTrendDirection { get; set; } = "Stable";

        /// <summary>
        /// Trend confidence score (0-100)
        /// </summary>
        [Display(Name = "Trend Confidence")]
        public decimal TrendConfidence { get; set; }

        /// <summary>
        /// Key insights from trend analysis
        /// </summary>
        [Display(Name = "Key Insights")]
        public List<TrendInsightModel> KeyInsights { get; set; } = new List<TrendInsightModel>();

        /// <summary>
        /// Seasonal patterns identified in the data
        /// </summary>
        [Display(Name = "Seasonal Patterns")]
        public List<SeasonalPatternModel> SeasonalPatterns { get; set; } = new List<SeasonalPatternModel>();

        /// <summary>
        /// Anomalies detected in the trend data
        /// </summary>
        [Display(Name = "Anomalies")]
        public List<TrendAnomalyModel> Anomalies { get; set; } = new List<TrendAnomalyModel>();

        /// <summary>
        /// CSS class for overall trend direction display
        /// </summary>
        public string TrendDirectionClass
        {
            get
            {
                return OverallTrendDirection switch
                {
                    "Improving" => "text-success fw-bold",
                    "Declining" => "text-danger fw-bold",
                    "Stable" => "text-info",
                    _ => "text-muted"
                };
            }
        }

        /// <summary>
        /// Icon for overall trend direction
        /// </summary>
        public string TrendDirectionIcon
        {
            get
            {
                return OverallTrendDirection switch
                {
                    "Improving" => "fas fa-arrow-up",
                    "Declining" => "fas fa-arrow-down",
                    "Stable" => "fas fa-minus",
                    _ => "fas fa-question"
                };
            }
        }

        /// <summary>
        /// Formatted trend confidence display
        /// </summary>
        [Display(Name = "Confidence")]
        public string TrendConfidenceFormatted => $"{TrendConfidence:F0}%";

        /// <summary>
        /// CSS class for trend confidence display
        /// </summary>
        public string TrendConfidenceClass
        {
            get
            {
                if (TrendConfidence >= 80) return "text-success";
                if (TrendConfidence >= 60) return "text-warning";
                return "text-danger";
            }
        }

        /// <summary>
        /// Data sufficiency warning message
        /// </summary>
        public string DataSufficiencyWarning
        {
            get
            {
                if (!HasSufficientData)
                    return "Insufficient data points for reliable trend analysis. At least 2 periods are recommended.";
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets chart data formatted for Chart.js consumption
        /// </summary>
        /// <param name="chartType">Type of chart (completion, workload, efficiency, variance)</param>
        /// <returns>Chart.js compatible data structure</returns>
        public object GetChartJsData(string chartType)
        {
            var dataPoints = chartType.ToLower() switch
            {
                "completion" => CompletionTrendData,
                "workload" => WorkloadTrendData,
                "efficiency" => EfficiencyTrendData,
                "variance" => VarianceTrendData,
                _ => CompletionTrendData
            };

            return new
            {
                labels = TimeLabels,
                datasets = new[]
                {
                    new
                    {
                        label = GetChartLabel(chartType),
                        data = dataPoints.Select(d => d.Value).ToArray(),
                        borderColor = GetChartColor(chartType),
                        backgroundColor = GetChartBackgroundColor(chartType),
                        fill = false,
                        tension = 0.1
                    }
                }
            };
        }

        /// <summary>
        /// Gets appropriate chart label for the given type
        /// </summary>
        private string GetChartLabel(string chartType)
        {
            return chartType.ToLower() switch
            {
                "completion" => "Completion Rate (%)",
                "workload" => "Tasks Completed",
                "efficiency" => "Efficiency Rating (%)",
                "variance" => "Time Variance (%)",
                _ => "Value"
            };
        }

        /// <summary>
        /// Gets appropriate chart color for the given type
        /// </summary>
        private string GetChartColor(string chartType)
        {
            return chartType.ToLower() switch
            {
                "completion" => "#28a745",
                "workload" => "#007bff",
                "efficiency" => "#ffc107",
                "variance" => "#dc3545",
                _ => "#6c757d"
            };
        }

        /// <summary>
        /// Gets appropriate chart background color for the given type
        /// </summary>
        private string GetChartBackgroundColor(string chartType)
        {
            return chartType.ToLower() switch
            {
                "completion" => "rgba(40, 167, 69, 0.1)",
                "workload" => "rgba(0, 123, 255, 0.1)",
                "efficiency" => "rgba(255, 193, 7, 0.1)",
                "variance" => "rgba(220, 53, 69, 0.1)",
                _ => "rgba(108, 117, 125, 0.1)"
            };
        }
    }

    /// <summary>
    /// Model for individual trend data points
    /// </summary>
    public class TrendDataPoint
    {
        /// <summary>
        /// Date for this data point
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Value for this data point
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// Label for this data point
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Additional metadata for this point
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Formatted date for display
        /// </summary>
        public string DateFormatted => Date.ToString("dd/MM/yyyy");

        /// <summary>
        /// Formatted value for display
        /// </summary>
        public string ValueFormatted => $"{Value:F1}";
    }

    /// <summary>
    /// Model for trend line calculations
    /// </summary>
    public class TrendLineModel
    {
        /// <summary>
        /// Slope of the trend line
        /// </summary>
        public decimal Slope { get; set; }

        /// <summary>
        /// Y-intercept of the trend line
        /// </summary>
        public decimal Intercept { get; set; }

        /// <summary>
        /// R-squared value for trend line fit (0-1)
        /// </summary>
        public decimal RSquared { get; set; }

        /// <summary>
        /// Trend direction (Positive, Negative, Flat)
        /// </summary>
        public string Direction { get; set; } = "Flat";

        /// <summary>
        /// Strength of the trend (Weak, Moderate, Strong)
        /// </summary>
        public string Strength { get; set; } = "Weak";

        /// <summary>
        /// Projected value for next period
        /// </summary>
        public decimal ProjectedNextValue { get; set; }

        /// <summary>
        /// Confidence interval for projection
        /// </summary>
        public decimal ProjectionConfidenceInterval { get; set; }

        /// <summary>
        /// Formatted R-squared display
        /// </summary>
        public string RSquaredFormatted => $"{RSquared:F3}";

        /// <summary>
        /// CSS class for trend direction
        /// </summary>
        public string DirectionClass
        {
            get
            {
                return Direction switch
                {
                    "Positive" => "text-success",
                    "Negative" => "text-danger",
                    "Flat" => "text-info",
                    _ => "text-muted"
                };
            }
        }
    }

    /// <summary>
    /// Model for period-over-period comparison
    /// </summary>
    public class PeriodComparisonModel
    {
        /// <summary>
        /// Current period label
        /// </summary>
        public string CurrentPeriod { get; set; } = string.Empty;

        /// <summary>
        /// Previous period label
        /// </summary>
        public string PreviousPeriod { get; set; } = string.Empty;

        /// <summary>
        /// Current period value
        /// </summary>
        public decimal CurrentValue { get; set; }

        /// <summary>
        /// Previous period value
        /// </summary>
        public decimal PreviousValue { get; set; }

        /// <summary>
        /// Change amount (current - previous)
        /// </summary>
        public decimal Change { get; set; }

        /// <summary>
        /// Change percentage
        /// </summary>
        public decimal ChangePercentage { get; set; }

        /// <summary>
        /// Comparison metrics by category
        /// </summary>
        public Dictionary<string, PeriodMetricComparison> MetricComparisons { get; set; } = new Dictionary<string, PeriodMetricComparison>();

        /// <summary>
        /// Formatted change display
        /// </summary>
        public string ChangeFormatted
        {
            get
            {
                var sign = Change >= 0 ? "+" : "";
                return $"{sign}{Change:F1}";
            }
        }

        /// <summary>
        /// Formatted change percentage display
        /// </summary>
        public string ChangePercentageFormatted
        {
            get
            {
                var sign = ChangePercentage >= 0 ? "+" : "";
                return $"{sign}{ChangePercentage:F1}%";
            }
        }

        /// <summary>
        /// CSS class for change display
        /// </summary>
        public string ChangeClass
        {
            get
            {
                if (Change > 0) return "text-success";
                if (Change < 0) return "text-danger";
                return "text-muted";
            }
        }
    }

    /// <summary>
    /// Model for individual metric comparison
    /// </summary>
    public class PeriodMetricComparison
    {
        /// <summary>
        /// Metric name
        /// </summary>
        public string MetricName { get; set; } = string.Empty;

        /// <summary>
        /// Current period value
        /// </summary>
        public decimal CurrentValue { get; set; }

        /// <summary>
        /// Previous period value
        /// </summary>
        public decimal PreviousValue { get; set; }

        /// <summary>
        /// Change percentage
        /// </summary>
        public decimal ChangePercentage { get; set; }

        /// <summary>
        /// Improvement indicator
        /// </summary>
        public bool IsImprovement { get; set; }
    }

    /// <summary>
    /// Model for chart configuration
    /// </summary>
    public class ChartConfigurationModel
    {
        /// <summary>
        /// Chart type (line, bar, area, pie)
        /// </summary>
        public string ChartType { get; set; } = "line";

        /// <summary>
        /// Chart title
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Y-axis label
        /// </summary>
        public string YAxisLabel { get; set; } = string.Empty;

        /// <summary>
        /// X-axis label
        /// </summary>
        public string XAxisLabel { get; set; } = string.Empty;

        /// <summary>
        /// Chart colors
        /// </summary>
        public List<string> Colors { get; set; } = new List<string>();

        /// <summary>
        /// Chart options for Chart.js
        /// </summary>
        public Dictionary<string, object> Options { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Model for trend insights
    /// </summary>
    public class TrendInsightModel
    {
        /// <summary>
        /// Insight title
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Insight description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Insight type (Positive, Negative, Neutral)
        /// </summary>
        public string Type { get; set; } = "Neutral";

        /// <summary>
        /// Confidence level (0-100)
        /// </summary>
        public decimal Confidence { get; set; }

        /// <summary>
        /// Related time period
        /// </summary>
        public string TimePeriod { get; set; } = string.Empty;

        /// <summary>
        /// CSS class for insight type
        /// </summary>
        public string TypeClass
        {
            get
            {
                return Type switch
                {
                    "Positive" => "alert alert-success",
                    "Negative" => "alert alert-danger",
                    "Neutral" => "alert alert-info",
                    _ => "alert alert-secondary"
                };
            }
        }
    }

    /// <summary>
    /// Model for seasonal patterns
    /// </summary>
    public class SeasonalPatternModel
    {
        /// <summary>
        /// Pattern name
        /// </summary>
        public string PatternName { get; set; } = string.Empty;

        /// <summary>
        /// Pattern description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Recurring period (daily, weekly, monthly)
        /// </summary>
        public string RecurringPeriod { get; set; } = string.Empty;

        /// <summary>
        /// Pattern strength (0-100)
        /// </summary>
        public decimal Strength { get; set; }

        /// <summary>
        /// Time periods when pattern occurs
        /// </summary>
        public List<string> OccurrencePeriods { get; set; } = new List<string>();
    }

    /// <summary>
    /// Model for trend anomalies
    /// </summary>
    public class TrendAnomalyModel
    {
        /// <summary>
        /// Date of the anomaly
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Anomaly description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Severity level (Low, Medium, High)
        /// </summary>
        public string Severity { get; set; } = "Medium";

        /// <summary>
        /// Expected value
        /// </summary>
        public decimal ExpectedValue { get; set; }

        /// <summary>
        /// Actual value
        /// </summary>
        public decimal ActualValue { get; set; }

        /// <summary>
        /// Deviation percentage
        /// </summary>
        public decimal DeviationPercentage { get; set; }

        /// <summary>
        /// Possible causes
        /// </summary>
        public List<string> PossibleCauses { get; set; } = new List<string>();

        /// <summary>
        /// Formatted date display
        /// </summary>
        public string DateFormatted => Date.ToString("dd/MM/yyyy");

        /// <summary>
        /// CSS class for severity display
        /// </summary>
        public string SeverityClass
        {
            get
            {
                return Severity switch
                {
                    "High" => "badge bg-danger",
                    "Medium" => "badge bg-warning",
                    "Low" => "badge bg-info",
                    _ => "badge bg-secondary"
                };
            }
        }
    }
}
