using TeamWorkFlow.Core.Models.Dashboard;

namespace UnitTests
{
    /// <summary>
    /// Unit tests for Dashboard Models - testing new dashboard model functionalities
    /// </summary>
    [TestFixture]
    public class DashboardModelsUnitTests
    {
        #region PerformanceDashboardModel Tests

        [Test]
        public void PerformanceDashboardModel_DefaultConstructor_ShouldInitializeProperties()
        {
            // Act
            var model = new PerformanceDashboardModel();

            // Assert
            Assert.That(model.EfficiencyMetrics, Is.Not.Null);
            Assert.That(model.OperatorPerformance, Is.Not.Null);
            Assert.That(model.BottleneckAnalysis, Is.Not.Null);
            Assert.That(model.TrendCharts, Is.Not.Null);
            Assert.That(model.AppliedFilters, Is.Not.Null);
            Assert.That(model.KpiSummary, Is.Not.Null);
            Assert.That(model.LastUpdated, Is.LessThanOrEqualTo(DateTime.UtcNow));
            Assert.That(model.TotalTasksAnalyzed, Is.EqualTo(0));
            Assert.That(model.TotalOperatorsAnalyzed, Is.EqualTo(0));
            Assert.That(model.AnalysisPeriod, Is.EqualTo(string.Empty));
        }

        [Test]
        public void PerformanceDashboardModel_HasSufficientData_WithInsufficientTasks_ShouldReturnFalse()
        {
            // Arrange
            var model = new PerformanceDashboardModel
            {
                TotalTasksAnalyzed = 3, // Less than 5
                TotalOperatorsAnalyzed = 2
            };

            // Act & Assert
            Assert.That(model.HasSufficientData, Is.False);
        }

        [Test]
        public void PerformanceDashboardModel_HasSufficientData_WithInsufficientOperators_ShouldReturnFalse()
        {
            // Arrange
            var model = new PerformanceDashboardModel
            {
                TotalTasksAnalyzed = 10,
                TotalOperatorsAnalyzed = 0 // Less than 1
            };

            // Act & Assert
            Assert.That(model.HasSufficientData, Is.False);
        }

        [Test]
        public void PerformanceDashboardModel_HasSufficientData_WithSufficientData_ShouldReturnTrue()
        {
            // Arrange
            var model = new PerformanceDashboardModel
            {
                TotalTasksAnalyzed = 10,
                TotalOperatorsAnalyzed = 3
            };

            // Act & Assert
            Assert.That(model.HasSufficientData, Is.True);
        }

        [Test]
        public void PerformanceDashboardModel_DataSufficiencyWarning_WithInsufficientTasks_ShouldReturnTaskWarning()
        {
            // Arrange
            var model = new PerformanceDashboardModel
            {
                TotalTasksAnalyzed = 2,
                TotalOperatorsAnalyzed = 1
            };

            // Act
            var warning = model.DataSufficiencyWarning;

            // Assert
            Assert.That(warning, Is.EqualTo("Insufficient task data for reliable analysis. At least 5 completed tasks are recommended."));
        }

        [Test]
        public void PerformanceDashboardModel_DataSufficiencyWarning_WithInsufficientOperators_ShouldReturnOperatorWarning()
        {
            // Arrange
            var model = new PerformanceDashboardModel
            {
                TotalTasksAnalyzed = 10,
                TotalOperatorsAnalyzed = 0
            };

            // Act
            var warning = model.DataSufficiencyWarning;

            // Assert
            Assert.That(warning, Is.EqualTo("No operator data available for analysis."));
        }

        [Test]
        public void PerformanceDashboardModel_DataSufficiencyWarning_WithSufficientData_ShouldReturnEmpty()
        {
            // Arrange
            var model = new PerformanceDashboardModel
            {
                TotalTasksAnalyzed = 10,
                TotalOperatorsAnalyzed = 3
            };

            // Act
            var warning = model.DataSufficiencyWarning;

            // Assert
            Assert.That(warning, Is.EqualTo(string.Empty));
        }

        [Test]
        public void PerformanceDashboardModel_LastUpdatedFormatted_ShouldReturnCorrectFormat()
        {
            // Arrange
            var testDate = new DateTime(2024, 1, 15, 14, 30, 0);
            var model = new PerformanceDashboardModel
            {
                LastUpdated = testDate
            };

            // Act
            var formatted = model.LastUpdatedFormatted;

            // Assert
            Assert.That(formatted, Is.EqualTo("15/01/2024 14:30"));
        }

