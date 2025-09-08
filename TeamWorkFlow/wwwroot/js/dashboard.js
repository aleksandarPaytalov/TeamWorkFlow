// Dashboard JavaScript
(function() {
    'use strict';

    // Dashboard state
    let charts = {};
    let currentFilters = {};

    // Initialize dashboard when DOM is loaded
    document.addEventListener('DOMContentLoaded', function() {
        initializeDashboard();
        setupEventListeners();
        loadInitialData();
    });

    function initializeDashboard() {
        console.log('Initializing Performance Dashboard...');
        
        // Get current filter values from the form
        currentFilters = {
            fromDate: document.getElementById('fromDate')?.value || null,
            toDate: document.getElementById('toDate')?.value || null,
            granularity: document.getElementById('granularity')?.value || 'weekly'
        };
    }

    function setupEventListeners() {
        // Filter controls
        const applyFiltersBtn = document.getElementById('apply-filters');
        const refreshDataBtn = document.getElementById('refresh-data');
        const retryLoadBtn = document.getElementById('retry-load');

        if (applyFiltersBtn) {
            applyFiltersBtn.addEventListener('click', handleApplyFilters);
        }

        if (refreshDataBtn) {
            refreshDataBtn.addEventListener('click', handleRefreshData);
        }

        if (retryLoadBtn) {
            retryLoadBtn.addEventListener('click', handleRetryLoad);
        }

        // Section refresh buttons
        const refreshButtons = document.querySelectorAll('[id^="refresh-"]');
        refreshButtons.forEach(button => {
            button.addEventListener('click', function() {
                const sectionType = this.id.replace('refresh-', '');
                refreshSection(sectionType);
            });
        });

        // Operator sorting
        const operatorSort = document.getElementById('operator-sort');
        if (operatorSort) {
            operatorSort.addEventListener('change', function() {
                refreshSection('operators', { sortBy: this.value });
            });
        }

        // Export buttons (Admin only)
        const exportPdfBtn = document.getElementById('export-pdf');
        const exportExcelBtn = document.getElementById('export-excel');

        if (exportPdfBtn) {
            exportPdfBtn.addEventListener('click', handleExportPdf);
        }

        if (exportExcelBtn) {
            exportExcelBtn.addEventListener('click', handleExportExcel);
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
            trackPerformance('initialization', startTime);

        } catch (error) {
            handleError(error, 'initialization');
        }
    }

    function handleApplyFilters() {
        showLoading();
        
        // Get filter values
        const fromDate = document.getElementById('fromDate')?.value;
        const toDate = document.getElementById('toDate')?.value;
        const granularity = document.getElementById('granularity')?.value;

        // Validate date range
        if (fromDate && toDate && new Date(fromDate) > new Date(toDate)) {
            hideLoading();
            showError('From date cannot be later than to date.');
            return;
        }

        // Update current filters
        currentFilters = { fromDate, toDate, granularity };

        // Reload page with new filters
        const params = new URLSearchParams();
        if (fromDate) params.append('fromDate', fromDate);
        if (toDate) params.append('toDate', toDate);
        if (granularity) params.append('granularity', granularity);

        window.location.href = `/Dashboard?${params.toString()}`;
    }

    function handleRefreshData() {
        const startTime = performance.now();
        showLoading();

        // Refresh all sections
        Promise.all([
            refreshSection('efficiency'),
            refreshSection('operators'),
            refreshSection('bottlenecks'),
            refreshSection('trends')
        ]).then(() => {
            hideLoading();
            trackPerformance('data refresh', startTime);
            showSuccess('Dashboard data refreshed successfully.');

            // Refresh charts with new data
            refreshCharts();

        }).catch(error => {
            hideLoading();
            handleError(error, 'refresh');
        });
    }

    function handleRetryLoad() {
        window.location.reload();
    }

    function refreshSection(sectionType, additionalParams = {}) {
        return new Promise((resolve, reject) => {
            const params = { ...currentFilters, ...additionalParams };
            let endpoint = '';

            switch (sectionType) {
                case 'efficiency':
                    endpoint = '/Dashboard/GetEfficiencyData';
                    break;
                case 'operators':
                    endpoint = '/Dashboard/GetOperatorData';
                    break;
                case 'bottlenecks':
                    endpoint = '/Dashboard/GetBottleneckData';
                    break;
                case 'trends':
                    endpoint = '/Dashboard/GetTrendData';
                    break;
                default:
                    reject(new Error('Unknown section type: ' + sectionType));
                    return;
            }

            // Build query string
            const queryParams = new URLSearchParams();
            Object.keys(params).forEach(key => {
                if (params[key] !== null && params[key] !== undefined) {
                    queryParams.append(key, params[key]);
                }
            });

            fetch(`${endpoint}?${queryParams.toString()}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': getAntiForgeryToken()
                }
            })
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP ${response.status}: ${response.statusText}`);
                }
                return response.json();
            })
            .then(data => {
                updateSectionContent(sectionType, data);
                resolve(data);
            })
            .catch(error => {
                console.error(`Error refreshing ${sectionType} section:`, error);
                reject(error);
            });
        });
    }

    function updateSectionContent(sectionType, data) {
        console.log(`Updating ${sectionType} section with data:`, data);

        try {
            switch (sectionType) {
                case 'efficiency':
                    updateEfficiencySection(data);
                    break;
                case 'operators':
                    updateOperatorSection(data);
                    break;
                case 'bottlenecks':
                    updateBottleneckSection(data);
                    break;
                case 'trends':
                    updateTrendSection(data);
                    break;
                default:
                    console.warn('Unknown section type:', sectionType);
            }
        } catch (error) {
            console.error(`Error updating ${sectionType} section:`, error);
        }
    }

    function updateEfficiencySection(data) {
        // Update KPI cards
        updateKPICard('on-time-rate', data.onTimeCompletionRate, '%');
        updateKPICard('avg-overrun', data.averageTimeOverrunPercentage, '%');
        updateKPICard('tasks-completed', data.totalTasksCompleted, '');
        updateKPICard('efficiency-score', data.overallEfficiencyScore, '');

        // Update efficiency chart
        if (charts.efficiency && data.trendData) {
            const chartData = {
                labels: data.trendData.map(d => d.dateFormatted || d.periodLabel),
                data: data.trendData.map(d => d.onTimeRate || d.value)
            };
            updateChartDataSafe(charts.efficiency, chartData, 'efficiency');
        }

        // Update trend indicators
        updateTrendIndicator('efficiency-trend', data.efficiencyTrend);
    }

    function updateOperatorSection(data) {
        // Update operator performance chart
        if (charts.operator && data.operators) {
            const chartColors = {
                success: '#10b981',
                warning: '#f59e0b',
                danger: '#ef4444'
            };

            const chartData = {
                labels: data.operators.map(op => op.operatorName),
                data: data.operators.map(op => op.efficiencyRating)
            };

            const backgroundColors = data.operators.map(op => {
                if (op.efficiencyRating >= 85) return chartColors.success;
                if (op.efficiencyRating >= 70) return chartColors.warning;
                return chartColors.danger;
            });

            charts.operator.data.datasets[0].backgroundColor = backgroundColors;
            updateChartDataSafe(charts.operator, chartData, 'operator');
        }

        // Update operator table
        updateOperatorTable(data.operators);

        // Update summary cards
        if (data.summary) {
            updateKPICard('top-performers', data.summary.topPerformers, '');
            updateKPICard('team-average', data.summary.teamAverage, '%');
            updateKPICard('needs-attention', data.summary.needsAttention, '');
        }
    }

    function updateBottleneckSection(data) {
        // Update bottleneck chart
        if (charts.bottleneck && data.delaysByCategory) {
            const chartData = {
                labels: data.delaysByCategory.map(d => d.categoryName),
                data: data.delaysByCategory.map(d => d.delayCount)
            };
            updateChartDataSafe(charts.bottleneck, chartData, 'bottleneck');
        }

        // Update bottleneck stats
        updateKPICard('total-bottlenecks', data.totalBottlenecks, '');
        updateKPICard('avg-delay', data.averageDelayHours, 'h');
        updateKPICard('tasks-affected', data.tasksAffectedPercentage, '%');
        updateKPICard('severity-score', data.severityScore, '');

        // Update bottleneck list
        updateBottleneckList(data.frequentBottleneckTasks);
    }

    function updateTrendSection(data) {
        // Update main trend chart
        if (charts.mainTrend && data.trendData) {
            charts.mainTrend.data.labels = data.timeLabels || [];

            if (data.completionTrendData) {
                charts.mainTrend.data.datasets[0].data = data.completionTrendData.map(d => d.value);
            }
            if (data.efficiencyTrendData) {
                charts.mainTrend.data.datasets[1].data = data.efficiencyTrendData.map(d => d.value);
            }
            if (data.workloadTrendData) {
                charts.mainTrend.data.datasets[2].data = data.workloadTrendData.map(d => d.value);
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
            'completion-trend-chart': data.completionTrendData,
            'efficiency-trend-chart': data.efficiencyTrendData,
            'workload-trend-chart': data.workloadTrendData,
            'variance-trend-chart': data.varianceTrendData
        };

        Object.keys(chartMappings).forEach(chartId => {
            if (charts[chartId] && chartMappings[chartId]) {
                const chartData = {
                    labels: chartMappings[chartId].map(d => d.dateFormatted || d.label),
                    data: chartMappings[chartId].map(d => d.value)
                };
                updateChartDataSafe(charts[chartId], chartData, chartId);
            }
        });
    }

    // Helper functions for updating UI elements
    function updateKPICard(cardId, value, suffix = '') {
        const element = document.getElementById(cardId);
        if (element) {
            const valueElement = element.querySelector('.metric-value, .stat-value, .summary-value');
            if (valueElement) {
                const formattedValue = typeof value === 'number' ? value.toFixed(1) : value;
                valueElement.textContent = formattedValue + suffix;
            }
        }
    }

    function updateTrendIndicator(indicatorId, trendValue) {
        const element = document.getElementById(indicatorId);
        if (element && typeof trendValue === 'number') {
            const icon = element.querySelector('i');
            const text = element.querySelector('.trend-text');

            if (icon) {
                icon.className = trendValue >= 0 ? 'fas fa-arrow-up' : 'fas fa-arrow-down';
            }

            if (text) {
                text.textContent = Math.abs(trendValue).toFixed(1) + '% vs last period';
            }

            element.className = element.className.replace(/positive|negative/g, '') +
                               (trendValue >= 0 ? ' positive' : ' negative');
        }
    }

    function updateOperatorTable(operators) {
        const tableBody = document.querySelector('#operator-table tbody');
        if (!tableBody || !operators) return;

        tableBody.innerHTML = '';

        operators.forEach((operator, index) => {
            const row = document.createElement('tr');
            row.className = operator.isTopPerformer ? 'top-performer' :
                           operator.needsAttention ? 'needs-attention' : '';

            row.innerHTML = `
                <td><span class="${operator.rankClass || ''}">${index + 1}</span></td>
                <td>
                    <div class="operator-info">
                        <strong>${operator.operatorName}</strong>
                        <small class="text-muted d-block">${operator.operatorEmail || ''}</small>
                    </div>
                </td>
                <td>${operator.tasksCompleted || 0}</td>
                <td><span class="${operator.efficiencyClass || ''}">${operator.efficiencyRating?.toFixed(1) || '0'}%</span></td>
                <td>${operator.onTimeCompletionRate?.toFixed(1) || '0'}%</td>
                <td>${operator.averageCompletionTimeHours?.toFixed(1) || '0'}h</td>
                <td>
                    <span class="badge ${operator.isTopPerformer ? 'bg-success' : operator.needsAttention ? 'bg-warning' : 'bg-secondary'}">
                        ${operator.performanceStatus || 'Normal'}
                    </span>
                </td>
                <td>
                    <span class="${operator.trendClass || ''}">
                        <i class="${operator.trendIcon || 'fas fa-minus'}"></i>
                        ${operator.performanceTrend?.toFixed(1) || '0'}%
                    </span>
                </td>
            `;

            tableBody.appendChild(row);
        });
    }

    function updateBottleneckList(bottlenecks) {
        const listContainer = document.querySelector('.bottleneck-list');
        if (!listContainer || !bottlenecks) return;

        listContainer.innerHTML = '';

        bottlenecks.slice(0, 10).forEach(bottleneck => {
            const item = document.createElement('div');
            item.className = 'bottleneck-item';

            const delayPercentage = Math.min(100, (bottleneck.averageDelayHours / 8 * 100));

            item.innerHTML = `
                <div class="bottleneck-header">
                    <div class="bottleneck-title">
                        <strong>${bottleneck.taskName}</strong>
                        <span class="project-name">(${bottleneck.projectName})</span>
                    </div>
                    <div class="bottleneck-metrics">
                        <span class="occurrences">${bottleneck.occurrences} times</span>
                        <span class="avg-delay">${bottleneck.averageDelayHours?.toFixed(1) || '0'}h avg</span>
                    </div>
                </div>
                <div class="bottleneck-progress">
                    <div class="progress">
                        <div class="progress-bar bg-warning" style="width: ${delayPercentage.toFixed(0)}%"></div>
                    </div>
                    <small class="text-muted">Total delay: ${bottleneck.totalDelayHours?.toFixed(1) || '0'}h</small>
                </div>
            `;

            listContainer.appendChild(item);
        });
    }

    function updateTrendOverview(data) {
        if (data.overallTrendDirection) {
            updateKPICard('overall-trend', data.overallTrendDirection, '');
        }
        if (data.trendConfidence) {
            updateKPICard('trend-confidence', data.trendConfidence, '%');
        }
        if (data.totalDataPoints) {
            updateKPICard('data-points', data.totalDataPoints, '');
        }
        if (data.analysisPeriod) {
            updateKPICard('analysis-period', data.analysisPeriod, '');
        }
    }

    function initializeCharts() {
        console.log('Initializing dashboard charts...');

        if (typeof Chart === 'undefined') {
            console.warn('Chart.js not loaded. Charts will not be available.');
            return;
        }

        // Set global Chart.js defaults
        Chart.defaults.font.family = "'Inter', 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif";
        Chart.defaults.color = '#374151';
        Chart.defaults.plugins.legend.position = 'top';

        // Chart color palette
        const chartColors = {
            primary: '#3b82f6',
            success: '#10b981',
            warning: '#f59e0b',
            danger: '#ef4444',
            info: '#06b6d4',
            secondary: '#6b7280'
        };

        // Initialize individual charts
        initializeEfficiencyChart(chartColors);
        initializeOperatorChart(chartColors);
        initializeBottleneckChart(chartColors);
        initializeTrendCharts(chartColors);

        console.log('Dashboard charts initialized successfully');
    }

    function initializeEfficiencyChart(colors) {
        const ctx = document.getElementById('efficiency-comparison-chart');
        if (!ctx) return;

        try {
            charts.efficiency = new Chart(ctx.getContext('2d'), {
                type: 'line',
                data: {
                    labels: [],
                    datasets: [{
                        label: 'On-Time Rate (%)',
                        data: [],
                        borderColor: colors.primary,
                        backgroundColor: colors.primary + '20',
                        tension: 0.4,
                        fill: true,
                        pointBackgroundColor: colors.primary,
                        pointBorderColor: '#ffffff',
                        pointBorderWidth: 2,
                        pointRadius: 4
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            display: true,
                            position: 'top'
                        },
                        tooltip: {
                            mode: 'index',
                            intersect: false,
                            callbacks: {
                                label: function(context) {
                                    return context.dataset.label + ': ' + context.parsed.y.toFixed(1) + '%';
                                }
                            }
                        }
                    },
                    scales: {
                        x: {
                            display: true,
                            grid: {
                                display: false
                            }
                        },
                        y: {
                            beginAtZero: true,
                            max: 100,
                            ticks: {
                                callback: function(value) {
                                    return value + '%';
                                }
                            },
                            grid: {
                                color: '#f3f4f6'
                            }
                        }
                    },
                    interaction: {
                        mode: 'nearest',
                        axis: 'x',
                        intersect: false
                    }
                }
            });

            // Load data if available
            loadEfficiencyChartData();

        } catch (error) {
            console.error('Error initializing efficiency chart:', error);
        }
    }

    function initializeOperatorChart(colors) {
        const ctx = document.getElementById('operator-performance-chart');
        if (!ctx) return;

        try {
            charts.operator = new Chart(ctx.getContext('2d'), {
                type: 'bar',
                data: {
                    labels: [],
                    datasets: [{
                        label: 'Efficiency Rating (%)',
                        data: [],
                        backgroundColor: [],
                        borderColor: '#dee2e6',
                        borderWidth: 1,
                        borderRadius: 4,
                        borderSkipped: false
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            display: false
                        },
                        tooltip: {
                            callbacks: {
                                label: function(context) {
                                    return context.dataset.label + ': ' + context.parsed.y.toFixed(1) + '%';
                                },
                                afterLabel: function(context) {
                                    const rating = context.parsed.y;
                                    if (rating >= 85) return 'Performance: Excellent';
                                    if (rating >= 70) return 'Performance: Good';
                                    return 'Performance: Needs Improvement';
                                }
                            }
                        }
                    },
                    scales: {
                        x: {
                            display: true,
                            grid: {
                                display: false
                            },
                            ticks: {
                                maxRotation: 45,
                                minRotation: 0
                            }
                        },
                        y: {
                            beginAtZero: true,
                            max: 100,
                            ticks: {
                                callback: function(value) {
                                    return value + '%';
                                }
                            },
                            grid: {
                                color: '#f3f4f6'
                            }
                        }
                    }
                }
            });

            // Load data if available
            loadOperatorChartData(colors);

        } catch (error) {
            console.error('Error initializing operator chart:', error);
        }
    }

    function initializeBottleneckChart(colors) {
        const ctx = document.getElementById('bottleneck-categories-chart');
        if (!ctx) return;

        try {
            charts.bottleneck = new Chart(ctx.getContext('2d'), {
                type: 'doughnut',
                data: {
                    labels: [],
                    datasets: [{
                        data: [],
                        backgroundColor: [
                            colors.danger,
                            colors.warning,
                            colors.info,
                            colors.success,
                            colors.primary,
                            colors.secondary
                        ],
                        borderWidth: 2,
                        borderColor: '#ffffff',
                        hoverBorderWidth: 3
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            position: 'right',
                            labels: {
                                usePointStyle: true,
                                padding: 20
                            }
                        },
                        tooltip: {
                            callbacks: {
                                label: function(context) {
                                    const total = context.dataset.data.reduce((a, b) => a + b, 0);
                                    const percentage = ((context.parsed / total) * 100).toFixed(1);
                                    return context.label + ': ' + context.parsed + ' (' + percentage + '%)';
                                }
                            }
                        }
                    },
                    cutout: '60%'
                }
            });

            // Load data if available
            loadBottleneckChartData();

        } catch (error) {
            console.error('Error initializing bottleneck chart:', error);
        }
    }

    function initializeTrendCharts(colors) {
        // Main trend chart
        const mainTrendCtx = document.getElementById('main-trend-chart');
        if (mainTrendCtx) {
            try {
                charts.mainTrend = new Chart(mainTrendCtx.getContext('2d'), {
                    type: 'line',
                    data: {
                        labels: [],
                        datasets: [
                            {
                                label: 'Completion Rate (%)',
                                data: [],
                                borderColor: colors.primary,
                                backgroundColor: colors.primary + '20',
                                tension: 0.4,
                                yAxisID: 'y'
                            },
                            {
                                label: 'Efficiency Score (%)',
                                data: [],
                                borderColor: colors.success,
                                backgroundColor: colors.success + '20',
                                tension: 0.4,
                                yAxisID: 'y'
                            },
                            {
                                label: 'Workload (Tasks)',
                                data: [],
                                borderColor: colors.warning,
                                backgroundColor: colors.warning + '20',
                                tension: 0.4,
                                yAxisID: 'y1'
                            }
                        ]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            legend: {
                                position: 'top'
                            }
                        },
                        scales: {
                            x: {
                                display: true,
                                grid: {
                                    display: false
                                }
                            },
                            y: {
                                type: 'linear',
                                display: true,
                                position: 'left',
                                beginAtZero: true,
                                max: 100,
                                ticks: {
                                    callback: function(value) {
                                        return value + '%';
                                    }
                                }
                            },
                            y1: {
                                type: 'linear',
                                display: true,
                                position: 'right',
                                beginAtZero: true,
                                grid: {
                                    drawOnChartArea: false,
                                },
                                ticks: {
                                    callback: function(value) {
                                        return value + ' tasks';
                                    }
                                }
                            }
                        }
                    }
                });
            } catch (error) {
                console.error('Error initializing main trend chart:', error);
            }
        }

        // Secondary trend charts
        const secondaryCharts = [
            { id: 'completion-trend-chart', label: 'Completion Rate', color: colors.primary },
            { id: 'efficiency-trend-chart', label: 'Efficiency Score', color: colors.success },
            { id: 'workload-trend-chart', label: 'Workload', color: colors.warning },
            { id: 'variance-trend-chart', label: 'Variance', color: colors.danger }
        ];

        secondaryCharts.forEach(config => {
            const ctx = document.getElementById(config.id);
            if (ctx) {
                try {
                    charts[config.id] = new Chart(ctx.getContext('2d'), {
                        type: 'line',
                        data: {
                            labels: [],
                            datasets: [{
                                label: config.label,
                                data: [],
                                borderColor: config.color,
                                backgroundColor: config.color + '20',
                                tension: 0.4,
                                fill: true,
                                pointRadius: 3,
                                pointHoverRadius: 5
                            }]
                        },
                        options: {
                            responsive: true,
                            maintainAspectRatio: false,
                            plugins: {
                                legend: {
                                    display: false
                                }
                            },
                            scales: {
                                x: {
                                    display: true,
                                    grid: {
                                        display: false
                                    }
                                },
                                y: {
                                    beginAtZero: true,
                                    grid: {
                                        color: '#f3f4f6'
                                    }
                                }
                            }
                        }
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
        const chartData = getChartDataFromPage('efficiency-chart-data');
        if (chartData && charts.efficiency) {
            updateChartData(charts.efficiency, chartData);
        }
    }

    function loadOperatorChartData(colors) {
        const chartData = getChartDataFromPage('operator-chart-data');
        if (chartData && charts.operator) {
            // Generate colors based on performance
            const backgroundColors = chartData.data.map(value => {
                if (value >= 85) return colors.success;
                if (value >= 70) return colors.warning;
                return colors.danger;
            });

            charts.operator.data.datasets[0].backgroundColor = backgroundColors;
            updateChartData(charts.operator, chartData);
        }
    }

    function loadBottleneckChartData() {
        const chartData = getChartDataFromPage('bottleneck-chart-data');
        if (chartData && charts.bottleneck) {
            updateChartData(charts.bottleneck, chartData);
        }
    }

    function loadTrendChartsData() {
        const trendData = getChartDataFromPage('trend-chart-data');
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
                'completion-trend-chart': trendData.completion,
                'efficiency-trend-chart': trendData.efficiency,
                'workload-trend-chart': trendData.workload,
                'variance-trend-chart': trendData.variance
            };

            Object.keys(secondaryData).forEach(chartId => {
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
        console.log('Refreshing all charts...');

        loadEfficiencyChartData();
        loadOperatorChartData({
            primary: '#3b82f6',
            success: '#10b981',
            warning: '#f59e0b',
            danger: '#ef4444'
        });
        loadBottleneckChartData();
        loadTrendChartsData();
    }

    function handleExportPdf() {
        showLoading();
        
        fetch('/Dashboard/ExportPdf', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiForgeryToken()
            },
            body: JSON.stringify(currentFilters)
        })
        .then(response => {
            hideLoading();
            if (response.ok) {
                // Handle PDF download
                return response.blob();
            } else {
                throw new Error('Failed to generate PDF report');
            }
        })
        .then(blob => {
            // Create download link
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `dashboard-report-${new Date().toISOString().split('T')[0]}.pdf`;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);
        })
        .catch(error => {
            hideLoading();
            showError('Failed to export PDF: ' + error.message);
        });
    }

    function handleExportExcel() {
        showLoading();
        
        fetch('/Dashboard/ExportExcel', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiForgeryToken()
            },
            body: JSON.stringify(currentFilters)
        })
        .then(response => {
            hideLoading();
            if (response.ok) {
                // Handle Excel download
                return response.blob();
            } else {
                throw new Error('Failed to generate Excel report');
            }
        })
        .then(blob => {
            // Create download link
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `dashboard-report-${new Date().toISOString().split('T')[0]}.xlsx`;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);
        })
        .catch(error => {
            hideLoading();
            showError('Failed to export Excel: ' + error.message);
        });
    }

    // Utility functions
    function showLoading() {
        const loadingIndicator = document.getElementById('loading-indicator');
        if (loadingIndicator) {
            loadingIndicator.style.display = 'flex';
        }
    }

    function hideLoading() {
        const loadingIndicator = document.getElementById('loading-indicator');
        if (loadingIndicator) {
            loadingIndicator.style.display = 'none';
        }
    }

    function showSuccess(message) {
        // Use toastr if available, otherwise console log
        if (typeof toastr !== 'undefined') {
            toastr.success(message);
        } else {
            console.log('Success:', message);
        }
    }

    function showError(message) {
        // Use toastr if available, otherwise console error
        if (typeof toastr !== 'undefined') {
            toastr.error(message);
        } else {
            console.error('Error:', message);
        }
    }

    function getAntiForgeryToken() {
        const token = document.querySelector('input[name="__RequestVerificationToken"]');
        return token ? token.value : '';
    }

    // Enhanced Auto-refresh functionality with real-time capabilities
    let autoRefreshInterval = null;
    let autoRefreshEnabled = false;
    let refreshInProgress = false;
    let lastRefreshTime = null;
    let refreshFailureCount = 0;
    let refreshStatusInterval = null;
    const MAX_REFRESH_FAILURES = 3;
    const REFRESH_RETRY_DELAY = 5000; // 5 seconds

    function initializeAutoRefresh() {
        const autoRefreshToggle = document.getElementById('auto-refresh-toggle');
        const autoRefreshIntervalSelect = document.getElementById('auto-refresh-interval');
        const manualRefreshBtn = document.getElementById('manual-refresh-btn');
        const refreshStatusBtn = document.getElementById('refresh-status-btn');

        if (autoRefreshToggle) {
            autoRefreshToggle.addEventListener('change', function() {
                if (this.checked) {
                    startAutoRefresh();
                } else {
                    stopAutoRefresh();
                }
            });
        }

        if (autoRefreshIntervalSelect) {
            autoRefreshIntervalSelect.addEventListener('change', function() {
                if (autoRefreshEnabled) {
                    stopAutoRefresh();
                    startAutoRefresh();
                }
            });
        }

        if (manualRefreshBtn) {
            manualRefreshBtn.addEventListener('click', function() {
                performManualRefresh();
            });
        }

        if (refreshStatusBtn) {
            refreshStatusBtn.addEventListener('click', function() {
                updateRefreshStatus();
            });
        }

        // Initialize refresh status display
        initializeRefreshStatus();

        // Set up periodic status updates
        refreshStatusInterval = setInterval(updateRefreshStatus, 1000);
    }

    function startAutoRefresh() {
        const intervalSelect = document.getElementById('auto-refresh-interval');
        const intervalMinutes = intervalSelect ? parseInt(intervalSelect.value) : 5;
        const intervalMs = intervalMinutes * 60 * 1000;

        autoRefreshEnabled = true;
        refreshFailureCount = 0; // Reset failure count

        autoRefreshInterval = setInterval(() => {
            console.log('Auto-refreshing dashboard data...');
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
        autoRefreshEnabled = false;
        updateRefreshStatus();
        showSuccess('Auto-refresh disabled');
    }

    function performManualRefresh() {
        if (refreshInProgress) {
            showWarning('Refresh already in progress. Please wait...');
            return;
        }

        console.log('Manual refresh triggered');
        performDataRefresh(true); // true = manual refresh
    }

    async function performDataRefresh(isManual = false) {
        if (refreshInProgress) {
            console.log('Refresh already in progress, skipping...');
            return;
        }

        refreshInProgress = true;
        updateRefreshStatus();

        try {
            const refreshType = isManual ? 'Manual' : 'Auto';
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
                showSuccess('Dashboard data refreshed successfully');
            }

        } catch (error) {
            refreshFailureCount++;
            console.error('Refresh failed:', error);

            if (isManual) {
                showError('Failed to refresh dashboard data. Please try again.');
            }

            // Auto-disable refresh after max failures
            if (refreshFailureCount >= MAX_REFRESH_FAILURES && autoRefreshEnabled) {
                stopAutoRefresh();
                showError(`Auto-refresh disabled after ${MAX_REFRESH_FAILURES} consecutive failures`);
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
        document.addEventListener('visibilitychange', function() {
            if (document.hidden && autoRefreshEnabled) {
                console.log('Tab hidden, pausing auto-refresh');
            } else if (!document.hidden && autoRefreshEnabled) {
                console.log('Tab visible, resuming auto-refresh');
                updateRefreshStatus();
            }
        });
    }

    function updateRefreshStatus() {
        const statusElement = document.getElementById('refresh-status');
        const lastRefreshElement = document.getElementById('last-refresh-time');
        const nextRefreshElement = document.getElementById('next-refresh-time');

        if (statusElement) {
            let statusText = 'Disabled';
            let statusClass = 'text-muted';

            if (refreshInProgress) {
                statusText = 'Refreshing...';
                statusClass = 'text-info';
            } else if (autoRefreshEnabled) {
                statusText = 'Active';
                statusClass = 'text-success';
            } else if (refreshFailureCount > 0) {
                statusText = `Failed (${refreshFailureCount})`;
                statusClass = 'text-danger';
            }

            statusElement.textContent = statusText;
            statusElement.className = `badge ${statusClass}`;
        }

        if (lastRefreshElement && lastRefreshTime) {
            const timeAgo = getTimeAgo(lastRefreshTime);
            lastRefreshElement.textContent = timeAgo;
        }

        if (nextRefreshElement && autoRefreshEnabled && autoRefreshInterval) {
            const intervalSelect = document.getElementById('auto-refresh-interval');
            const intervalMinutes = intervalSelect ? parseInt(intervalSelect.value) : 5;
            const nextRefresh = new Date(Date.now() + (intervalMinutes * 60 * 1000));
            nextRefreshElement.textContent = nextRefresh.toLocaleTimeString();
        } else if (nextRefreshElement) {
            nextRefreshElement.textContent = 'N/A';
        }
    }

    function showRefreshIndicator(show) {
        const indicator = document.getElementById('refresh-indicator');
        const manualBtn = document.getElementById('manual-refresh-btn');

        if (indicator) {
            indicator.style.display = show ? 'inline-block' : 'none';
        }

        if (manualBtn) {
            manualBtn.disabled = show;
            if (show) {
                manualBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Refreshing...';
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

            console.log('All dashboard sections refreshed successfully');

        } catch (error) {
            console.error('Error refreshing dashboard data:', error);
            throw error;
        }
    }

    async function refreshEfficiencyData(filters) {
        try {
            const response = await fetch('/Dashboard/GetEfficiencyData', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': getAntiForgeryToken()
                },
                body: JSON.stringify(filters)
            });

            if (!response.ok) {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }

            const data = await response.json();
            updateEfficiencyMetrics(data);

        } catch (error) {
            console.error('Error refreshing efficiency data:', error);
            throw error;
        }
    }

    async function refreshOperatorData(filters) {
        try {
            const response = await fetch('/Dashboard/GetOperatorData', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': getAntiForgeryToken()
                },
                body: JSON.stringify(filters)
            });

            if (!response.ok) {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }

            const data = await response.json();
            updateOperatorPerformance(data);

        } catch (error) {
            console.error('Error refreshing operator data:', error);
            throw error;
        }
    }

    async function refreshBottleneckData(filters) {
        try {
            const response = await fetch('/Dashboard/GetBottleneckData', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': getAntiForgeryToken()
                },
                body: JSON.stringify(filters)
            });

            if (!response.ok) {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }

            const data = await response.json();
            updateBottleneckAnalysis(data);

        } catch (error) {
            console.error('Error refreshing bottleneck data:', error);
            throw error;
        }
    }

    async function refreshTrendData(filters) {
        try {
            const response = await fetch('/Dashboard/GetTrendData', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': getAntiForgeryToken()
                },
                body: JSON.stringify(filters)
            });

            if (!response.ok) {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }

            const data = await response.json();
            updateTrendCharts(data);

        } catch (error) {
            console.error('Error refreshing trend data:', error);
            throw error;
        }
    }

    // Data update functions for UI refresh
    function updateEfficiencyMetrics(data) {
        try {
            // Update KPI cards
            const onTimeRate = document.getElementById('on-time-completion-rate');
            const overrunRate = document.getElementById('average-overrun-rate');
            const tasksCompleted = document.getElementById('tasks-completed');
            const efficiencyScore = document.getElementById('efficiency-score');

            if (onTimeRate && data.onTimeCompletionRate !== undefined) {
                onTimeRate.textContent = `${data.onTimeCompletionRate.toFixed(1)}%`;
            }
            if (overrunRate && data.averageTimeOverrunPercentage !== undefined) {
                overrunRate.textContent = `${data.averageTimeOverrunPercentage.toFixed(1)}%`;
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

            console.log('Efficiency metrics updated');
        } catch (error) {
            console.error('Error updating efficiency metrics:', error);
        }
    }

    function updateOperatorPerformance(data) {
        try {
            // Update operator performance table
            const tableBody = document.querySelector('#operator-performance-table tbody');
            if (tableBody && data.operators) {
                tableBody.innerHTML = '';

                data.operators.forEach(operator => {
                    const row = document.createElement('tr');
                    row.innerHTML = `
                        <td>${operator.operatorName}</td>
                        <td>${operator.efficiencyRating.toFixed(1)}%</td>
                        <td>${operator.tasksCompleted}</td>
                        <td>${operator.averageCompletionTimeHours.toFixed(1)}h</td>
                        <td>
                            <span class="badge ${operator.isTopPerformer ? 'bg-success' : operator.needsAttention ? 'bg-warning' : 'bg-secondary'}">
                                ${operator.isTopPerformer ? 'Top Performer' : operator.needsAttention ? 'Needs Attention' : 'Average'}
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

            console.log('Operator performance updated');
        } catch (error) {
            console.error('Error updating operator performance:', error);
        }
    }

    function updateBottleneckAnalysis(data) {
        try {
            // Update bottleneck summary cards
            const totalBottlenecks = document.getElementById('total-bottlenecks');
            const averageDelay = document.getElementById('average-delay');
            const tasksAffected = document.getElementById('tasks-affected');
            const severityScore = document.getElementById('severity-score');

            if (totalBottlenecks && data.totalBottlenecks !== undefined) {
                totalBottlenecks.textContent = data.totalBottlenecks;
            }
            if (averageDelay && data.averageDelayHours !== undefined) {
                averageDelay.textContent = `${data.averageDelayHours.toFixed(1)}h`;
            }
            if (tasksAffected && data.tasksAffectedPercentage !== undefined) {
                tasksAffected.textContent = `${data.tasksAffectedPercentage.toFixed(1)}%`;
            }
            if (severityScore && data.severityScoreFormatted !== undefined) {
                severityScore.textContent = data.severityScoreFormatted;
            }

            // Update bottleneck chart if it exists
            if (charts.bottlenecks && data.chartData) {
                updateChart(charts.bottlenecks, data.chartData);
            }

            console.log('Bottleneck analysis updated');
        } catch (error) {
            console.error('Error updating bottleneck analysis:', error);
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

            console.log('Trend charts updated');
        } catch (error) {
            console.error('Error updating trend charts:', error);
        }
    }

    function updateChart(chart, newData) {
        try {
            if (!chart || !newData) {
                console.warn('Chart or data is null/undefined');
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
            chart.update('active');

        } catch (error) {
            console.error('Error updating chart:', error);
        }
    }

    // Enhanced error handling
    function handleError(error, context = '') {
        console.error(`Dashboard error ${context}:`, error);

        let errorMessage = 'An unexpected error occurred.';

        if (error.name === 'TypeError' && error.message.includes('fetch')) {
            errorMessage = 'Network error. Please check your connection and try again.';
        } else if (error.status === 401) {
            errorMessage = 'Your session has expired. Please log in again.';
            // Redirect to login after a delay
            setTimeout(() => {
                window.location.href = '/Account/Login';
            }, 3000);
        } else if (error.status === 403) {
            errorMessage = 'You do not have permission to access this data.';
        } else if (error.status === 404) {
            errorMessage = 'The requested data could not be found.';
        } else if (error.status >= 500) {
            errorMessage = 'Server error. Please try again later.';
        } else if (error.message) {
            errorMessage = error.message;
        }

        showError(errorMessage);

        // Show retry option for certain errors
        if (error.status >= 500 || error.name === 'TypeError') {
            showRetryOption(context);
        }
    }

    function showRetryOption(context) {
        const retryContainer = document.getElementById('retry-container');
        if (retryContainer) {
            retryContainer.style.display = 'block';

            const retryButton = retryContainer.querySelector('.retry-button');
            if (retryButton) {
                retryButton.onclick = () => {
                    retryContainer.style.display = 'none';
                    if (context === 'refresh') {
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
            console.warn(`Slow dashboard operation detected: ${operation} took ${duration.toFixed(2)}ms`);
        }
    }

    // Responsive chart handling
    function handleResize() {
        Object.keys(charts).forEach(chartKey => {
            if (charts[chartKey] && typeof charts[chartKey].resize === 'function') {
                charts[chartKey].resize();
            }
        });
    }

    // Initialize resize handler
    window.addEventListener('resize', debounce(handleResize, 250));

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
    function updateChartDataSafe(chart, data, chartType = 'unknown') {
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
            chart.update('none'); // Use 'none' animation for better performance
        } catch (error) {
            console.error(`Error updating ${chartType} chart:`, error);
        }
    }

    // Accessibility improvements
    function initializeAccessibility() {
        // Add ARIA labels to charts
        Object.keys(charts).forEach(chartKey => {
            const canvas = document.getElementById(chartKey.replace('charts.', '') + '-chart');
            if (canvas) {
                canvas.setAttribute('role', 'img');
                canvas.setAttribute('aria-label', `${chartKey} performance chart`);
            }
        });

        // Add keyboard navigation for interactive elements
        const interactiveElements = document.querySelectorAll('.chart-container, .kpi-card, .filter-control');
        interactiveElements.forEach(element => {
            if (!element.hasAttribute('tabindex')) {
                element.setAttribute('tabindex', '0');
            }
        });
    }

    // Export functions for use by other scripts
    window.Dashboard = {
        refreshSection: refreshSection,
        updateFilters: function(filters) {
            currentFilters = { ...currentFilters, ...filters };
        },
        getCurrentFilters: function() {
            return { ...currentFilters };
        },
        refreshCharts: refreshCharts,
        startAutoRefresh: startAutoRefresh,
        stopAutoRefresh: stopAutoRefresh,
        performManualRefresh: performManualRefresh,
        updateRefreshStatus: updateRefreshStatus,
        refreshDashboardData: refreshDashboardData,
        charts: charts,
        handleError: handleError
    };

})();
