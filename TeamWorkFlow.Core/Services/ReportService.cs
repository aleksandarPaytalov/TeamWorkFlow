using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Text;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Dashboard;

namespace TeamWorkFlow.Core.Services
{
    /// <summary>
    /// Service for generating PDF and Excel reports from dashboard data
    /// </summary>
    public class ReportService : IReportService
    {
        private readonly ILogger<ReportService> _logger;

        public ReportService(ILogger<ReportService> logger)
        {
            _logger = logger;
            
            // Set EPPlus license context for non-commercial use
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        /// <summary>
        /// Generates a PDF report from dashboard data
        /// </summary>
        public async Task<byte[]> GeneratePdfReportAsync(PerformanceDashboardModel dashboardData, ReportFilterModel filters)
        {
            try
            {
                _logger.LogInformation("Starting PDF report generation");

                using var memoryStream = new MemoryStream();
                var document = new Document(PageSize.A4, 50, 50, 50, 50);
                var writer = PdfWriter.GetInstance(document, memoryStream);

                document.Open();

                // Add title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                var title = new Paragraph("Performance Dashboard Report", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20
                };
                document.Add(title);

                // Add report period
                var periodFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                var period = new Paragraph($"Report Period: {filters.FromDate:dd/MM/yyyy} - {filters.ToDate:dd/MM/yyyy}", periodFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 30
                };
                document.Add(period);

                // Add efficiency metrics section
                await AddEfficiencyMetricsSection(document, dashboardData.EfficiencyMetrics);

                // Add operator performance section
                await AddOperatorPerformanceSection(document, dashboardData.OperatorPerformance);

                // Add bottleneck analysis section
                await AddBottleneckAnalysisSection(document, dashboardData.BottleneckAnalysis);

                // Add summary section
                await AddSummarySection(document, dashboardData, filters);

                document.Close();

                _logger.LogInformation("PDF report generated successfully");
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF report");
                throw;
            }
        }

        /// <summary>
        /// Generates an Excel report from dashboard data
        /// </summary>
        public async Task<byte[]> GenerateExcelReportAsync(PerformanceDashboardModel dashboardData, ReportFilterModel filters)
        {
            try
            {
                _logger.LogInformation("Starting Excel report generation");

                using var package = new ExcelPackage();

                // Create worksheets
                await CreateSummaryWorksheet(package, dashboardData, filters);
                await CreateEfficiencyWorksheet(package, dashboardData.EfficiencyMetrics);
                await CreateOperatorWorksheet(package, dashboardData.OperatorPerformance);
                await CreateBottleneckWorksheet(package, dashboardData.BottleneckAnalysis);
                await CreateTrendsWorksheet(package, dashboardData.TrendCharts);

                _logger.LogInformation("Excel report generated successfully");
                return package.GetAsByteArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Excel report");
                throw;
            }
        }