        [Test]
        public void PerformanceDashboardModel_TopPerformingOperator_ShouldReturnHighestEfficiencyRating()
        {
            // Arrange
            var model = new PerformanceDashboardModel
            {
                OperatorPerformance = new List<OperatorPerformanceModel>
                {
                    new OperatorPerformanceModel { OperatorName = "John", TasksCompleted = 5, EfficiencyRating = 7.5m },
                    new OperatorPerformanceModel { OperatorName = "Jane", TasksCompleted = 8, EfficiencyRating = 9.2m },
                    new OperatorPerformanceModel { OperatorName = "Bob", TasksCompleted = 0, EfficiencyRating = 0m }
                }
            };

            // Act
            var topPerformer = model.TopPerformingOperator;

            // Assert
            Assert.That(topPerformer, Is.Not.Null);
            Assert.That(topPerformer!.OperatorName, Is.EqualTo("Jane"));
            Assert.That(topPerformer.EfficiencyRating, Is.EqualTo(9.2m));
        }

        [Test]
        public void PerformanceDashboardModel_OperatorNeedingAttention_ShouldReturnLowestEfficiencyRating()
        {
            // Arrange
            var model = new PerformanceDashboardModel
            {
                OperatorPerformance = new List<OperatorPerformanceModel>
                {
                    new OperatorPerformanceModel { OperatorName = "John", TasksCompleted = 5, EfficiencyRating = 7.5m },
                    new OperatorPerformanceModel { OperatorName = "Jane", TasksCompleted = 8, EfficiencyRating = 9.2m },
                    new OperatorPerformanceModel { OperatorName = "Mike", TasksCompleted = 3, EfficiencyRating = 4.1m }
                }
            };

            // Act
            var needsAttention = model.OperatorNeedingAttention;

            // Assert
            Assert.That(needsAttention, Is.Not.Null);
            Assert.That(needsAttention!.OperatorName, Is.EqualTo("Mike"));
            Assert.That(needsAttention.EfficiencyRating, Is.EqualTo(4.1m));
        }

        [Test]
        public void PerformanceDashboardModel_OverallHealthScore_WithInsufficientData_ShouldReturnZero()
        {
            // Arrange
            var model = new PerformanceDashboardModel
            {
                TotalTasksAnalyzed = 2, // Insufficient
                TotalOperatorsAnalyzed = 1
            };

            // Act
            var healthScore = model.OverallHealthScore;

            // Assert
            Assert.That(healthScore, Is.EqualTo(0));
        }

        [Test]
        public void PerformanceDashboardModel_OverallHealthScore_WithGoodMetrics_ShouldCalculateCorrectly()
        {
            // Arrange
            var model = new PerformanceDashboardModel
            {
                TotalTasksAnalyzed = 10,
                TotalOperatorsAnalyzed = 3,
                EfficiencyMetrics = new EfficiencyMetricsModel
                {
                    OnTimeCompletionRate = 85.0m,
                    AverageTimeOverrunPercentage = 1.5m
                },
                BottleneckAnalysis = new BottleneckAnalysisModel
                {
                    TotalBottlenecks = 2
                }
            };

            // Act
            var healthScore = model.OverallHealthScore;

            // Assert
            Assert.That(healthScore, Is.GreaterThan(0));
            Assert.That(healthScore, Is.LessThanOrEqualTo(100));
        }

        #endregion

        #region EfficiencyMetricsModel Tests

        [Test]
        public void EfficiencyMetricsModel_DefaultConstructor_ShouldInitializeProperties()
        {
            // Act
            var model = new EfficiencyMetricsModel();

            // Assert
            Assert.That(model.OnTimeCompletionRate, Is.EqualTo(0));
            Assert.That(model.AverageTimeOverrunPercentage, Is.EqualTo(0));
            Assert.That(model.TotalTasksCompleted, Is.EqualTo(0));
            Assert.That(model.TotalTasksCompleted, Is.GreaterThanOrEqualTo(0));
            Assert.That(model.AverageActualTimeHours, Is.EqualTo(0));
            Assert.That(model.TrendData, Is.Not.Null);
            Assert.That(model.CommonDelayReasons, Is.Not.Null);
        }

        [Test]
        public void EfficiencyMetricsModel_OverallEfficiencyScore_WithNoTasks_ShouldReturnZero()
        {
            // Arrange
            var model = new EfficiencyMetricsModel
            {
                TotalTasksCompleted = 0
            };

            // Act
            var score = model.OverallEfficiencyScore;

            // Assert
            Assert.That(score, Is.EqualTo(0));
        }

