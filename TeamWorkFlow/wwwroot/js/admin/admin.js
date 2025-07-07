/**
 * Admin Area JavaScript
 * Handles interactions and animations for all admin pages
 */

document.addEventListener('DOMContentLoaded', function() {
    console.log('Admin area initialized');
    
    // Initialize all functionality
    initializeAnimations();
    initializeDashboardCards();
    initializeTableInteractions();
    initializePagination();
    initializeResponsiveFeatures();
    initializeConfirmations();
    initializeTooltips();
    initializeKeyboardNavigation();
    initializeStatusIndicators();
    initializeLoadingStates();
});

/**
 * Initialize animations for admin elements
 */
function initializeAnimations() {
    // Animate dashboard cards on page load
    const cards = document.querySelectorAll('.dashboard-card');
    cards.forEach((card, index) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        
        setTimeout(() => {
            card.style.transition = 'all 0.6s ease';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, index * 100);
    });

    // Animate table rows
    const tableRows = document.querySelectorAll('.admin-table tbody tr');
    tableRows.forEach((row, index) => {
        row.style.opacity = '0';
        row.style.transform = 'translateX(-20px)';
        
        setTimeout(() => {
            row.style.transition = 'all 0.4s ease';
            row.style.opacity = '1';
            row.style.transform = 'translateX(0)';
        }, index * 50);
    });

    // Add fade-in class to containers
    setTimeout(() => {
        document.querySelectorAll('.admin-container, .admin-table-container').forEach(element => {
            element.classList.add('fade-in-admin');
        });
    }, 100);
}

/**
 * Initialize dashboard card interactions
 */
function initializeDashboardCards() {
    const cards = document.querySelectorAll('.dashboard-card');
    
    cards.forEach(card => {
        // Enhanced hover effects
        card.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-8px) scale(1.02)';
            this.style.boxShadow = '0 25px 50px -12px rgba(0, 0, 0, 0.25)';
        });
        
        card.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0) scale(1)';
            this.style.boxShadow = '0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)';
        });

        // Click animation for action buttons
        const actionButton = card.querySelector('.dashboard-card-action');
        if (actionButton) {
            actionButton.addEventListener('mousedown', function() {
                this.style.transform = 'translateY(-1px) scale(0.98)';
            });
            
            actionButton.addEventListener('mouseup', function() {
                this.style.transform = 'translateY(-2px) scale(1)';
            });
        }
    });
}

/**
 * Initialize table interactions
 */
function initializeTableInteractions() {
    const tables = document.querySelectorAll('.admin-table');
    
    tables.forEach(table => {
        const rows = table.querySelectorAll('tbody tr');
        
        rows.forEach(row => {
            // Enhanced hover effects
            row.addEventListener('mouseenter', function() {
                this.style.background = '#f8fafc';
                this.style.transform = 'translateX(4px)';
                this.style.boxShadow = '4px 0 8px -2px rgba(0, 0, 0, 0.1)';
            });
            
            row.addEventListener('mouseleave', function() {
                this.style.background = '';
                this.style.transform = 'translateX(0)';
                this.style.boxShadow = '';
            });
        });
    });

    // Initialize sortable headers (if needed)
    const sortableHeaders = document.querySelectorAll('.admin-table th[data-sortable]');
    sortableHeaders.forEach(header => {
        header.style.cursor = 'pointer';
        header.addEventListener('click', function() {
            // Add sorting logic here if needed
            console.log('Sorting by:', this.textContent);
        });
    });
}

/**
 * Initialize pagination functionality
 */
function initializePagination() {
    const paginationLinks = document.querySelectorAll('.admin-page-link');
    
    paginationLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            if (this.parentElement.classList.contains('disabled')) {
                e.preventDefault();
                return;
            }
            
            // Add loading state
            const tableContainer = document.querySelector('.admin-table-container');
            if (tableContainer) {
                tableContainer.classList.add('admin-loading');
            }
        });
    });
}

/**
 * Initialize responsive features
 */
function initializeResponsiveFeatures() {
    let resizeTimeout;
    
    window.addEventListener('resize', function() {
        clearTimeout(resizeTimeout);
        resizeTimeout = setTimeout(() => {
            adjustTableLayout();
            adjustCardLayout();
        }, 250);
    });
    
    // Initial adjustment
    adjustTableLayout();
    adjustCardLayout();
}

/**
 * Adjust table layout for mobile
 */
function adjustTableLayout() {
    const tables = document.querySelectorAll('.admin-table');
    const screenWidth = window.innerWidth;
    
    tables.forEach(table => {
        if (screenWidth < 768) {
            // Add horizontal scroll for mobile
            const container = table.closest('.admin-table-container');
            if (container) {
                container.style.overflowX = 'auto';
            }
        }
    });
}

/**
 * Adjust card layout for different screen sizes
 */
function adjustCardLayout() {
    const grid = document.querySelector('.dashboard-grid');
    if (!grid) return;
    
    const screenWidth = window.innerWidth;
    
    if (screenWidth < 480) {
        grid.style.gridTemplateColumns = '1fr';
        grid.style.gap = '1rem';
    } else if (screenWidth < 768) {
        grid.style.gridTemplateColumns = '1fr';
        grid.style.gap = '1.5rem';
    } else if (screenWidth < 1024) {
        grid.style.gridTemplateColumns = 'repeat(2, 1fr)';
        grid.style.gap = '2rem';
    } else {
        grid.style.gridTemplateColumns = 'repeat(auto-fit, minmax(300px, 1fr))';
        grid.style.gap = '2rem';
    }
}