        /// <summary>
        /// Generates a summary report with key metrics
        /// </summary>
        public async Task<string> GenerateSummaryReportAsync(PerformanceDashboardModel dashboardData, ReportFilterModel filters)
        {
            try
            {
                var summary = new StringBuilder();
                
                summary.AppendLine("PERFORMANCE DASHBOARD SUMMARY REPORT");
                summary.AppendLine("=====================================");
                summary.AppendLine();
                summary.AppendLine($"Report Period: {filters.FromDate:dd/MM/yyyy} - {filters.ToDate:dd/MM/yyyy}");
                summary.AppendLine($"Generated: {DateTime.UtcNow:dd/MM/yyyy HH:mm} UTC");
                summary.AppendLine();

                // Efficiency metrics summary
                summary.AppendLine("EFFICIENCY METRICS:");
                summary.AppendLine($"- On-Time Completion Rate: {dashboardData.EfficiencyMetrics.OnTimeCompletionRate:F1}%");
                summary.AppendLine($"- Average Time Overrun: {dashboardData.EfficiencyMetrics.AverageTimeOverrunPercentage:F1}%");
                summary.AppendLine($"- Tasks Completed: {dashboardData.EfficiencyMetrics.TotalTasksCompleted}");
                summary.AppendLine($"- Overall Efficiency Score: {dashboardData.EfficiencyMetrics.OverallEfficiencyScore:F1}");
                summary.AppendLine();

                // Operator performance summary
                summary.AppendLine("OPERATOR PERFORMANCE:");
                summary.AppendLine($"- Total Operators: {dashboardData.OperatorPerformance.Count}");
                summary.AppendLine($"- Top Performers: {dashboardData.OperatorPerformance.Count(o => o.IsTopPerformer)}");
                summary.AppendLine($"- Need Attention: {dashboardData.OperatorPerformance.Count(o => o.NeedsAttention)}");
                if (dashboardData.OperatorPerformance.Any())
                {
                    summary.AppendLine($"- Average Team Efficiency: {dashboardData.OperatorPerformance.Average(o => o.EfficiencyRating):F1}%");
                }
                summary.AppendLine();

                // Bottleneck analysis summary
                summary.AppendLine("BOTTLENECK ANALYSIS:");
                summary.AppendLine($"- Total Bottlenecks: {dashboardData.BottleneckAnalysis.TotalBottlenecks}");
                summary.AppendLine($"- Average Delay Time: {dashboardData.BottleneckAnalysis.AverageDelayHours:F1} hours");
                summary.AppendLine($"- Tasks Affected: {dashboardData.BottleneckAnalysis.TasksAffectedPercentage:F1}%");
                summary.AppendLine($"- Severity Score: {dashboardData.BottleneckAnalysis.SeverityScoreFormatted}");

                return await Task.FromResult(summary.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating summary report");
                throw;
            }
        }

        /// <summary>
        /// Validates that the dashboard data is suitable for report generation
        /// </summary>
        public bool ValidateReportData(PerformanceDashboardModel dashboardData)
        {
            if (dashboardData == null)
            {
                _logger.LogWarning("Dashboard data is null");
                return false;
            }

            if (dashboardData.EfficiencyMetrics == null)
            {
                _logger.LogWarning("Efficiency metrics data is null");
                return false;
            }

            if (dashboardData.OperatorPerformance == null)
            {
                _logger.LogWarning("Operator performance data is null");
                return false;
            }

            if (dashboardData.BottleneckAnalysis == null)
            {
                _logger.LogWarning("Bottleneck analysis data is null");
                return false;
            }

            return true;
        }

        #region Private PDF Helper Methods

        private async Task AddEfficiencyMetricsSection(Document document, EfficiencyMetricsModel metrics)
        {
            var sectionFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
            var section = new Paragraph("Efficiency Metrics", sectionFont) { SpacingBefore = 20, SpacingAfter = 10 };
            document.Add(section);

            var table = new PdfPTable(2) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 3, 1 });

            // Add metrics data
            AddTableRow(table, "On-Time Completion Rate", $"{metrics.OnTimeCompletionRate:F1}%");
            AddTableRow(table, "Average Time Overrun", $"{metrics.AverageTimeOverrunPercentage:F1}%");
            AddTableRow(table, "Tasks Completed", metrics.TotalTasksCompleted.ToString());
            AddTableRow(table, "Overall Efficiency Score", $"{metrics.OverallEfficiencyScore:F1}");
            AddTableRow(table, "Average Actual Time", $"{metrics.AverageActualTimeHours:F1} hours");
            AddTableRow(table, "Average Estimated Time", $"{metrics.AverageEstimatedTimeHours:F1} hours");

            document.Add(table);
            await Task.CompletedTask;
        }

