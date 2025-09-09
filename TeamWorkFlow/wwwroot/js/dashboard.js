// Dashboard JavaScript
(function () {
  "use strict";

  // Global Chart.js reference (loaded externally)
  /* global Chart */

  // Dashboard state
  let charts = {};
  let currentFilters = {};

  // Initialize dashboard when DOM is loaded
  document.addEventListener("DOMContentLoaded", function () {
    // Prevent duplicate initialization
    if (window.DashboardInitialized) {
      console.log("Dashboard already initialized, skipping...");
      return;
    }

    // Check if Chart.js is loaded
    if (typeof window.Chart === "undefined") {
      console.warn("Chart.js not loaded yet, waiting...");
      // Wait for Chart.js to load
      setTimeout(() => {
        if (typeof window.Chart !== "undefined") {
          console.log("Chart.js loaded, initializing dashboard...");
          initializeDashboard();
          setupEventListeners();
          loadInitialData();
          window.DashboardInitialized = true;
        } else {
          console.error(
            "Chart.js failed to load. Charts will not be available."
          );
          // Initialize without charts
          initializeDashboard();
          setupEventListeners();
          window.DashboardInitialized = true;
        }
      }, 1000);
    } else {
      initializeDashboard();
      setupEventListeners();
      loadInitialData();
      window.DashboardInitialized = true;
    }
  });

  function initializeDashboard() {
    console.log("Initializing Performance Dashboard...");

    // Initialize date inputs with default values if empty
    initializeDateInputs();

    // Get current filter values from the form
    currentFilters = {
      fromDate: document.getElementById("fromDate")?.value || null,
      toDate: document.getElementById("toDate")?.value || null,
      granularity: document.getElementById("granularity")?.value || "weekly",
    };
  }

  function setupEventListeners() {
    // Filter controls
    const applyFiltersBtn = document.getElementById("apply-filters");
    const refreshDataBtn = document.getElementById("refresh-data");
    const retryLoadBtn = document.getElementById("retry-load");

    if (applyFiltersBtn) {
      applyFiltersBtn.addEventListener("click", handleApplyFilters);
    }

    if (refreshDataBtn) {
      refreshDataBtn.addEventListener("click", handleRefreshData);
    }

    if (retryLoadBtn) {
      retryLoadBtn.addEventListener("click", handleRetryLoad);
    }

    // Section refresh buttons
    const refreshButtons = document.querySelectorAll('[id^="refresh-"]');
    refreshButtons.forEach((button) => {
      button.addEventListener("click", function () {
        const sectionType = this.id.replace("refresh-", "");
        refreshSection(sectionType);
      });
    });

    // Operator sorting
    const operatorSort = document.getElementById("operator-sort");
    if (operatorSort) {
      operatorSort.addEventListener("change", function () {
        refreshSection("operators", { sortBy: this.value });
      });
    }

    // Export buttons (Admin only)
    const exportPdfBtn = document.getElementById("export-pdf");
    const exportExcelBtn = document.getElementById("export-excel");

    if (exportPdfBtn) {
      exportPdfBtn.addEventListener("click", handleExportPdf);
    }

    if (exportExcelBtn) {
      exportExcelBtn.addEventListener("click", handleExportExcel);
    }
  }

  function loadInitialData() {
    const startTime = performance.now();

    try {
      // Initial data is loaded server-side, but we can initialize charts here
      initializeCharts();

      // Initialize auto-refresh
      initializeAutoRefresh();

      // Initialize accessibility features
      initializeAccessibility();

      // Track performance
      trackPerformance("initialization", startTime);
    } catch (error) {
      handleError(error, "initialization");
    }
  }

  function handleApplyFilters() {
    showLoading();

    // Get filter values
    const fromDateDisplay = document.getElementById("fromDate")?.value?.trim();
    const toDateDisplay = document.getElementById("toDate")?.value?.trim();
    const granularity = document.getElementById("granularity")?.value;

    // Convert display format to server format (only if values exist)
    const fromDate = fromDateDisplay
      ? convertToServerFormat(fromDateDisplay)
      : "";
    const toDate = toDateDisplay ? convertToServerFormat(toDateDisplay) : "";

    // Validate date range (only if both dates are provided)
    if (fromDate && toDate) {
      const fromDateObj = new Date(fromDate);
      const toDateObj = new Date(toDate);
      if (fromDateObj > toDateObj) {
        hideLoading();
        showError("From date cannot be later than to date.");
        return;
      }
    }

    // Update current filters
    currentFilters = { fromDate, toDate, granularity };

    // Reload page with new filters (only include non-empty values)
    const params = new URLSearchParams();
    if (fromDate) params.append("fromDate", fromDate);
    if (toDate) params.append("toDate", toDate);
    if (granularity) params.append("granularity", granularity);

    window.location.href = `/Dashboard?${params.toString()}`;
  }

  function handleRefreshData() {
    const startTime = performance.now();
    showLoading();

    // Refresh all sections
    Promise.all([
      refreshSection("efficiency"),
      refreshSection("operators"),
      refreshSection("bottlenecks"),
      refreshSection("trends"),
    ])
      .then(() => {
        hideLoading();
        trackPerformance("data refresh", startTime);
        showSuccess("Dashboard data refreshed successfully.");

        // Refresh charts with new data
        refreshCharts();
      })
      .catch((error) => {
        hideLoading();
        handleError(error, "refresh");
      });
  }

  function handleRetryLoad() {
    window.location.reload();
  }

  function refreshSection(sectionType, additionalParams = {}) {
    return new Promise((resolve, reject) => {
      const params = { ...currentFilters, ...additionalParams };
      let endpoint = "";

      switch (sectionType) {
        case "efficiency":
          endpoint = "/Dashboard/GetEfficiencyData";
          break;
        case "operators":
          endpoint = "/Dashboard/GetOperatorData";
          break;
        case "bottlenecks":
          endpoint = "/Dashboard/GetBottleneckData";
          break;
        case "trends":
          endpoint = "/Dashboard/GetTrendData";
          break;
        case "data":
          // Handle general data refresh - reload the entire page
          window.location.reload();
          return;
        default:
          reject(new Error("Unknown section type: " + sectionType));
          return;
      }

      // Build query string
      const queryParams = new URLSearchParams();
      Object.keys(params).forEach((key) => {
        if (params[key] !== null && params[key] !== undefined) {
          queryParams.append(key, params[key]);
        }
      });

      fetch(`${endpoint}?${queryParams.toString()}`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          RequestVerificationToken: getAntiForgeryToken(),
        },
      })
        .then((response) => {
          if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
          }
          return response.json();
        })
        .then((data) => {
          updateSectionContent(sectionType, data);
          resolve(data);
        })
        .catch((error) => {
          console.error(`Error refreshing ${sectionType} section:`, error);
          reject(error);
        });
    });
  }

  function updateSectionContent(sectionType, data) {
    console.log(`Updating ${sectionType} section with data:`, data);

    try {
      switch (sectionType) {
        case "efficiency":
          updateEfficiencySection(data);
          break;
        case "operators":
          updateOperatorSection(data);
          break;
        case "bottlenecks":
          updateBottleneckSection(data);
          break;
        case "trends":
          updateTrendSection(data);
          break;
        default:
          console.warn("Unknown section type:", sectionType);
      }
    } catch (error) {
      console.error(`Error updating ${sectionType} section:`, error);
    }
  }

  function updateEfficiencySection(data) {
    // Update KPI cards
    updateKPICard("on-time-rate", data.onTimeCompletionRate, "%");
    updateKPICard("avg-overrun", data.averageTimeOverrunPercentage, "%");
    updateKPICard("tasks-completed", data.totalTasksCompleted, "");
    updateKPICard("efficiency-score", data.overallEfficiencyScore, "");

    // Update efficiency chart
    if (charts.efficiency && data.trendData) {
      const chartData = {
        labels: data.trendData.map((d) => d.dateFormatted || d.periodLabel),
        data: data.trendData.map((d) => d.onTimeRate || d.value),
      };
      updateChartDataSafe(charts.efficiency, chartData, "efficiency");
    }

    // Update trend indicators
    updateTrendIndicator("efficiency-trend", data.efficiencyTrend);
  }

  function updateOperatorSection(data) {
    // Update operator performance chart
    if (charts.operator && data.operators) {
      const chartColors = {
        success: "#10b981",
        warning: "#f59e0b",
        danger: "#ef4444",
      };

      const chartData = {
        labels: data.operators.map((op) => op.operatorName),
        data: data.operators.map((op) => op.efficiencyRating),
      };

      const backgroundColors = data.operators.map((op) => {
        if (op.efficiencyRating >= 85) return chartColors.success;
        if (op.efficiencyRating >= 70) return chartColors.warning;
        return chartColors.danger;
      });

      charts.operator.data.datasets[0].backgroundColor = backgroundColors;
      updateChartDataSafe(charts.operator, chartData, "operator");
    }

    // Update operator table
    updateOperatorTable(data.operators);

    // Update summary cards
    if (data.summary) {
      updateKPICard("top-performers", data.summary.topPerformers, "");
      updateKPICard("team-average", data.summary.teamAverage, "%");
      updateKPICard("needs-attention", data.summary.needsAttention, "");
    }
  }

  function updateBottleneckSection(data) {
    // Update bottleneck chart
    if (charts.bottleneck && data.delaysByCategory) {
      const chartData = {
        labels: data.delaysByCategory.map((d) => d.categoryName),
        data: data.delaysByCategory.map((d) => d.delayCount),
      };
      updateChartDataSafe(charts.bottleneck, chartData, "bottleneck");
    }

    // Update bottleneck stats
    updateKPICard("total-bottlenecks", data.totalBottlenecks, "");
    updateKPICard("avg-delay", data.averageDelayHours, "h");
    updateKPICard("tasks-affected", data.tasksAffectedPercentage, "%");
    updateKPICard("severity-score", data.severityScore, "");

    // Update bottleneck list
    updateBottleneckList(data.frequentBottleneckTasks);
  }

  function updateTrendSection(data) {
    // Update main trend chart
    if (charts.mainTrend && data.trendData) {
      charts.mainTrend.data.labels = data.timeLabels || [];

      if (data.completionTrendData) {
        charts.mainTrend.data.datasets[0].data = data.completionTrendData.map(
          (d) => d.value
        );
      }
      if (data.efficiencyTrendData) {
        charts.mainTrend.data.datasets[1].data = data.efficiencyTrendData.map(
          (d) => d.value
        );
      }
      if (data.workloadTrendData) {
        charts.mainTrend.data.datasets[2].data = data.workloadTrendData.map(
          (d) => d.value
        );
      }

      charts.mainTrend.update();
    }

    // Update secondary charts
    updateSecondaryTrendCharts(data);

    // Update trend overview
    updateTrendOverview(data);
  }

  function updateSecondaryTrendCharts(data) {
    const chartMappings = {
      "completion-trend-chart": data.completionTrendData,
      "efficiency-trend-chart": data.efficiencyTrendData,
      "workload-trend-chart": data.workloadTrendData,
      "variance-trend-chart": data.varianceTrendData,
    };

    Object.keys(chartMappings).forEach((chartId) => {
      if (charts[chartId] && chartMappings[chartId]) {
        const chartData = {
          labels: chartMappings[chartId].map((d) => d.dateFormatted || d.label),
          data: chartMappings[chartId].map((d) => d.value),
        };
        updateChartDataSafe(charts[chartId], chartData, chartId);
      }
    });
  }

  // Helper functions for updating UI elements
  function updateKPICard(cardId, value, suffix = "") {
    const element = document.getElementById(cardId);
    if (element) {
      const valueElement = element.querySelector(
        ".metric-value, .stat-value, .summary-value"
      );
      if (valueElement) {
        const formattedValue =
          typeof value === "number" ? value.toFixed(1) : value;
        valueElement.textContent = formattedValue + suffix;
      }
    }
  }

  function updateTrendIndicator(indicatorId, trendValue) {
    const element = document.getElementById(indicatorId);
    if (element && typeof trendValue === "number") {
      const icon = element.querySelector("i");
      const text = element.querySelector(".trend-text");

      if (icon) {
        icon.className =
          trendValue >= 0 ? "fas fa-arrow-up" : "fas fa-arrow-down";
      }

      if (text) {
        text.textContent = Math.abs(trendValue).toFixed(1) + "% vs last period";
      }

      element.className =
        element.className.replace(/positive|negative/g, "") +
        (trendValue >= 0 ? " positive" : " negative");
    }
  }

  function updateOperatorTable(operators) {
    const tableBody = document.querySelector("#operator-table tbody");
    if (!tableBody || !operators) return;

    tableBody.innerHTML = "";

    operators.forEach((operator, index) => {
      const row = document.createElement("tr");
      row.className = operator.isTopPerformer
        ? "top-performer"
        : operator.needsAttention
        ? "needs-attention"
        : "";

      row.innerHTML = `
                <td><span class="${operator.rankClass || ""}">${
        index + 1
      }</span></td>
                <td>
                    <div class="operator-info">
                        <strong>${operator.operatorName}</strong>
                        <small class="text-muted d-block">${
                          operator.operatorEmail || ""
                        }</small>
                    </div>
                </td>
                <td>${operator.tasksCompleted || 0}</td>
                <td><span class="${operator.efficiencyClass || ""}">${
        operator.efficiencyRating?.toFixed(1) || "0"
      }%</span></td>
                <td>${operator.onTimeCompletionRate?.toFixed(1) || "0"}%</td>
                <td>${
                  operator.averageCompletionTimeHours?.toFixed(1) || "0"
                }h</td>
                <td>
                    <span class="badge ${
                      operator.isTopPerformer
                        ? "bg-success"
                        : operator.needsAttention
                        ? "bg-warning"
                        : "bg-secondary"
                    }">
                        ${operator.performanceStatus || "Normal"}
                    </span>
                </td>
                <td>
                    <span class="${operator.trendClass || ""}">
                        <i class="${operator.trendIcon || "fas fa-minus"}"></i>
                        ${operator.performanceTrend?.toFixed(1) || "0"}%
                    </span>
                </td>
            `;

      tableBody.appendChild(row);
    });
  }

  function updateBottleneckList(bottlenecks) {
    const listContainer = document.querySelector(".bottleneck-list");
    if (!listContainer || !bottlenecks) return;

    listContainer.innerHTML = "";

    bottlenecks.slice(0, 10).forEach((bottleneck) => {
      const item = document.createElement("div");
      item.className = "bottleneck-item";

      const delayPercentage = Math.min(
        100,
        (bottleneck.averageDelayHours / 8) * 100
      );

      item.innerHTML = `
                <div class="bottleneck-header">
                    <div class="bottleneck-title">
                        <strong>${bottleneck.taskName}</strong>
                        <span class="project-name">(${
                          bottleneck.projectName
                        })</span>
                    </div>
                    <div class="bottleneck-metrics">
                        <span class="occurrences">${
                          bottleneck.occurrences
                        } times</span>
                        <span class="avg-delay">${
                          bottleneck.averageDelayHours?.toFixed(1) || "0"
                        }h avg</span>
                    </div>
                </div>
                <div class="bottleneck-progress">
                    <div class="progress">
                        <div class="progress-bar bg-warning" style="width: ${delayPercentage.toFixed(
                          0
                        )}%"></div>
                    </div>
                    <small class="text-muted">Total delay: ${
                      bottleneck.totalDelayHours?.toFixed(1) || "0"
                    }h</small>
                </div>
            `;

      listContainer.appendChild(item);
    });
  }

  function updateTrendOverview(data) {
    if (data.overallTrendDirection) {
      updateKPICard("overall-trend", data.overallTrendDirection, "");
    }
    if (data.trendConfidence) {
      updateKPICard("trend-confidence", data.trendConfidence, "%");
    }
    if (data.totalDataPoints) {
      updateKPICard("data-points", data.totalDataPoints, "");
    }
    if (data.analysisPeriod) {
      updateKPICard("analysis-period", data.analysisPeriod, "");
    }
  }

  function initializeCharts() {
    console.log("Initializing dashboard charts...");

    if (typeof window.Chart === "undefined") {
      console.warn("Chart.js not loaded. Charts will not be available.");
      return;
    }

    // Set global Chart.js defaults
    window.Chart.defaults.font.family =
      "'Inter', 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif";
    window.Chart.defaults.color = "#374151";
    window.Chart.defaults.plugins.legend.position = "top";

    // Chart color palette
    const chartColors = {
      primary: "#3b82f6",
      success: "#10b981",
      warning: "#f59e0b",
      danger: "#ef4444",
      info: "#06b6d4",
      secondary: "#6b7280",
    };

    // Initialize individual charts with error handling
    try {
      initializeEfficiencyChart(chartColors);
      initializeOperatorChart(chartColors);
      initializeBottleneckChart(chartColors);
      initializeTrendCharts(chartColors);
      console.log("Dashboard charts initialized successfully");
    } catch (error) {
      console.error("Error initializing charts:", error);
    }
  }

  function initializeEfficiencyChart(colors) {
    const ctx = document.getElementById("efficiency-comparison-chart");
    if (!ctx) return;

    try {
      // Destroy existing chart if it exists
      const existingChart = window.Chart.getChart(ctx);
      if (existingChart) {
        existingChart.destroy();
      }

      charts.efficiency = new window.Chart(ctx.getContext("2d"), {
        type: "line",
        data: {
          labels: [],
          datasets: [
            {
              label: "On-Time Rate (%)",
              data: [],
              borderColor: colors.primary,
              backgroundColor: colors.primary + "20",
              tension: 0.4,
              fill: true,
              pointBackgroundColor: colors.primary,
              pointBorderColor: "#ffffff",
              pointBorderWidth: 2,
              pointRadius: 4,
            },
          ],
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          plugins: {
            legend: {
              display: true,
              position: "top",
            },
            tooltip: {
              mode: "index",
              intersect: false,
              callbacks: {
                label: function (context) {
                  return (
                    context.dataset.label +
                    ": " +
                    context.parsed.y.toFixed(1) +
                    "%"
                  );
                },
              },
            },
          },
          scales: {
            x: {
              display: true,
              grid: {
                display: false,
              },
            },
            y: {
              beginAtZero: true,
              max: 100,
              ticks: {
                callback: function (value) {
                  return value + "%";
                },
              },
              grid: {
                color: "#f3f4f6",
              },
            },
          },
          interaction: {
            mode: "nearest",
            axis: "x",
            intersect: false,
          },
        },
      });

      // Load data if available
      loadEfficiencyChartData();
    } catch (error) {
      console.error("Error initializing efficiency chart:", error);
    }
  }

  function initializeOperatorChart(colors) {
    const ctx = document.getElementById("operator-performance-chart");
    if (!ctx) return;

    try {
      // Destroy existing chart if it exists
      const existingChart = window.Chart.getChart(ctx);
      if (existingChart) {
        existingChart.destroy();
      }

      charts.operator = new window.Chart(ctx.getContext("2d"), {
        type: "bar",
        data: {
          labels: [],
          datasets: [
            {
              label: "Efficiency Rating (%)",
              data: [],
              backgroundColor: [],
              borderColor: "#dee2e6",
              borderWidth: 1,
              borderRadius: 4,
              borderSkipped: false,
            },
          ],
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          plugins: {
            legend: {
              display: false,
            },
            tooltip: {
              callbacks: {
                label: function (context) {
                  return (
                    context.dataset.label +
                    ": " +
                    context.parsed.y.toFixed(1) +
                    "%"
                  );
                },
                afterLabel: function (context) {
                  const rating = context.parsed.y;
                  if (rating >= 85) return "Performance: Excellent";
                  if (rating >= 70) return "Performance: Good";
                  return "Performance: Needs Improvement";
                },
              },
            },
          },
          scales: {
            x: {
              display: true,
              grid: {
                display: false,
              },
              ticks: {
                maxRotation: 45,
                minRotation: 0,
              },
            },
            y: {
              beginAtZero: true,
              max: 100,
              ticks: {
                callback: function (value) {
                  return value + "%";
                },
              },
              grid: {
                color: "#f3f4f6",
              },
            },
          },
        },
      });

      // Load data if available
      loadOperatorChartData(colors);
    } catch (error) {
      console.error("Error initializing operator chart:", error);
    }
  }

  function initializeBottleneckChart(colors) {
    const ctx = document.getElementById("bottleneck-categories-chart");
    if (!ctx) return;

    try {
      // Destroy existing chart if it exists
      const existingChart = window.Chart.getChart(ctx);
      if (existingChart) {
        existingChart.destroy();
      }

      charts.bottleneck = new window.Chart(ctx.getContext("2d"), {
        type: "doughnut",
        data: {
          labels: [],
          datasets: [
            {
              data: [],
              backgroundColor: [
                colors.danger,
                colors.warning,
                colors.info,
                colors.success,
                colors.primary,
                colors.secondary,
              ],
              borderWidth: 2,
              borderColor: "#ffffff",
              hoverBorderWidth: 3,
            },
          ],
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          plugins: {
            legend: {
              position: "right",
              labels: {
                usePointStyle: true,
                padding: 20,
              },
            },
            tooltip: {
              callbacks: {
                label: function (context) {
                  const total = context.dataset.data.reduce((a, b) => a + b, 0);
                  const percentage = ((context.parsed / total) * 100).toFixed(
                    1
                  );
                  return (
                    context.label +
                    ": " +
                    context.parsed +
                    " (" +
                    percentage +
                    "%)"
                  );
                },
              },
            },
          },
          cutout: "60%",
        },
      });

      // Load data if available
      loadBottleneckChartData();
    } catch (error) {
      console.error("Error initializing bottleneck chart:", error);
    }
  }

  function initializeTrendCharts(colors) {
    // Main trend chart
    const mainTrendCtx = document.getElementById("main-trend-chart");
    if (mainTrendCtx) {
      try {
        // Destroy existing chart if it exists
        const existingChart = window.Chart.getChart(mainTrendCtx);
        if (existingChart) {
          existingChart.destroy();
        }

        charts.mainTrend = new window.Chart(mainTrendCtx.getContext("2d"), {
          type: "line",
          data: {
            labels: [],
            datasets: [
              {
                label: "Completion Rate (%)",
                data: [],
                borderColor: colors.primary,
                backgroundColor: colors.primary + "20",
                tension: 0.4,
                yAxisID: "y",
              },
              {
                label: "Efficiency Score (%)",
                data: [],
                borderColor: colors.success,
                backgroundColor: colors.success + "20",
                tension: 0.4,
                yAxisID: "y",
              },
              {
                label: "Workload (Tasks)",
                data: [],
                borderColor: colors.warning,
                backgroundColor: colors.warning + "20",
                tension: 0.4,
                yAxisID: "y1",
              },
            ],
          },
          options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
              legend: {
                position: "top",
              },
            },
            scales: {
              x: {
                display: true,
                grid: {
                  display: false,
                },
              },
              y: {
                type: "linear",
                display: true,
                position: "left",
                beginAtZero: true,
                max: 100,
                ticks: {
                  callback: function (value) {
                    return value + "%";
                  },
                },
              },
              y1: {
                type: "linear",
                display: true,
                position: "right",
                beginAtZero: true,
                grid: {
                  drawOnChartArea: false,
                },
                ticks: {
                  callback: function (value) {
                    return value + " tasks";
                  },
                },
              },
            },
          },
        });
      } catch (error) {
        console.error("Error initializing main trend chart:", error);
      }
    }

    // Secondary trend charts
    const secondaryCharts = [
      {
        id: "completion-trend-chart",
        label: "Completion Rate",
        color: colors.primary,
      },
      {
        id: "efficiency-trend-chart",
        label: "Efficiency Score",
        color: colors.success,
      },
      { id: "workload-trend-chart", label: "Workload", color: colors.warning },
      { id: "variance-trend-chart", label: "Variance", color: colors.danger },
    ];

    secondaryCharts.forEach((config) => {
      const ctx = document.getElementById(config.id);
      if (ctx) {
        try {
          // Destroy existing chart if it exists
          const existingChart = window.Chart.getChart(ctx);
          if (existingChart) {
            existingChart.destroy();
          }

          charts[config.id] = new window.Chart(ctx.getContext("2d"), {
            type: "line",
            data: {
              labels: [],
              datasets: [
                {
                  label: config.label,
                  data: [],
                  borderColor: config.color,
                  backgroundColor: config.color + "20",
                  tension: 0.4,
                  fill: true,
                  pointRadius: 3,
                  pointHoverRadius: 5,
                },
              ],
            },
            options: {
              responsive: true,
              maintainAspectRatio: false,
              plugins: {
                legend: {
                  display: false,
                },
              },
              scales: {
                x: {
                  display: true,
                  grid: {
                    display: false,
                  },
                },
                y: {
                  beginAtZero: true,
                  grid: {
                    color: "#f3f4f6",
                  },
                },
              },
            },
          });
        } catch (error) {
          console.error(`Error initializing ${config.id}:`, error);
        }
      }
    });

    // Load trend data
    loadTrendChartsData();
  }

  // Data loading functions
  function loadEfficiencyChartData() {
    // Try to get data from the page if available
    const chartData = getChartDataFromPage("efficiency-chart-data");
    if (chartData && charts.efficiency) {
      updateChartData(charts.efficiency, chartData);
    }
  }

  function loadOperatorChartData(colors) {
    const chartData = getChartDataFromPage("operator-chart-data");
    if (chartData && charts.operator) {
      // Generate colors based on performance
      const backgroundColors = chartData.data.map((value) => {
        if (value >= 85) return colors.success;
        if (value >= 70) return colors.warning;
        return colors.danger;
      });

      charts.operator.data.datasets[0].backgroundColor = backgroundColors;
      updateChartData(charts.operator, chartData);
    }
  }

  function loadBottleneckChartData() {
    const chartData = getChartDataFromPage("bottleneck-chart-data");
    if (chartData && charts.bottleneck) {
      updateChartData(charts.bottleneck, chartData);
    }
  }

  function loadTrendChartsData() {
    const trendData = getChartDataFromPage("trend-chart-data");
    if (trendData) {
      // Update main trend chart
      if (charts.mainTrend) {
        charts.mainTrend.data.labels = trendData.labels || [];
        if (trendData.completion) {
          charts.mainTrend.data.datasets[0].data = trendData.completion;
        }
        if (trendData.efficiency) {
          charts.mainTrend.data.datasets[1].data = trendData.efficiency;
        }
        if (trendData.workload) {
          charts.mainTrend.data.datasets[2].data = trendData.workload;
        }
        charts.mainTrend.update();
      }

      // Update secondary charts
      const secondaryData = {
        "completion-trend-chart": trendData.completion,
        "efficiency-trend-chart": trendData.efficiency,
        "workload-trend-chart": trendData.workload,
        "variance-trend-chart": trendData.variance,
      };

      Object.keys(secondaryData).forEach((chartId) => {
        if (charts[chartId] && secondaryData[chartId]) {
          charts[chartId].data.labels = trendData.labels || [];
          charts[chartId].data.datasets[0].data = secondaryData[chartId];
          charts[chartId].update();
        }
      });
    }
  }

  // Utility functions for chart management
  function updateChartData(chart, data) {
    if (!chart || !data) return;

    chart.data.labels = data.labels || [];
    if (data.data) {
      chart.data.datasets[0].data = data.data;
    }
    chart.update();
  }

  function getChartDataFromPage(dataId) {
    const dataElement = document.getElementById(dataId);
    if (dataElement) {
      try {
        return JSON.parse(dataElement.textContent);
      } catch (error) {
        console.warn(`Failed to parse chart data for ${dataId}:`, error);
      }
    }
    return null;
  }

  // Chart refresh functions
  function refreshCharts() {
    console.log("Refreshing all charts...");

    loadEfficiencyChartData();
    loadOperatorChartData({
      primary: "#3b82f6",
      success: "#10b981",
      warning: "#f59e0b",
      danger: "#ef4444",
    });
    loadBottleneckChartData();
    loadTrendChartsData();
  }

  function handleExportPdf() {
    showLoading();

    fetch("/Dashboard/ExportPdf", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        RequestVerificationToken: getAntiForgeryToken(),
      },
      body: JSON.stringify(currentFilters),
    })
      .then((response) => {
        hideLoading();
        if (response.ok) {
          // Handle PDF download
          return response.blob();
        } else {
          throw new Error("Failed to generate PDF report");
        }
      })
      .then((blob) => {
        // Create download link
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement("a");
        a.href = url;
        a.download = `dashboard-report-${
          new Date().toISOString().split("T")[0]
        }.pdf`;
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);
      })
      .catch((error) => {
        hideLoading();
        showError("Failed to export PDF: " + error.message);
      });
  }

  function handleExportExcel() {
    showLoading();

    fetch("/Dashboard/ExportExcel", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        RequestVerificationToken: getAntiForgeryToken(),
      },
      body: JSON.stringify(currentFilters),
    })
      .then((response) => {
        hideLoading();
        if (response.ok) {
          // Handle Excel download
          return response.blob();
        } else {
          throw new Error("Failed to generate Excel report");
        }
      })
      .then((blob) => {
        // Create download link
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement("a");
        a.href = url;
        a.download = `dashboard-report-${
          new Date().toISOString().split("T")[0]
        }.xlsx`;
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);
      })
      .catch((error) => {
        hideLoading();
        showError("Failed to export Excel: " + error.message);
      });
  }

  // Utility functions
  function showLoading() {
    const loadingIndicator = document.getElementById("loading-indicator");
    if (loadingIndicator) {
      loadingIndicator.style.display = "flex";
    }
  }

  function hideLoading() {
    const loadingIndicator = document.getElementById("loading-indicator");
    if (loadingIndicator) {
      loadingIndicator.style.display = "none";
    }
  }

  function showSuccess(message) {
    // Use toastr if available, otherwise console log
    if (typeof toastr !== "undefined") {
      toastr.success(message);
    } else {
      console.log("Success:", message);
    }
  }

  function showError(message) {
    // Use toastr if available, otherwise console error
    if (typeof toastr !== "undefined") {
      toastr.error(message);
    } else {
      console.error("Error:", message);
    }
  }

  function showWarning(message) {
    // Use toastr if available, otherwise console warn
    if (typeof toastr !== "undefined") {
      toastr.warning(message);
    } else {
      console.warn("Warning:", message);
    }
  }

  function getAntiForgeryToken() {
    const token = document.querySelector(
      'input[name="__RequestVerificationToken"]'
    );
    return token ? token.value : "";
  }

  // Enhanced Auto-refresh functionality with real-time capabilities
  let autoRefreshInterval = null;
  let autoRefreshEnabled = false;
  let refreshInProgress = false;
  let lastRefreshTime = null;
  let refreshFailureCount = 0;
  let refreshStatusInterval = null;
  const MAX_REFRESH_FAILURES = 3;

  function initializeAutoRefresh() {
    const autoRefreshToggle = document.getElementById("auto-refresh-toggle");
    const autoRefreshIntervalSelect = document.getElementById(
      "auto-refresh-interval"
    );
    const manualRefreshBtn = document.getElementById("manual-refresh-btn");
    const refreshStatusBtn = document.getElementById("refresh-status-btn");

    if (autoRefreshToggle) {
      autoRefreshToggle.addEventListener("change", function () {
        if (this.checked) {
          startAutoRefresh();
        } else {
          stopAutoRefresh();
        }
      });
    }

    if (autoRefreshIntervalSelect) {
      autoRefreshIntervalSelect.addEventListener("change", function () {
        if (autoRefreshEnabled) {
          stopAutoRefresh();
          startAutoRefresh();
        }
      });
    }

    if (manualRefreshBtn) {
      manualRefreshBtn.addEventListener("click", function () {
        performManualRefresh();
      });
    }

    if (refreshStatusBtn) {
      refreshStatusBtn.addEventListener("click", function () {
        updateRefreshStatus();
      });
    }

    // Initialize refresh status display
    initializeRefreshStatus();

    // Set up periodic status updates
    refreshStatusInterval = setInterval(updateRefreshStatus, 1000);
  }

  function startAutoRefresh() {
    const intervalSelect = document.getElementById("auto-refresh-interval");
    const intervalMinutes = intervalSelect ? parseInt(intervalSelect.value) : 5;
    const intervalMs = intervalMinutes * 60 * 1000;

    autoRefreshEnabled = true;
    refreshFailureCount = 0; // Reset failure count

    autoRefreshInterval = setInterval(() => {
      console.log("Auto-refreshing dashboard data...");
      performDataRefresh(false); // false = auto refresh
    }, intervalMs);

    updateRefreshStatus();
    showSuccess(`Auto-refresh enabled (every ${intervalMinutes} minutes)`);
  }

  function stopAutoRefresh() {
    if (autoRefreshInterval) {
      clearInterval(autoRefreshInterval);
      autoRefreshInterval = null;
    }
    if (refreshStatusInterval) {
      clearInterval(refreshStatusInterval);
      refreshStatusInterval = null;
    }
    autoRefreshEnabled = false;
    updateRefreshStatus();
    showSuccess("Auto-refresh disabled");
  }

  function performManualRefresh() {
    if (refreshInProgress) {
      showWarning("Refresh already in progress. Please wait...");
      return;
    }

    console.log("Manual refresh triggered");
    performDataRefresh(true); // true = manual refresh
  }

  async function performDataRefresh(isManual = false) {
    if (refreshInProgress) {
      console.log("Refresh already in progress, skipping...");
      return;
    }

    refreshInProgress = true;
    updateRefreshStatus();

    try {
      const refreshType = isManual ? "Manual" : "Auto";
      console.log(`${refreshType} refresh started`);

      // Show loading indicator
      showRefreshIndicator(true);

      // Get current filters
      const filters = getCurrentFilters();

      // Perform incremental data refresh
      await refreshDashboardData(filters);

      // Update last refresh time
      lastRefreshTime = new Date();
      refreshFailureCount = 0;

      console.log(`${refreshType} refresh completed successfully`);

      if (isManual) {
        showSuccess("Dashboard data refreshed successfully");
      }
    } catch (error) {
      refreshFailureCount++;
      console.error("Refresh failed:", error);

      if (isManual) {
        showError("Failed to refresh dashboard data. Please try again.");
      }

      // Auto-disable refresh after max failures
      if (refreshFailureCount >= MAX_REFRESH_FAILURES && autoRefreshEnabled) {
        stopAutoRefresh();
        showError(
          `Auto-refresh disabled after ${MAX_REFRESH_FAILURES} consecutive failures`
        );
      }
    } finally {
      refreshInProgress = false;
      showRefreshIndicator(false);
      updateRefreshStatus();
    }
  }

  // Refresh status and indicator functions
  function initializeRefreshStatus() {
    updateRefreshStatus();

    // Add visibility change listener to pause/resume refresh when tab is hidden
    document.addEventListener("visibilitychange", function () {
      if (document.hidden && autoRefreshEnabled) {
        console.log("Tab hidden, pausing auto-refresh");
      } else if (!document.hidden && autoRefreshEnabled) {
        console.log("Tab visible, resuming auto-refresh");
        updateRefreshStatus();
      }
    });
  }

  function updateRefreshStatus() {
    const statusElement = document.getElementById("refresh-status");
    const lastRefreshElement = document.getElementById("last-refresh-time");
    const nextRefreshElement = document.getElementById("next-refresh-time");

    if (statusElement) {
      let statusText = "Disabled";
      let statusClass = "text-muted";

      if (refreshInProgress) {
        statusText = "Refreshing...";
        statusClass = "text-info";
      } else if (autoRefreshEnabled) {
        statusText = "Active";
        statusClass = "text-success";
      } else if (refreshFailureCount > 0) {
        statusText = `Failed (${refreshFailureCount})`;
        statusClass = "text-danger";
      }

      statusElement.textContent = statusText;
      statusElement.className = `badge ${statusClass}`;
    }

    if (lastRefreshElement && lastRefreshTime) {
      const timeAgo = getTimeAgo(lastRefreshTime);
      lastRefreshElement.textContent = timeAgo;
    }

    if (nextRefreshElement && autoRefreshEnabled && autoRefreshInterval) {
      const intervalSelect = document.getElementById("auto-refresh-interval");
      const intervalMinutes = intervalSelect
        ? parseInt(intervalSelect.value)
        : 5;
      const nextRefresh = new Date(Date.now() + intervalMinutes * 60 * 1000);
      nextRefreshElement.textContent = nextRefresh.toLocaleTimeString();
    } else if (nextRefreshElement) {
      nextRefreshElement.textContent = "N/A";
    }
  }

  function showRefreshIndicator(show) {
    const indicator = document.getElementById("refresh-indicator");
    const manualBtn = document.getElementById("manual-refresh-btn");

    if (indicator) {
      indicator.style.display = show ? "inline-block" : "none";
    }

    if (manualBtn) {
      manualBtn.disabled = show;
      if (show) {
        manualBtn.innerHTML =
          '<i class="fas fa-spinner fa-spin"></i> Refreshing...';
      } else {
        manualBtn.innerHTML = '<i class="fas fa-sync-alt"></i> Refresh Now';
      }
    }
  }

  function getTimeAgo(date) {
    const now = new Date();
    const diffMs = now - date;
    const diffSecs = Math.floor(diffMs / 1000);
    const diffMins = Math.floor(diffSecs / 60);
    const diffHours = Math.floor(diffMins / 60);

    if (diffSecs < 60) {
      return `${diffSecs} seconds ago`;
    } else if (diffMins < 60) {
      return `${diffMins} minutes ago`;
    } else if (diffHours < 24) {
      return `${diffHours} hours ago`;
    } else {
      return date.toLocaleString();
    }
  }

  /**
   * Initialize date inputs with enhanced functionality
   */
  function initializeDateInputs() {
    const fromDateInput = document.getElementById("fromDate");
    const toDateInput = document.getElementById("toDate");

    // Only set values if they come from the server (existing filters)
    // Don't set default values on initial page load

    // Add date format validation and input formatting
    if (fromDateInput) {
      fromDateInput.addEventListener("input", formatDateInput);
      fromDateInput.addEventListener("blur", validateAndFormatDate);
      fromDateInput.addEventListener("keypress", handleDateKeyPress);
    }
    if (toDateInput) {
      toDateInput.addEventListener("input", formatDateInput);
      toDateInput.addEventListener("blur", validateAndFormatDate);
      toDateInput.addEventListener("keypress", handleDateKeyPress);
    }

    // Create hidden date inputs for form submission
    createHiddenDateInputs();
  }

  /**
   * Format date for display (dd/MM/yyyy)
   */
  function formatDateForDisplay(date) {
    if (!date) return "";

    // Handle both Date objects and date strings
    if (typeof date === "string") {
      date = new Date(date);
    }

    if (isNaN(date.getTime())) return "";

    const day = String(date.getDate()).padStart(2, "0");
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const year = date.getFullYear();
    return `${day}/${month}/${year}`;
  }

  /**
   * Parse dd/MM/yyyy format to Date object
   */
  function parseDateFromDisplay(dateString) {
    if (!dateString) return null;

    const parts = dateString.split("/");
    if (parts.length !== 3) return null;

    const day = parseInt(parts[0], 10);
    const month = parseInt(parts[1], 10) - 1; // Month is 0-indexed
    const year = parseInt(parts[2], 10);

    const date = new Date(year, month, day);

    // Validate the date
    if (
      date.getDate() !== day ||
      date.getMonth() !== month ||
      date.getFullYear() !== year
    ) {
      return null;
    }

    return date;
  }

  /**
   * Convert dd/MM/yyyy to yyyy-MM-dd for server
   */
  function convertToServerFormat(dateString) {
    const date = parseDateFromDisplay(dateString);
    if (!date) return "";

    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const day = String(date.getDate()).padStart(2, "0");
    return `${year}-${month}-${day}`;
  }

  /**
   * Validate date input and show format hint
   */
  function validateDateInput(event) {
    const input = event.target;
    const value = input.value;

    if (value) {
      // Use our custom parser for dd/MM/yyyy format
      const date = parseDateFromDisplay(value);
      if (!date || isNaN(date.getTime())) {
        showError("Please enter a valid date in dd/MM/yyyy format.");
        input.focus();
        return false;
      }

      // Validate date range
      validateDateRange();
    }
    return true;
  }

  /**
   * Validate that From Date is not later than To Date
   */
  function validateDateRange() {
    const fromDateInput = document.getElementById("fromDate");
    const toDateInput = document.getElementById("toDate");

    // Clear previous validation states
    clearValidationStates();

    if (
      fromDateInput &&
      toDateInput &&
      fromDateInput.value &&
      toDateInput.value
    ) {
      // Use our custom parser for dd/MM/yyyy format
      const fromDate = parseDateFromDisplay(fromDateInput.value);
      const toDate = parseDateFromDisplay(toDateInput.value);

      if (fromDate && toDate && fromDate > toDate) {
        showError("From date cannot be later than To date.");
        setValidationState(fromDateInput, "error");
        setValidationState(toDateInput, "error");
        fromDateInput.focus();
        return false;
      } else if (fromDate && toDate) {
        setValidationState(fromDateInput, "success");
        setValidationState(toDateInput, "success");
      }
    }
    return true;
  }

  /**
   * Set validation state for date input
   */
  function setValidationState(input, state) {
    const wrapper = input.closest(".date-input-wrapper");

    // Remove existing states
    input.classList.remove("is-valid", "is-invalid");
    wrapper.classList.remove("has-success", "has-error");

    // Add new state
    if (state === "success") {
      input.classList.add("is-valid");
      wrapper.classList.add("has-success");
    } else if (state === "error") {
      input.classList.add("is-invalid");
      wrapper.classList.add("has-error");
    }
  }

  /**
   * Clear all validation states
   */
  function clearValidationStates() {
    const dateInputs = document.querySelectorAll(".filter-input.date-input");
    dateInputs.forEach((input) => {
      clearValidationState(input);
    });
  }

  /**
   * Clear validation state for a single input
   */
  function clearValidationState(input) {
    const wrapper = input.closest(".date-input-wrapper");
    input.classList.remove("is-valid", "is-invalid");
    if (wrapper) {
      wrapper.classList.remove("has-success", "has-error");
    }
  }

  /**
   * Format date input as user types
   */
  function formatDateInput(event) {
    const input = event.target;
    let value = input.value.replace(/\D/g, ""); // Remove non-digits

    // Format as dd/MM/yyyy
    if (value.length >= 2) {
      value = value.substring(0, 2) + "/" + value.substring(2);
    }
    if (value.length >= 5) {
      value = value.substring(0, 5) + "/" + value.substring(5, 9);
    }

    input.value = value;
  }

  /**
   * Handle key press for date input
   */
  function handleDateKeyPress(event) {
    const char = String.fromCharCode(event.which);

    // Allow digits, forward slash, backspace, delete, arrow keys
    if (
      !/[\d\/]/.test(char) &&
      ![8, 9, 27, 13, 46, 37, 38, 39, 40].includes(event.keyCode)
    ) {
      event.preventDefault();
    }
  }

  /**
   * Validate and format date when user leaves input
   */
  function validateAndFormatDate(event) {
    const input = event.target;
    const value = input.value.trim();

    if (!value) {
      clearValidationState(input);
      return;
    }

    const date = parseDateFromDisplay(value);
    if (date) {
      // Valid date - format it properly
      input.value = formatDateForDisplay(date);
      setValidationState(input, "success");
      validateDateRange();
      updateHiddenInput(input);
    } else {
      // Invalid date
      setValidationState(input, "error");
      showError("Please enter a valid date in dd/MM/yyyy format.");
    }
  }

  /**
   * Create hidden inputs for form submission in server format
   */
  function createHiddenDateInputs() {
    const form = document.getElementById("dashboard-filters");
    if (!form) return;

    // Create hidden inputs for server submission
    const hiddenFromDate = document.createElement("input");
    hiddenFromDate.type = "hidden";
    hiddenFromDate.name = "fromDateServer";
    hiddenFromDate.id = "fromDateServer";

    const hiddenToDate = document.createElement("input");
    hiddenToDate.type = "hidden";
    hiddenToDate.name = "toDateServer";
    hiddenToDate.id = "toDateServer";

    form.appendChild(hiddenFromDate);
    form.appendChild(hiddenToDate);
  }

  /**
   * Update hidden input with server format
   */
  function updateHiddenInput(input) {
    const inputId = input.id;
    const hiddenInput = document.getElementById(inputId + "Server");

    if (hiddenInput && input.value) {
      const serverFormat = convertToServerFormat(input.value);
      hiddenInput.value = serverFormat;
    }
  }

  // Data refresh functions
  async function refreshDashboardData(filters) {
    try {
      // Refresh efficiency metrics
      await refreshEfficiencyData(filters);

      // Refresh operator performance
      await refreshOperatorData(filters);

      // Refresh bottleneck analysis
      await refreshBottleneckData(filters);

      // Refresh trend charts
      await refreshTrendData(filters);

      console.log("All dashboard sections refreshed successfully");
    } catch (error) {
      console.error("Error refreshing dashboard data:", error);
      throw error;
    }
  }

  async function refreshEfficiencyData(filters) {
    try {
      const response = await fetch("/Dashboard/GetEfficiencyData", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          RequestVerificationToken: getAntiForgeryToken(),
        },
        body: JSON.stringify(filters),
      });

      if (!response.ok) {
        throw new Error(`HTTP ${response.status}: ${response.statusText}`);
      }

      const data = await response.json();
      updateEfficiencyMetrics(data);
    } catch (error) {
      console.error("Error refreshing efficiency data:", error);
      throw error;
    }
  }

  async function refreshOperatorData(filters) {
    try {
      const response = await fetch("/Dashboard/GetOperatorData", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          RequestVerificationToken: getAntiForgeryToken(),
        },
        body: JSON.stringify(filters),
      });

      if (!response.ok) {
        throw new Error(`HTTP ${response.status}: ${response.statusText}`);
      }

      const data = await response.json();
      updateOperatorPerformance(data);
    } catch (error) {
      console.error("Error refreshing operator data:", error);
      throw error;
    }
  }

  async function refreshBottleneckData(filters) {
    try {
      const response = await fetch("/Dashboard/GetBottleneckData", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          RequestVerificationToken: getAntiForgeryToken(),
        },
        body: JSON.stringify(filters),
      });

      if (!response.ok) {
        throw new Error(`HTTP ${response.status}: ${response.statusText}`);
      }

      const data = await response.json();
      updateBottleneckAnalysis(data);
    } catch (error) {
      console.error("Error refreshing bottleneck data:", error);
      throw error;
    }
  }

  async function refreshTrendData(filters) {
    try {
      const response = await fetch("/Dashboard/GetTrendData", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          RequestVerificationToken: getAntiForgeryToken(),
        },
        body: JSON.stringify(filters),
      });

      if (!response.ok) {
        throw new Error(`HTTP ${response.status}: ${response.statusText}`);
      }

      const data = await response.json();
      updateTrendCharts(data);
    } catch (error) {
      console.error("Error refreshing trend data:", error);
      throw error;
    }
  }

  // Data update functions for UI refresh
  function updateEfficiencyMetrics(data) {
    try {
      // Update KPI cards
      const onTimeRate = document.getElementById("on-time-completion-rate");
      const overrunRate = document.getElementById("average-overrun-rate");
      const tasksCompleted = document.getElementById("tasks-completed");
      const efficiencyScore = document.getElementById("efficiency-score");

      if (onTimeRate && data.onTimeCompletionRate !== undefined) {
        onTimeRate.textContent = `${data.onTimeCompletionRate.toFixed(1)}%`;
      }
      if (overrunRate && data.averageTimeOverrunPercentage !== undefined) {
        overrunRate.textContent = `${data.averageTimeOverrunPercentage.toFixed(
          1
        )}%`;
      }
      if (tasksCompleted && data.totalTasksCompleted !== undefined) {
        tasksCompleted.textContent = data.totalTasksCompleted;
      }
      if (efficiencyScore && data.overallEfficiencyScore !== undefined) {
        efficiencyScore.textContent = data.overallEfficiencyScore.toFixed(1);
      }

      // Update efficiency chart if it exists
      if (charts.efficiency && data.efficiencyTrend) {
        updateChart(charts.efficiency, data.efficiencyTrend);
      }

      console.log("Efficiency metrics updated");
    } catch (error) {
      console.error("Error updating efficiency metrics:", error);
    }
  }

  function updateOperatorPerformance(data) {
    try {
      // Update operator performance table
      const tableBody = document.querySelector(
        "#operator-performance-table tbody"
      );
      if (tableBody && data.operators) {
        tableBody.innerHTML = "";

        data.operators.forEach((operator) => {
          const row = document.createElement("tr");
          row.innerHTML = `
                        <td>${operator.operatorName}</td>
                        <td>${operator.efficiencyRating.toFixed(1)}%</td>
                        <td>${operator.tasksCompleted}</td>
                        <td>${operator.averageCompletionTimeHours.toFixed(
                          1
                        )}h</td>
                        <td>
                            <span class="badge ${
                              operator.isTopPerformer
                                ? "bg-success"
                                : operator.needsAttention
                                ? "bg-warning"
                                : "bg-secondary"
                            }">
                                ${
                                  operator.isTopPerformer
                                    ? "Top Performer"
                                    : operator.needsAttention
                                    ? "Needs Attention"
                                    : "Average"
                                }
                            </span>
                        </td>
                    `;
          tableBody.appendChild(row);
        });
      }

      // Update operator performance chart if it exists
      if (charts.operatorPerformance && data.chartData) {
        updateChart(charts.operatorPerformance, data.chartData);
      }

      console.log("Operator performance updated");
    } catch (error) {
      console.error("Error updating operator performance:", error);
    }
  }

  function updateBottleneckAnalysis(data) {
    try {
      // Update bottleneck summary cards
      const totalBottlenecks = document.getElementById("total-bottlenecks");
      const averageDelay = document.getElementById("average-delay");
      const tasksAffected = document.getElementById("tasks-affected");
      const severityScore = document.getElementById("severity-score");

      if (totalBottlenecks && data.totalBottlenecks !== undefined) {
        totalBottlenecks.textContent = data.totalBottlenecks;
      }
      if (averageDelay && data.averageDelayHours !== undefined) {
        averageDelay.textContent = `${data.averageDelayHours.toFixed(1)}h`;
      }
      if (tasksAffected && data.tasksAffectedPercentage !== undefined) {
        tasksAffected.textContent = `${data.tasksAffectedPercentage.toFixed(
          1
        )}%`;
      }
      if (severityScore && data.severityScoreFormatted !== undefined) {
        severityScore.textContent = data.severityScoreFormatted;
      }

      // Update bottleneck chart if it exists
      if (charts.bottlenecks && data.chartData) {
        updateChart(charts.bottlenecks, data.chartData);
      }

      console.log("Bottleneck analysis updated");
    } catch (error) {
      console.error("Error updating bottleneck analysis:", error);
    }
  }

  function updateTrendCharts(data) {
    try {
      // Update completion trend chart
      if (charts.completionTrend && data.completionTrendData) {
        updateChart(charts.completionTrend, data.completionTrendData);
      }

      // Update workload trend chart
      if (charts.workloadTrend && data.workloadTrendData) {
        updateChart(charts.workloadTrend, data.workloadTrendData);
      }

      // Update efficiency trend chart
      if (charts.efficiencyTrend && data.efficiencyTrendData) {
        updateChart(charts.efficiencyTrend, data.efficiencyTrendData);
      }

      // Update variance trend chart
      if (charts.varianceTrend && data.varianceTrendData) {
        updateChart(charts.varianceTrend, data.varianceTrendData);
      }

      console.log("Trend charts updated");
    } catch (error) {
      console.error("Error updating trend charts:", error);
    }
  }

  function updateChart(chart, newData) {
    try {
      if (!chart || !newData) {
        console.warn("Chart or data is null/undefined");
        return;
      }

      // Update chart data
      if (newData.labels) {
        chart.data.labels = newData.labels;
      }

      if (newData.datasets) {
        chart.data.datasets = newData.datasets;
      } else if (newData.data) {
        // Handle simple data format
        if (chart.data.datasets[0]) {
          chart.data.datasets[0].data = newData.data;
        }
      }

      // Update chart with animation
      chart.update("active");
    } catch (error) {
      console.error("Error updating chart:", error);
    }
  }

  // Enhanced error handling
  function handleError(error, context = "") {
    console.error(`Dashboard error ${context}:`, error);

    let errorMessage = "An unexpected error occurred.";

    if (error.name === "TypeError" && error.message.includes("fetch")) {
      errorMessage =
        "Network error. Please check your connection and try again.";
    } else if (error.status === 401) {
      errorMessage = "Your session has expired. Please log in again.";
      // Redirect to login after a delay
      setTimeout(() => {
        window.location.href = "/Account/Login";
      }, 3000);
    } else if (error.status === 403) {
      errorMessage = "You do not have permission to access this data.";
    } else if (error.status === 404) {
      errorMessage = "The requested data could not be found.";
    } else if (error.status >= 500) {
      errorMessage = "Server error. Please try again later.";
    } else if (error.message) {
      errorMessage = error.message;
    }

    showError(errorMessage);

    // Show retry option for certain errors
    if (error.status >= 500 || error.name === "TypeError") {
      showRetryOption(context);
    }
  }

  function showRetryOption(context) {
    const retryContainer = document.getElementById("retry-container");
    if (retryContainer) {
      retryContainer.style.display = "block";

      const retryButton = retryContainer.querySelector(".retry-button");
      if (retryButton) {
        retryButton.onclick = () => {
          retryContainer.style.display = "none";
          if (context === "refresh") {
            handleRefreshData();
          } else {
            window.location.reload();
          }
        };
      }
    }
  }

  // Performance monitoring
  function trackPerformance(operation, startTime) {
    const endTime = performance.now();
    const duration = endTime - startTime;

    console.log(`Dashboard ${operation} completed in ${duration.toFixed(2)}ms`);

    // Log slow operations
    if (duration > 2000) {
      console.warn(
        `Slow dashboard operation detected: ${operation} took ${duration.toFixed(
          2
        )}ms`
      );
    }
  }

  // Responsive chart handling
  function handleResize() {
    Object.keys(charts).forEach((chartKey) => {
      if (charts[chartKey] && typeof charts[chartKey].resize === "function") {
        charts[chartKey].resize();
      }
    });
  }

  // Initialize resize handler
  window.addEventListener("resize", debounce(handleResize, 250));

  // Debounce utility function
  function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
      const later = () => {
        clearTimeout(timeout);
        func(...args);
      };
      clearTimeout(timeout);
      timeout = setTimeout(later, wait);
    };
  }

  // Get current filter values
  function getCurrentFilters() {
    return {
      fromDate: document.getElementById("fromDate")?.value || null,
      toDate: document.getElementById("toDate")?.value || null,
      granularity: document.getElementById("granularity")?.value || "weekly",
    };
  }

  // Enhanced data validation
  function validateChartData(data, chartType) {
    if (!data) {
      console.warn(`No data provided for ${chartType} chart`);
      return false;
    }

    if (!Array.isArray(data.labels) || !Array.isArray(data.data)) {
      console.warn(`Invalid data structure for ${chartType} chart`);
      return false;
    }

    if (data.labels.length !== data.data.length) {
      console.warn(`Data length mismatch for ${chartType} chart`);
      return false;
    }

    return true;
  }

  // Enhanced chart update with validation
  function updateChartDataSafe(chart, data, chartType = "unknown") {
    if (!chart) {
      console.warn(`Chart not found for ${chartType}`);
      return;
    }

    if (!validateChartData(data, chartType)) {
      return;
    }

    try {
      chart.data.labels = data.labels;
      if (data.data) {
        chart.data.datasets[0].data = data.data;
      }
      chart.update("none"); // Use 'none' animation for better performance
    } catch (error) {
      console.error(`Error updating ${chartType} chart:`, error);
    }
  }

  // Accessibility improvements
  function initializeAccessibility() {
    // Add ARIA labels to charts
    Object.keys(charts).forEach((chartKey) => {
      const canvas = document.getElementById(
        chartKey.replace("charts.", "") + "-chart"
      );
      if (canvas) {
        canvas.setAttribute("role", "img");
        canvas.setAttribute("aria-label", `${chartKey} performance chart`);
      }
    });

    // Add keyboard navigation for interactive elements
    const interactiveElements = document.querySelectorAll(
      ".chart-container, .kpi-card, .filter-control"
    );
    interactiveElements.forEach((element) => {
      if (!element.hasAttribute("tabindex")) {
        element.setAttribute("tabindex", "0");
      }
    });
  }

  /**
   * Open date picker when calendar icon is clicked
   */
  function openDatePicker(inputId) {
    console.log("openDatePicker called with inputId:", inputId);

    const input = document.getElementById(inputId);
    if (!input) {
      console.error("Input element not found:", inputId);
      return;
    }

    console.log("Input found, creating date picker...");

    // Create a temporary HTML5 date input for the picker
    const tempDateInput = document.createElement("input");
    tempDateInput.type = "date";
    tempDateInput.style.position = "absolute";
    tempDateInput.style.left = "-9999px";
    tempDateInput.style.opacity = "0";
    tempDateInput.style.pointerEvents = "none";

    // Set current value if valid
    const currentValue = input.value.trim();
    console.log("Current input value:", currentValue);

    if (currentValue) {
      try {
        const serverFormat = convertToServerFormat(currentValue);
        console.log("Converted to server format:", serverFormat);
        if (serverFormat) {
          tempDateInput.value = serverFormat;
        }
      } catch (e) {
        console.warn("Error converting current value:", e);
      }
    }

    // Add to DOM temporarily
    document.body.appendChild(tempDateInput);
    console.log("Temp input added to DOM");

    // Handle date selection
    tempDateInput.addEventListener("change", function () {
      console.log("Date selected:", this.value);
      try {
        if (this.value) {
          const selectedDate = new Date(this.value);
          const displayFormat = formatDateForDisplay(selectedDate);
          console.log("Formatted for display:", displayFormat);
          input.value = displayFormat;

          // Trigger validation if function exists
          if (typeof validateAndFormatDate === "function") {
            try {
              validateAndFormatDate({ target: input });
            } catch (e) {
              console.warn("Validation error:", e);
            }
          }
        }
      } catch (e) {
        console.error("Error processing selected date:", e);
      }

      // Clean up
      try {
        if (document.body.contains(this)) {
          document.body.removeChild(this);
          console.log("Temp input removed");
        }
      } catch (e) {
        console.warn("Error removing temp input:", e);
      }
    });

    // Handle cancel/close
    tempDateInput.addEventListener("blur", function () {
      setTimeout(() => {
        try {
          if (document.body.contains(this)) {
            document.body.removeChild(this);
            console.log("Temp input removed on blur");
          }
        } catch (e) {
          console.warn("Error removing temp input on blur:", e);
        }
      }, 100);
    });

    // Open the date picker
    console.log("Attempting to open date picker...");
    try {
      tempDateInput.focus();
      if (tempDateInput.showPicker) {
        tempDateInput.showPicker();
        console.log("showPicker() called successfully");
      } else {
        console.log("showPicker() not available, trying click()");
        tempDateInput.click();
      }
    } catch (e) {
      console.warn("Error opening date picker:", e);
      try {
        tempDateInput.click();
        console.log("Fallback click() called");
      } catch (e2) {
        console.error("Both showPicker() and click() failed:", e2);
      }
    }
  }

  // Working date picker function
  function openDatePickerSimple(inputId) {
    console.log("openDatePickerSimple called with inputId:", inputId);

    const input = document.getElementById(inputId);
    if (!input) {
      console.error("Input element not found:", inputId);
      return;
    }

    // Create a visible but transparent date input overlay
    const dateInput = document.createElement("input");
    dateInput.type = "date";

    // Position it exactly over the text input
    const rect = input.getBoundingClientRect();
    dateInput.style.position = "fixed";
    dateInput.style.top = rect.top + "px";
    dateInput.style.left = rect.left + "px";
    dateInput.style.width = rect.width + "px";
    dateInput.style.height = rect.height + "px";
    dateInput.style.opacity = "0";
    dateInput.style.zIndex = "9999";
    dateInput.style.border = "none";
    dateInput.style.background = "transparent";
    dateInput.style.cursor = "pointer";

    // Set current value if valid
    const currentValue = input.value.trim();
    if (currentValue) {
      try {
        const serverFormat = convertToServerFormat(currentValue);
        if (serverFormat) {
          dateInput.value = serverFormat;
          console.log("Set initial value:", serverFormat);
        }
      } catch (e) {
        console.warn("Error converting current value:", e);
      }
    }

    // Add to body
    document.body.appendChild(dateInput);
    console.log("Date input added to DOM");

    // Handle date selection
    dateInput.addEventListener("change", function () {
      console.log("Date selected:", this.value);
      if (this.value) {
        try {
          const selectedDate = new Date(this.value);
          const displayFormat = formatDateForDisplay(selectedDate);
          console.log("Setting input value to:", displayFormat);
          input.value = displayFormat;

          // Trigger input event
          const inputEvent = new Event("input", { bubbles: true });
          input.dispatchEvent(inputEvent);

          console.log("Input value set successfully");
        } catch (e) {
          console.error("Error formatting selected date:", e);
        }
      }
      // Clean up
      this.remove();
      console.log("Date input removed");
    });

    // Handle click outside or escape
    dateInput.addEventListener("blur", function () {
      console.log("Date input blur event");
      setTimeout(() => {
        if (document.body.contains(this)) {
          this.remove();
          console.log("Date input removed on blur");
        }
      }, 100);
    });

    // Handle escape key
    dateInput.addEventListener("keydown", function (e) {
      if (e.key === "Escape") {
        this.remove();
        console.log("Date input removed on escape");
      }
    });

    // Focus and open picker immediately
    dateInput.focus();
    console.log("Date input focused");

    // Try to open the picker
    setTimeout(() => {
      try {
        if (dateInput.showPicker) {
          dateInput.showPicker();
          console.log("showPicker() called successfully");
        } else {
          console.log("showPicker() not available, trying click");
          dateInput.click();
        }
      } catch (e) {
        console.error("Error opening date picker:", e);
        // Try alternative approach
        try {
          dateInput.click();
          console.log("Fallback click() called");
        } catch (e2) {
          console.error("Both methods failed:", e2);
          dateInput.remove();
        }
      }
    }, 50);
  }

  // Clean date picker implementation using only dd/MM/yyyy format
  function openDatePickerDirect(inputId) {
    console.log("openDatePickerDirect called with inputId:", inputId);

    const input = document.getElementById(inputId);
    if (!input) {
      console.error("Input element not found:", inputId);
      return;
    }

    // Create a custom date picker modal
    createDatePickerModal(input);
  }

  function createDatePickerModal(targetInput) {
    // Remove any existing date picker modal
    const existingModal = document.getElementById("custom-date-picker-modal");
    if (existingModal) {
      existingModal.remove();
    }

    // Create modal overlay
    const modal = document.createElement("div");
    modal.id = "custom-date-picker-modal";
    modal.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.5);
            display: flex;
            justify-content: center;
            align-items: center;
            z-index: 10000;
            animation: fadeIn 0.2s ease;
        `;

    // Create date picker container
    const container = document.createElement("div");
    container.style.cssText = `
            background: white;
            border-radius: 8px;
            padding: 20px;
            box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
            max-width: 320px;
            width: 90%;
            animation: slideIn 0.3s ease;
        `;

    // Get current date or parse existing value
    let currentDate = new Date();
    if (targetInput.value && targetInput.value.trim()) {
      const parsedDate = parseDateFromDisplay(targetInput.value.trim());
      if (parsedDate) {
        currentDate = parsedDate;
      }
    }

    // Create date picker content
    container.innerHTML = `
            <div style="text-align: center; margin-bottom: 15px;">
                <h3 style="margin: 0; color: #333; font-size: 18px;">Select Date</h3>
            </div>
            <div id="date-picker-calendar"></div>
            <div style="display: flex; gap: 10px; margin-top: 15px;">
                <button id="date-picker-today" style="flex: 1; padding: 8px; border: 1px solid #ddd; background: #f8f9fa; border-radius: 4px; cursor: pointer;">Today</button>
                <button id="date-picker-clear" style="flex: 1; padding: 8px; border: 1px solid #ddd; background: #f8f9fa; border-radius: 4px; cursor: pointer;">Clear</button>
                <button id="date-picker-cancel" style="flex: 1; padding: 8px; border: 1px solid #ddd; background: #f8f9fa; border-radius: 4px; cursor: pointer;">Cancel</button>
            </div>
        `;

    modal.appendChild(container);
    document.body.appendChild(modal);

    // Generate calendar
    generateCalendar(currentDate, targetInput);

    // Add event listeners
    document
      .getElementById("date-picker-today")
      .addEventListener("click", () => {
        const today = new Date();
        const todayFormatted = formatDateForDisplay(today);
        targetInput.value = todayFormatted;
        triggerInputEvents(targetInput);
        modal.remove();
      });

    document
      .getElementById("date-picker-clear")
      .addEventListener("click", () => {
        targetInput.value = "";
        triggerInputEvents(targetInput);
        modal.remove();
      });

    document
      .getElementById("date-picker-cancel")
      .addEventListener("click", () => {
        modal.remove();
      });

    // Close on overlay click
    modal.addEventListener("click", (e) => {
      if (e.target === modal) {
        modal.remove();
      }
    });

    // Add CSS animations
    if (!document.getElementById("date-picker-styles")) {
      const style = document.createElement("style");
      style.id = "date-picker-styles";
      style.textContent = `
                @keyframes fadeIn {
                    from { opacity: 0; }
                    to { opacity: 1; }
                }
                @keyframes slideIn {
                    from { transform: scale(0.9) translateY(-20px); opacity: 0; }
                    to { transform: scale(1) translateY(0); opacity: 1; }
                }
            `;
      document.head.appendChild(style);
    }
  }

  function generateCalendar(currentDate, targetInput) {
    const calendarContainer = document.getElementById("date-picker-calendar");
    const year = currentDate.getFullYear();
    const month = currentDate.getMonth();

    // Create month/year header with navigation
    const header = document.createElement("div");
    header.style.cssText = `
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 15px;
            padding: 0 5px;
        `;

    const prevButton = document.createElement("button");
    prevButton.innerHTML = "‹";
    prevButton.style.cssText = `
            background: none;
            border: none;
            font-size: 20px;
            cursor: pointer;
            padding: 5px 10px;
            border-radius: 4px;
        `;
    prevButton.addEventListener("click", () => {
      const newDate = new Date(year, month - 1, 1);
      generateCalendar(newDate, targetInput);
    });

    const monthYear = document.createElement("span");
    monthYear.style.cssText = `
            font-weight: bold;
            font-size: 16px;
            color: #333;
        `;
    monthYear.textContent = currentDate.toLocaleDateString("en-GB", {
      month: "long",
      year: "numeric",
    });

    const nextButton = document.createElement("button");
    nextButton.innerHTML = "›";
    nextButton.style.cssText = `
            background: none;
            border: none;
            font-size: 20px;
            cursor: pointer;
            padding: 5px 10px;
            border-radius: 4px;
        `;
    nextButton.addEventListener("click", () => {
      const newDate = new Date(year, month + 1, 1);
      generateCalendar(newDate, targetInput);
    });

    header.appendChild(prevButton);
    header.appendChild(monthYear);
    header.appendChild(nextButton);

    // Create calendar grid
    const grid = document.createElement("div");
    grid.style.cssText = `
            display: grid;
            grid-template-columns: repeat(7, 1fr);
            gap: 2px;
            text-align: center;
        `;

    // Add day headers
    const dayHeaders = ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"];
    dayHeaders.forEach((day) => {
      const dayHeader = document.createElement("div");
      dayHeader.style.cssText = `
                padding: 8px 4px;
                font-weight: bold;
                color: #666;
                font-size: 12px;
            `;
      dayHeader.textContent = day;
      grid.appendChild(dayHeader);
    });

    // Get first day of month and number of days
    const firstDay = new Date(year, month, 1).getDay();
    const daysInMonth = new Date(year, month + 1, 0).getDate();
    const today = new Date();

    // Add empty cells for days before month starts
    for (let i = 0; i < firstDay; i++) {
      const emptyCell = document.createElement("div");
      emptyCell.style.cssText = `padding: 8px 4px;`;
      grid.appendChild(emptyCell);
    }

    // Add days of the month
    for (let day = 1; day <= daysInMonth; day++) {
      const dayCell = document.createElement("button");
      const cellDate = new Date(year, month, day);
      const isToday = cellDate.toDateString() === today.toDateString();

      dayCell.textContent = day;
      dayCell.style.cssText = `
                padding: 8px 4px;
                border: none;
                background: ${isToday ? "#e3f2fd" : "transparent"};
                cursor: pointer;
                border-radius: 4px;
                font-size: 14px;
                color: ${isToday ? "#1976d2" : "#333"};
                font-weight: ${isToday ? "bold" : "normal"};
            `;

      dayCell.addEventListener("mouseenter", () => {
        if (!isToday) {
          dayCell.style.backgroundColor = "#f5f5f5";
        }
      });

      dayCell.addEventListener("mouseleave", () => {
        if (!isToday) {
          dayCell.style.backgroundColor = "transparent";
        }
      });

      dayCell.addEventListener("click", () => {
        const selectedDate = new Date(year, month, day);
        const formattedDate = formatDateForDisplay(selectedDate);
        targetInput.value = formattedDate;
        triggerInputEvents(targetInput);
        document.getElementById("custom-date-picker-modal").remove();
      });

      grid.appendChild(dayCell);
    }

    // Clear and populate calendar container
    calendarContainer.innerHTML = "";
    calendarContainer.appendChild(header);
    calendarContainer.appendChild(grid);
  }

  function triggerInputEvents(input) {
    // Trigger input event
    const inputEvent = new Event("input", { bubbles: true });
    input.dispatchEvent(inputEvent);

    // Trigger change event
    const changeEvent = new Event("change", { bubbles: true });
    input.dispatchEvent(changeEvent);

    // Trigger validation
    if (typeof validateAndFormatDate === "function") {
      validateAndFormatDate({ target: input });
    }
  }

  // Make all functions globally accessible
  window.openDatePicker = openDatePicker;
  window.openDatePickerSimple = openDatePickerSimple;
  window.openDatePickerDirect = openDatePickerDirect;

  // Export functions for use by other scripts
  window.Dashboard = {
    refreshSection: refreshSection,
    updateFilters: function (filters) {
      currentFilters = { ...currentFilters, ...filters };
    },
    getCurrentFilters: function () {
      return { ...currentFilters };
    },
    refreshCharts: refreshCharts,
    startAutoRefresh: startAutoRefresh,
    stopAutoRefresh: stopAutoRefresh,
    performManualRefresh: performManualRefresh,
    updateRefreshStatus: updateRefreshStatus,
    refreshDashboardData: refreshDashboardData,
    charts: charts,
    handleError: handleError,
  };
})();