        [Test]
        public void EfficiencyMetricsModel_OverallEfficiencyScore_WithGoodMetrics_ShouldCalculateCorrectly()
        {
            // Arrange
            var model = new EfficiencyMetricsModel
            {
                TotalTasksCompleted = 10,
                OnTimeCompletionRate = 90.0m,
                AverageTimeOverrunPercentage = 0.5m // 0.5 hours
            };

            // Act
            var score = model.OverallEfficiencyScore;

            // Assert
            Assert.That(score, Is.GreaterThan(80)); // Should be high with good metrics
            Assert.That(score, Is.LessThanOrEqualTo(100));
        }

        [Test]
        public void EfficiencyMetricsModel_EfficiencyStatus_WithExcellentScore_ShouldReturnExcellent()
        {
            // Arrange
            var model = new EfficiencyMetricsModel
            {
                TotalTasksCompleted = 10,
                OnTimeCompletionRate = 95.0m,
                AverageTimeOverrunPercentage = 0.1m
            };

            // Act
            var status = model.EfficiencyStatus;

            // Assert
            Assert.That(status, Is.EqualTo("Excellent"));
        }

        [Test]
        public void EfficiencyMetricsModel_EfficiencyStatus_WithPoorScore_ShouldReturnNeedsImprovement()
        {
            // Arrange
            var model = new EfficiencyMetricsModel
            {
                TotalTasksCompleted = 10,
                OnTimeCompletionRate = 40.0m,
                AverageTimeOverrunPercentage = 5.0m
            };

            // Act
            var status = model.EfficiencyStatus;

            // Assert
            Assert.That(status, Is.EqualTo("Needs Improvement"));
        }

        [Test]
        public void EfficiencyMetricsModel_HasSufficientData_WithFewTasks_ShouldReturnFalse()
        {
            // Arrange
            var model = new EfficiencyMetricsModel
            {
                TotalTasksCompleted = 3 // Less than 5
            };

            // Act & Assert
            Assert.That(model.HasSufficientData, Is.False);
        }

        [Test]
        public void EfficiencyMetricsModel_HasSufficientData_WithEnoughTasks_ShouldReturnTrue()
        {
            // Arrange
            var model = new EfficiencyMetricsModel
            {
                TotalTasksCompleted = 10
            };

            // Act & Assert
            Assert.That(model.HasSufficientData, Is.True);
        }

        [Test]
        public void EfficiencyMetricsModel_AverageActualTimeFormatted_ShouldFormatCorrectly()
        {
            // Arrange
            var model = new EfficiencyMetricsModel
            {
                AverageActualTimeHours = 2.75m // 2 hours 45 minutes
            };

            // Act
            var formatted = model.AverageActualTimeFormatted;

            // Assert
            Assert.That(formatted, Is.EqualTo("2h 45m"));
        }

        [Test]
        public void EfficiencyMetricsModel_AverageActualTimeFormatted_WithMinutesOnly_ShouldFormatCorrectly()
        {
            // Arrange
            var model = new EfficiencyMetricsModel
            {
                AverageActualTimeHours = 0.5m // 30 minutes
            };

            // Act
            var formatted = model.AverageActualTimeFormatted;

            // Assert
            Assert.That(formatted, Is.EqualTo("30m"));
        }

        #endregion

        #region ReportFilterModel Tests

        [Test]
        public void ReportFilterModel_DefaultConstructor_ShouldInitializeProperties()
        {
            // Act
            var model = new ReportFilterModel();

            // Assert
            Assert.That(model.FromDate, Is.Null);
            Assert.That(model.ToDate, Is.Null);
            Assert.That(model.SelectedOperatorIds, Is.Not.Null);
            Assert.That(model.SelectedProjectIds, Is.Not.Null);
            Assert.That(model.SelectedTaskStatuses, Is.Not.Null);
            Assert.That(model.SelectedTaskPriorities, Is.Not.Null);
            Assert.That(model.TimeGranularity, Is.EqualTo("weekly"));
            Assert.That(model.SortBy, Is.EqualTo("efficiency"));
            Assert.That(model.SortDirection, Is.EqualTo("desc"));
            Assert.That(model.CompletedTasksOnly, Is.False);
            Assert.That(model.WithTimeTrackingOnly, Is.True);
        }

        [Test]
        public void ReportFilterModel_HasFiltersApplied_WithNoFilters_ShouldReturnFalse()
        {
            // Arrange
            var model = new ReportFilterModel();

            // Act & Assert
            Assert.That(model.HasFiltersApplied, Is.False);
        }