        private async Task AddOperatorPerformanceSection(Document document, List<OperatorPerformanceModel> operators)
        {
            var sectionFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
            var section = new Paragraph("Operator Performance", sectionFont) { SpacingBefore = 20, SpacingAfter = 10 };
            document.Add(section);

            if (!operators.Any())
            {
                document.Add(new Paragraph("No operator data available."));
                return;
            }

            var table = new PdfPTable(4) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 3, 1, 1, 1 });

            // Add headers
            AddTableHeader(table, "Operator");
            AddTableHeader(table, "Efficiency");
            AddTableHeader(table, "Tasks");
            AddTableHeader(table, "Avg Time");

            // Add operator data
            foreach (var op in operators.Take(10)) // Limit to top 10 for PDF
            {
                AddTableRow(table, op.OperatorName, $"{op.EfficiencyRating:F1}%", 
                           op.TasksCompleted.ToString(), $"{op.AverageCompletionTimeHours:F1}h");
            }

            document.Add(table);
            await Task.CompletedTask;
        }

        private async Task AddBottleneckAnalysisSection(Document document, BottleneckAnalysisModel bottlenecks)
        {
            var sectionFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
            var section = new Paragraph("Bottleneck Analysis", sectionFont) { SpacingBefore = 20, SpacingAfter = 10 };
            document.Add(section);

            var table = new PdfPTable(2) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 3, 1 });

            AddTableRow(table, "Total Bottlenecks", bottlenecks.TotalBottlenecks.ToString());
            AddTableRow(table, "Average Delay Time", $"{bottlenecks.AverageDelayHours:F1} hours");
            AddTableRow(table, "Tasks Affected", $"{bottlenecks.TasksAffectedPercentage:F1}%");
            AddTableRow(table, "Severity Score", bottlenecks.SeverityScoreFormatted);

            document.Add(table);
            await Task.CompletedTask;
        }

        private async Task AddSummarySection(Document document, PerformanceDashboardModel dashboardData, ReportFilterModel filters)
        {
            var sectionFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
            var section = new Paragraph("Report Summary", sectionFont) { SpacingBefore = 20, SpacingAfter = 10 };
            document.Add(section);

            var summary = await GenerateSummaryReportAsync(dashboardData, filters);
            var summaryParagraph = new Paragraph(summary, FontFactory.GetFont(FontFactory.HELVETICA, 10));
            document.Add(summaryParagraph);
        }

        private void AddTableHeader(PdfPTable table, string text)
        {
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
            var cell = new PdfPCell(new Phrase(text, headerFont))
            {
                Padding = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            table.AddCell(cell);
        }

        private void AddTableRow(PdfPTable table, params string[] values)
        {
            var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
            foreach (var value in values)
            {
                var cell = new PdfPCell(new Phrase(value, cellFont))
                {
                    Padding = 5,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
            }
        }

        #endregion

        #region Private Excel Helper Methods

        private async Task CreateSummaryWorksheet(ExcelPackage package, PerformanceDashboardModel dashboardData, ReportFilterModel filters)
        {
            var worksheet = package.Workbook.Worksheets.Add("Summary");

            // Title
            worksheet.Cells[1, 1].Value = "Performance Dashboard Summary";
            worksheet.Cells[1, 1].Style.Font.Size = 16;
            worksheet.Cells[1, 1].Style.Font.Bold = true;

            // Report period
            worksheet.Cells[2, 1].Value = $"Report Period: {filters.FromDate:dd/MM/yyyy} - {filters.ToDate:dd/MM/yyyy}";
            worksheet.Cells[3, 1].Value = $"Generated: {DateTime.UtcNow:dd/MM/yyyy HH:mm} UTC";

            // Key metrics
            var row = 5;
            worksheet.Cells[row, 1].Value = "Key Performance Indicators";
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            row++;

            worksheet.Cells[row, 1].Value = "On-Time Completion Rate";
            worksheet.Cells[row, 2].Value = dashboardData.EfficiencyMetrics.OnTimeCompletionRate / 100;
            worksheet.Cells[row, 2].Style.Numberformat.Format = "0.0%";
            row++;

            worksheet.Cells[row, 1].Value = "Average Time Overrun";
            worksheet.Cells[row, 2].Value = dashboardData.EfficiencyMetrics.AverageTimeOverrunPercentage / 100;
            worksheet.Cells[row, 2].Style.Numberformat.Format = "0.0%";
            row++;

            worksheet.Cells[row, 1].Value = "Tasks Completed";
            worksheet.Cells[row, 2].Value = dashboardData.EfficiencyMetrics.TotalTasksCompleted;
            row++;

            worksheet.Cells[row, 1].Value = "Overall Efficiency Score";
            worksheet.Cells[row, 2].Value = dashboardData.EfficiencyMetrics.OverallEfficiencyScore;
            worksheet.Cells[row, 2].Style.Numberformat.Format = "0.0";

            // Auto-fit columns
            worksheet.Cells.AutoFitColumns();
            await Task.CompletedTask;
        }

        private async Task CreateEfficiencyWorksheet(ExcelPackage package, EfficiencyMetricsModel metrics)
        {
            var worksheet = package.Workbook.Worksheets.Add("Efficiency Metrics");

            // Headers
            worksheet.Cells[1, 1].Value = "Metric";
            worksheet.Cells[1, 2].Value = "Value";
            worksheet.Cells[1, 1, 1, 2].Style.Font.Bold = true;
            worksheet.Cells[1, 1, 1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[1, 1, 1, 2].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

            // Data
            var row = 2;
            worksheet.Cells[row, 1].Value = "On-Time Completion Rate";
            worksheet.Cells[row, 2].Value = metrics.OnTimeCompletionRate / 100;
            worksheet.Cells[row, 2].Style.Numberformat.Format = "0.0%";
            row++;

            worksheet.Cells[row, 1].Value = "Average Time Overrun Percentage";
            worksheet.Cells[row, 2].Value = metrics.AverageTimeOverrunPercentage / 100;
            worksheet.Cells[row, 2].Style.Numberformat.Format = "0.0%";
            row++;

            worksheet.Cells[row, 1].Value = "Total Tasks Completed";
            worksheet.Cells[row, 2].Value = metrics.TotalTasksCompleted;
            row++;

            worksheet.Cells[row, 1].Value = "Overall Efficiency Score";
            worksheet.Cells[row, 2].Value = metrics.OverallEfficiencyScore;
            worksheet.Cells[row, 2].Style.Numberformat.Format = "0.0";
            row++;

            worksheet.Cells[row, 1].Value = "Average Actual Time (Hours)";
            worksheet.Cells[row, 2].Value = metrics.AverageActualTimeHours;
            worksheet.Cells[row, 2].Style.Numberformat.Format = "0.0";
            row++;

            worksheet.Cells[row, 1].Value = "Average Estimated Time (Hours)";
            worksheet.Cells[row, 2].Value = metrics.AverageEstimatedTimeHours;
            worksheet.Cells[row, 2].Style.Numberformat.Format = "0.0";

            // Create a chart
            var chart = worksheet.Drawings.AddChart("EfficiencyChart", eChartType.ColumnClustered);
            chart.Title.Text = "Efficiency Metrics";
            chart.SetPosition(1, 0, 4, 0);
            chart.SetSize(400, 300);

            var series = chart.Series.Add(worksheet.Cells[2, 2, row, 2], worksheet.Cells[2, 1, row, 1]);
            series.Header = "Values";

            worksheet.Cells.AutoFitColumns();
            await Task.CompletedTask;
        }

        private async Task CreateOperatorWorksheet(ExcelPackage package, List<OperatorPerformanceModel> operators)
        {
            var worksheet = package.Workbook.Worksheets.Add("Operator Performance");

            // Headers
            var headers = new[] { "Operator Name", "Email", "Efficiency Rating (%)", "Tasks Completed", "Avg Completion Time (h)", "Rank", "Status" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
            }
            worksheet.Cells[1, 1, 1, headers.Length].Style.Font.Bold = true;
            worksheet.Cells[1, 1, 1, headers.Length].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[1, 1, 1, headers.Length].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

            // Data
            var row = 2;
            foreach (var op in operators)
            {
                worksheet.Cells[row, 1].Value = op.OperatorName;
                worksheet.Cells[row, 2].Value = op.OperatorEmail;
                worksheet.Cells[row, 3].Value = op.EfficiencyRating;
                worksheet.Cells[row, 3].Style.Numberformat.Format = "0.0";
                worksheet.Cells[row, 4].Value = op.TasksCompleted;
                worksheet.Cells[row, 5].Value = op.AverageCompletionTimeHours;
                worksheet.Cells[row, 5].Style.Numberformat.Format = "0.0";
                worksheet.Cells[row, 6].Value = op.Rank;
                worksheet.Cells[row, 7].Value = op.IsTopPerformer ? "Top Performer" : op.NeedsAttention ? "Needs Attention" : "Average";

                // Color coding
                if (op.IsTopPerformer)
                {
                    worksheet.Cells[row, 1, row, headers.Length].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 1, row, headers.Length].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                }
                else if (op.NeedsAttention)
                {
                    worksheet.Cells[row, 1, row, headers.Length].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 1, row, headers.Length].Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
                }

                row++;
            }

            // Create performance chart
            if (operators.Any())
            {
                var chart = worksheet.Drawings.AddChart("PerformanceChart", eChartType.ColumnClustered);
                chart.Title.Text = "Operator Efficiency Ratings";
                chart.SetPosition(1, 0, headers.Length + 2, 0);
                chart.SetSize(600, 400);

                var series = chart.Series.Add(worksheet.Cells[2, 3, row - 1, 3], worksheet.Cells[2, 1, row - 1, 1]);
                series.Header = "Efficiency Rating (%)";
            }

            worksheet.Cells.AutoFitColumns();
            await Task.CompletedTask;
        }

        private async Task CreateBottleneckWorksheet(ExcelPackage package, BottleneckAnalysisModel bottlenecks)
        {
            var worksheet = package.Workbook.Worksheets.Add("Bottleneck Analysis");

            // Summary section
            worksheet.Cells[1, 1].Value = "Bottleneck Summary";
            worksheet.Cells[1, 1].Style.Font.Size = 14;
            worksheet.Cells[1, 1].Style.Font.Bold = true;

            var row = 3;
            worksheet.Cells[row, 1].Value = "Total Bottlenecks";
            worksheet.Cells[row, 2].Value = bottlenecks.TotalBottlenecks;
            row++;

            worksheet.Cells[row, 1].Value = "Average Delay Time (Hours)";
            worksheet.Cells[row, 2].Value = bottlenecks.AverageDelayHours;
            worksheet.Cells[row, 2].Style.Numberformat.Format = "0.0";
            row++;

            worksheet.Cells[row, 1].Value = "Tasks Affected (%)";
            worksheet.Cells[row, 2].Value = bottlenecks.TasksAffectedPercentage / 100;
            worksheet.Cells[row, 2].Style.Numberformat.Format = "0.0%";
            row++;

            worksheet.Cells[row, 1].Value = "Severity Score";
            worksheet.Cells[row, 2].Value = bottlenecks.SeverityScoreFormatted;

            // Frequent bottlenecks section
            row += 3;
            worksheet.Cells[row, 1].Value = "Frequent Bottlenecks";
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            row++;

            if (bottlenecks.FrequentBottleneckTasks?.Any() == true)
            {
                // Headers for bottleneck tasks
                var headers = new[] { "Task Name", "Project", "Occurrences", "Avg Delay (h)", "Total Delay (h)" };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[row, i + 1].Value = headers[i];
                }
                worksheet.Cells[row, 1, row, headers.Length].Style.Font.Bold = true;
                worksheet.Cells[row, 1, row, headers.Length].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, 1, row, headers.Length].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                row++;

                foreach (var task in bottlenecks.FrequentBottleneckTasks.Take(20))
                {
                    worksheet.Cells[row, 1].Value = task.TaskName;
                    worksheet.Cells[row, 2].Value = task.ProjectName;
                    worksheet.Cells[row, 3].Value = task.Occurrences;
                    worksheet.Cells[row, 4].Value = task.AverageDelayHours;
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "0.0";
                    worksheet.Cells[row, 5].Value = task.TotalDelayHours;
                    worksheet.Cells[row, 5].Style.Numberformat.Format = "0.0";
                    row++;
                }
            }

            worksheet.Cells.AutoFitColumns();
            await Task.CompletedTask;
        }

        private async Task CreateTrendsWorksheet(ExcelPackage package, TrendChartModel trends)
        {
            var worksheet = package.Workbook.Worksheets.Add("Trends");

            // Title
            worksheet.Cells[1, 1].Value = "Performance Trends";
            worksheet.Cells[1, 1].Style.Font.Size = 14;
            worksheet.Cells[1, 1].Style.Font.Bold = true;

            // Trend overview
            var row = 3;
            worksheet.Cells[row, 1].Value = "Trend Overview";
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            row++;

            worksheet.Cells[row, 1].Value = "Overall Trend Direction";
            worksheet.Cells[row, 2].Value = trends.OverallTrendDirection;
            row++;

            worksheet.Cells[row, 1].Value = "Trend Confidence";
            worksheet.Cells[row, 2].Value = trends.TrendConfidence / 100;
            worksheet.Cells[row, 2].Style.Numberformat.Format = "0.0%";
            row++;

            worksheet.Cells[row, 1].Value = "Total Data Points";
            worksheet.Cells[row, 2].Value = trends.TotalDataPoints;
            row++;

            worksheet.Cells[row, 1].Value = "Analysis Period";
            worksheet.Cells[row, 2].Value = trends.AnalysisPeriod;

            // Trend data section
            row += 3;
            if (trends.CompletionTrendData?.Any() == true)
            {
                worksheet.Cells[row, 1].Value = "Completion Trend Data";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                row++;

                // Headers
                worksheet.Cells[row, 1].Value = "Date";
                worksheet.Cells[row, 2].Value = "Completion Rate (%)";
                worksheet.Cells[row, 1, row, 2].Style.Font.Bold = true;
                worksheet.Cells[row, 1, row, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, 1, row, 2].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                row++;

                foreach (var dataPoint in trends.CompletionTrendData.Take(50))
                {
                    worksheet.Cells[row, 1].Value = dataPoint.Label;
                    worksheet.Cells[row, 2].Value = dataPoint.Value;
                    worksheet.Cells[row, 2].Style.Numberformat.Format = "0.0";
                    row++;
                }

                // Create trend chart
                var chart = worksheet.Drawings.AddChart("TrendChart", eChartType.Line);
                chart.Title.Text = "Completion Rate Trend";
                chart.SetPosition(1, 0, 4, 0);
                chart.SetSize(600, 400);

                var startRow = row - trends.CompletionTrendData.Count;
                var series = chart.Series.Add(worksheet.Cells[startRow, 2, row - 1, 2], worksheet.Cells[startRow, 1, row - 1, 1]);
                series.Header = "Completion Rate (%)";
            }

            worksheet.Cells.AutoFitColumns();
            await Task.CompletedTask;
        }

        #endregion
    }
}