/**
 * Initialize confirmation dialogs for destructive actions
 */
function initializeConfirmations() {
    const dangerButtons = document.querySelectorAll('.admin-btn-danger, .btn-danger');
    
    dangerButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            const action = this.textContent.trim().toLowerCase();
            let message = 'Are you sure you want to perform this action?';
            
            if (action.includes('delete')) {
                message = 'Are you sure you want to delete this item? This action cannot be undone.';
            } else if (action.includes('remove')) {
                message = 'Are you sure you want to remove this item?';
            } else if (action.includes('deactivate')) {
                message = 'Are you sure you want to deactivate this item?';
            }
            
            if (!confirm(message)) {
                e.preventDefault();
                return false;
            }
        });
    });
}

/**
 * Initialize tooltips for better UX
 */
function initializeTooltips() {
    const elementsWithTooltips = document.querySelectorAll('[data-tooltip]');
    
    elementsWithTooltips.forEach(element => {
        element.addEventListener('mouseenter', function() {
            showAdminTooltip(this, this.getAttribute('data-tooltip'));
        });
        
        element.addEventListener('mouseleave', function() {
            hideAdminTooltip();
        });
    });
}

/**
 * Show admin tooltip
 */
function showAdminTooltip(element, text) {
    const tooltip = document.createElement('div');
    tooltip.className = 'admin-tooltip';
    tooltip.textContent = text;
    tooltip.style.cssText = `
        position: absolute;
        background: #1f2937;
        color: white;
        padding: 0.5rem 0.75rem;
        border-radius: 0.375rem;
        font-size: 0.875rem;
        z-index: 1000;
        pointer-events: none;
        opacity: 0;
        transition: opacity 0.3s ease;
        box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
    `;
    
    document.body.appendChild(tooltip);
    
    const rect = element.getBoundingClientRect();
    tooltip.style.left = rect.left + (rect.width / 2) - (tooltip.offsetWidth / 2) + 'px';
    tooltip.style.top = rect.top - tooltip.offsetHeight - 8 + 'px';
    
    setTimeout(() => {
        tooltip.style.opacity = '1';
    }, 10);
}

/**
 * Hide admin tooltip
 */
function hideAdminTooltip() {
    const tooltip = document.querySelector('.admin-tooltip');
    if (tooltip) {
        tooltip.style.opacity = '0';
        setTimeout(() => {
            tooltip.remove();
        }, 300);
    }
}

/**
 * Initialize keyboard navigation
 */
function initializeKeyboardNavigation() {
    document.addEventListener('keydown', function(e) {
        // Escape key to close modals or cancel actions
        if (e.key === 'Escape') {
            hideAdminTooltip();
            // Add other escape actions here
        }
        
        // Ctrl+R to refresh data (prevent default browser refresh)
        if (e.ctrlKey && e.key === 'r') {
            e.preventDefault();
            location.reload();
        }
    });
}

/**
 * Initialize status indicators with dynamic updates
 */
function initializeStatusIndicators() {
    const statusElements = document.querySelectorAll('.status-active, .status-inactive');
    
    statusElements.forEach(element => {
        // Add pulsing animation for active statuses
        if (element.classList.contains('status-active')) {
            const indicator = element.querySelector('.status-indicator');
            if (indicator) {
                indicator.style.animation = 'pulse 2s infinite';
            }
        }
    });
}

/**
 * Initialize loading states for forms and actions
 */
function initializeLoadingStates() {
    const forms = document.querySelectorAll('form');
    
    forms.forEach(form => {
        form.addEventListener('submit', function() {
            const submitButton = this.querySelector('input[type="submit"], button[type="submit"]');
            if (submitButton) {
                submitButton.disabled = true;
                submitButton.style.opacity = '0.6';
                
                // Add loading text
                const originalText = submitButton.value || submitButton.textContent;
                if (submitButton.tagName === 'INPUT') {
                    submitButton.value = 'Processing...';
                } else {
                    submitButton.textContent = 'Processing...';
                }
                
                // Restore after timeout (fallback)
                setTimeout(() => {
                    submitButton.disabled = false;
                    submitButton.style.opacity = '1';
                    if (submitButton.tagName === 'INPUT') {
                        submitButton.value = originalText;
                    } else {
                        submitButton.textContent = originalText;
                    }
                }, 5000);
            }
        });
    });
}

/**
 * Utility function to format numbers with commas
 */
function formatNumber(num) {
    return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
}

/**
 * Utility function to get relative time
 */
function getRelativeTime(dateString) {
    const date = new Date(dateString);
    const now = new Date();
    const diffTime = Math.abs(now - date);
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    
    if (diffDays === 0) return 'Today';
    if (diffDays === 1) return 'Yesterday';
    if (diffDays < 7) return `${diffDays} days ago`;
    if (diffDays < 30) return `${Math.floor(diffDays / 7)} weeks ago`;
    return `${Math.floor(diffDays / 30)} months ago`;
}
