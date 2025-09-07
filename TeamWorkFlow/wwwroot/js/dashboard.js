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
        // Initial data is loaded server-side, but we can initialize charts here
        initializeCharts();
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
        showLoading();
        
        // Refresh all sections
        Promise.all([
            refreshSection('efficiency'),
            refreshSection('operators'),
            refreshSection('bottlenecks'),
            refreshSection('trends')
        ]).then(() => {
            hideLoading();
            showSuccess('Dashboard data refreshed successfully.');
        }).catch(error => {
            hideLoading();
            showError('Failed to refresh dashboard data: ' + error.message);
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
        // This would update the specific section with new data
        // For now, we'll just log the data since we need the partial views
        console.log(`Updated ${sectionType} section with data:`, data);
        
        // In a full implementation, this would:
        // 1. Update charts with new data
        // 2. Update tables and lists
        // 3. Refresh KPI cards
        // 4. Update trend indicators
    }

    function initializeCharts() {
        // Initialize Chart.js charts
        // This will be implemented in Step 8 when we create the chart components
        console.log('Charts will be initialized in Step 8');
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

    // Export functions for use by other scripts
    window.Dashboard = {
        refreshSection: refreshSection,
        updateFilters: function(filters) {
            currentFilters = { ...currentFilters, ...filters };
        },
        getCurrentFilters: function() {
            return { ...currentFilters };
        }
    };

})();