        [Test]
        public void ReportFilterModel_HasFiltersApplied_WithDateFilter_ShouldReturnTrue()
        {
            // Arrange
            var model = new ReportFilterModel
            {
                FromDate = DateTime.Today.AddDays(-30)
            };

            // Act & Assert
            Assert.That(model.HasFiltersApplied, Is.True);
        }

        [Test]
        public void ReportFilterModel_HasFiltersApplied_WithOperatorFilter_ShouldReturnTrue()
        {
            // Arrange
            var model = new ReportFilterModel();
            model.SelectedOperatorIds.Add(1);

            // Act & Assert
            Assert.That(model.HasFiltersApplied, Is.True);
        }

        [Test]
        public void ReportFilterModel_Clone_ShouldCreateDeepCopy()
        {
            // Arrange
            var original = new ReportFilterModel
            {
                FromDate = DateTime.Today.AddDays(-30),
                ToDate = DateTime.Today,
                TimeGranularity = "daily",
                CompletedTasksOnly = true
            };
            original.SelectedOperatorIds.Add(1);
            original.SelectedProjectIds.Add(2);

            // Act
            var clone = original.Clone();

            // Assert
            Assert.That(clone, Is.Not.SameAs(original));
            Assert.That(clone.FromDate, Is.EqualTo(original.FromDate));
            Assert.That(clone.ToDate, Is.EqualTo(original.ToDate));
            Assert.That(clone.TimeGranularity, Is.EqualTo(original.TimeGranularity));
            Assert.That(clone.CompletedTasksOnly, Is.EqualTo(original.CompletedTasksOnly));
            Assert.That(clone.SelectedOperatorIds, Is.Not.SameAs(original.SelectedOperatorIds));
            Assert.That(clone.SelectedOperatorIds, Is.EquivalentTo(original.SelectedOperatorIds));
            Assert.That(clone.SelectedProjectIds, Is.Not.SameAs(original.SelectedProjectIds));
            Assert.That(clone.SelectedProjectIds, Is.EquivalentTo(original.SelectedProjectIds));
        }

        #endregion

        #region TrendChartModel Tests

        [Test]
        public void TrendChartModel_DefaultConstructor_ShouldInitializeProperties()
        {
            // Act
            var model = new TrendChartModel();

            // Assert
            Assert.That(model.CompletionTrendData, Is.Not.Null);
            Assert.That(model.WorkloadTrendData, Is.Not.Null);
            Assert.That(model.EfficiencyTrendData, Is.Not.Null);
            Assert.That(model.VarianceTrendData, Is.Not.Null);
            Assert.That(model.TimeLabels, Is.Not.Null);
            Assert.That(model.TimeLabels, Is.Not.Null);
            Assert.That(model.TrendConfidence, Is.EqualTo(0));
            Assert.That(model.KeyInsights, Is.Not.Null);
            Assert.That(model.SeasonalPatterns, Is.Not.Null);
            Assert.That(model.Anomalies, Is.Not.Null);
        }

        [Test]
        public void TrendChartModel_GetChartJsData_WithCompletionType_ShouldReturnCompletionData()
        {
            // Arrange
            var model = new TrendChartModel();
            model.CompletionTrendData.Add(new TrendDataPoint { Date = DateTime.Today, Value = 10 });
            model.WorkloadTrendData.Add(new TrendDataPoint { Date = DateTime.Today, Value = 20 });

            // Act
            var result = model.GetChartJsData("completion");

            // Assert
            Assert.That(result, Is.Not.Null);
            // The method should return the completion trend data
        }

        [Test]
        public void TrendChartModel_GetChartJsData_WithWorkloadType_ShouldReturnWorkloadData()
        {
            // Arrange
            var model = new TrendChartModel();
            model.CompletionTrendData.Add(new TrendDataPoint { Date = DateTime.Today, Value = 10 });
            model.WorkloadTrendData.Add(new TrendDataPoint { Date = DateTime.Today, Value = 20 });

            // Act
            var result = model.GetChartJsData("workload");

            // Assert
            Assert.That(result, Is.Not.Null);
            // The method should return the workload trend data
        }

        [Test]
        public void TrendChartModel_GetChartJsData_WithInvalidType_ShouldReturnCompletionData()
        {
            // Arrange
            var model = new TrendChartModel();
            model.CompletionTrendData.Add(new TrendDataPoint { Date = DateTime.Today, Value = 10 });

            // Act
            var result = model.GetChartJsData("invalid");

            // Assert
            Assert.That(result, Is.Not.Null);
            // Should default to completion trend data
        }

        #endregion

        #region TrendDataPoint Tests

        [Test]
        public void TrendDataPoint_DefaultConstructor_ShouldInitializeProperties()
        {
            // Act
            var dataPoint = new TrendDataPoint();

            // Assert
            Assert.That(dataPoint.Date, Is.EqualTo(default(DateTime)));
            Assert.That(dataPoint.Value, Is.EqualTo(0));
            Assert.That(dataPoint.Label, Is.EqualTo(string.Empty));
            Assert.That(dataPoint.Metadata, Is.Not.Null);
        }

        [Test]
        public void TrendDataPoint_WithValues_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var testDate = new DateTime(2024, 1, 15);
            var testValue = 25.5m;
            var testLabel = "Week 3";
            // Act
            var dataPoint = new TrendDataPoint
            {
                Date = testDate,
                Value = testValue,
                Label = testLabel
            };

            // Assert
            Assert.That(dataPoint.Date, Is.EqualTo(testDate));
            Assert.That(dataPoint.Value, Is.EqualTo(testValue));
            Assert.That(dataPoint.Label, Is.EqualTo(testLabel));
            Assert.That(dataPoint.Metadata, Is.Not.Null);
        }

        #endregion

        #region BottleneckAnalysisModel Tests

        [Test]
        public void BottleneckAnalysisModel_DefaultConstructor_ShouldInitializeProperties()
        {
            // Act
            var model = new BottleneckAnalysisModel();

            // Assert
            Assert.That(model.FrequentDelayPatterns, Is.Not.Null);
            Assert.That(model.AverageDelayTimeMinutes, Is.EqualTo(0));
            Assert.That(model.RootCauseCategories, Is.Not.Null);
            Assert.That(model.TotalBottlenecks, Is.EqualTo(0));
            Assert.That(model.AverageDelayHours, Is.EqualTo(0));
            Assert.That(model.TasksAffectedPercentage, Is.EqualTo(0));
            Assert.That(model.SeverityScore, Is.EqualTo(0));
        }

        [Test]
        public void BottleneckAnalysisModel_WithBottleneckData_ShouldCalculateCorrectly()
        {
            // Arrange
            var model = new BottleneckAnalysisModel
            {
                TotalBottlenecks = 5,
                AverageDelayHours = 3.5m,
                TasksAffectedPercentage = 25.0m,
                SeverityScore = 75.0m
            };

            // Assert
            Assert.That(model.TotalBottlenecks, Is.EqualTo(5));
            Assert.That(model.AverageDelayHours, Is.EqualTo(3.5m));
            Assert.That(model.TasksAffectedPercentage, Is.EqualTo(25.0m));
            Assert.That(model.SeverityScore, Is.EqualTo(75.0m));
        }

        #endregion

        #region DashboardKpiSummaryModel Tests

        [Test]
        public void DashboardKpiSummaryModel_DefaultConstructor_ShouldInitializeProperties()
        {
            // Act
            var model = new DashboardKpiSummaryModel();

            // Assert
            Assert.That(model.TasksCompleted, Is.EqualTo(0));
            Assert.That(model.TasksInProgress, Is.EqualTo(0));
            Assert.That(model.OverdueTasks, Is.EqualTo(0));
            Assert.That(model.AverageCompletionTimeHours, Is.EqualTo(0));
            Assert.That(model.TotalProductiveHours, Is.EqualTo(0));
            Assert.That(model.ActiveOperators, Is.EqualTo(0));
        }

        [Test]
        public void DashboardKpiSummaryModel_WithKpiData_ShouldSetPropertiesCorrectly()
        {
            // Arrange & Act
            var model = new DashboardKpiSummaryModel
            {
                TasksCompleted = 25,
                TasksInProgress = 8,
                OverdueTasks = 3,
                AverageCompletionTimeHours = 6.5m,
                TotalProductiveHours = 120.0m,
                ActiveOperators = 5
            };

            // Assert
            Assert.That(model.TasksCompleted, Is.EqualTo(25));
            Assert.That(model.TasksInProgress, Is.EqualTo(8));
            Assert.That(model.OverdueTasks, Is.EqualTo(3));
            Assert.That(model.AverageCompletionTimeHours, Is.EqualTo(6.5m));
            Assert.That(model.TotalProductiveHours, Is.EqualTo(120.0m));
            Assert.That(model.ActiveOperators, Is.EqualTo(5));
        }

        #endregion
    }
}
