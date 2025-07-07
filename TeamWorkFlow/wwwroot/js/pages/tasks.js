/**
 * Tasks Page JavaScript
 * Handles interactions and animations for the Tasks listing page
 */

document.addEventListener('DOMContentLoaded', function() {
    console.log('Tasks page initialized');
    
    // Initialize all functionality
    initializeSearchForm();
    initializeAnimations();
    initializeCardInteractions();
    initializePagination();
    initializeResponsiveFeatures();
    initializeTooltips();
    initializeKeyboardNavigation();
    initializeStatusIndicators();
    initializePriorityIndicators();
    initializeDeadlineWarnings();
});

/**
 * Initialize search form functionality
 */
function initializeSearchForm() {
    const searchForm = document.getElementById('search-form');
    const searchInput = document.getElementById('search-input');
    const sortingSelect = document.getElementById('sorting-select');
    const clearButton = document.getElementById('clear-search');
    
    if (!searchForm) return;
    
    // Auto-submit on sorting change
    if (sortingSelect) {
        sortingSelect.addEventListener('change', function() {
            console.log('Sorting changed to:', this.value);
            searchForm.submit();
        });
    }
    
    // Debounced search input
    if (searchInput) {
        let searchTimeout;
        searchInput.addEventListener('input', function() {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                if (this.value.length >= 3 || this.value.length === 0) {
                    console.log('Search triggered:', this.value);
                    searchForm.submit();
                }
            }, 500);
        });
    }
    
    // Clear search functionality
    if (clearButton) {
        clearButton.addEventListener('click', function(e) {
            e.preventDefault();
            if (searchInput) searchInput.value = '';
            if (sortingSelect) sortingSelect.selectedIndex = 0;
            searchForm.submit();
        });
    }
    
    // Form submission with loading state
    searchForm.addEventListener('submit', function() {
        const submitButton = this.querySelector('input[type="submit"]');
        if (submitButton) {
            submitButton.disabled = true;
            submitButton.value = 'Searching...';
        }
        
        // Add loading class to grid
        const tasksGrid = document.querySelector('.tasks-grid');
        if (tasksGrid) {
            tasksGrid.classList.add('loading');
        }
    });
}

/**
 * Initialize card animations and interactions
 */
function initializeAnimations() {
    // Animate cards on page load
    const cards = document.querySelectorAll('.task-card');
    cards.forEach((card, index) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        
        setTimeout(() => {
            card.style.transition = 'all 0.6s ease';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, index * 100);
    });
    
    // Add stagger animation class
    setTimeout(() => {
        cards.forEach(card => {
            card.classList.add('fade-in');
        });
    }, 100);
}

/**
 * Initialize card hover interactions
 */
function initializeCardInteractions() {
    const cards = document.querySelectorAll('.task-card');
    
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
        
        // Click animation
        card.addEventListener('mousedown', function() {
            this.style.transform = 'translateY(-6px) scale(0.98)';
        });
        
        card.addEventListener('mouseup', function() {
            this.style.transform = 'translateY(-8px) scale(1.02)';
        });
    });
}

/**
 * Initialize pagination functionality
 */
function initializePagination() {
    const paginationLinks = document.querySelectorAll('.page-link');
    
    paginationLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            if (this.parentElement.classList.contains('disabled')) {
                e.preventDefault();
                return;
            }
            
            // Add loading state
            const tasksGrid = document.querySelector('.tasks-grid');
            if (tasksGrid) {
                tasksGrid.classList.add('loading');
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
            adjustGridLayout();
            adjustCardSizes();
        }, 250);
    });
    
    // Initial adjustment
    adjustGridLayout();
    adjustCardSizes();
}

/**
 * Adjust grid layout based on screen size
 */
function adjustGridLayout() {
    const grid = document.querySelector('.tasks-grid');
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
        grid.style.gridTemplateColumns = 'repeat(auto-fill, minmax(400px, 1fr))';
        grid.style.gap = '2rem';
    }
}

/**
 * Adjust card sizes for better mobile experience
 */
function adjustCardSizes() {
    const cards = document.querySelectorAll('.task-card');
    const screenWidth = window.innerWidth;
    
    cards.forEach(card => {
        if (screenWidth < 480) {
            card.style.minHeight = 'auto';
        } else {
            card.style.minHeight = '450px';
        }
    });
}

/**
 * Initialize tooltips for better UX
 */
function initializeTooltips() {
    const elementsWithTooltips = document.querySelectorAll('[data-tooltip]');
    
    elementsWithTooltips.forEach(element => {
        element.addEventListener('mouseenter', function() {
            showTooltip(this, this.getAttribute('data-tooltip'));
        });
        
        element.addEventListener('mouseleave', function() {
            hideTooltip();
        });
    });
}

/**
 * Show tooltip
 */
function showTooltip(element, text) {
    const tooltip = document.createElement('div');
    tooltip.className = 'tooltip';
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
 * Hide tooltip
 */
function hideTooltip() {
    const tooltip = document.querySelector('.tooltip');
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
        // Escape key to clear search
        if (e.key === 'Escape') {
            const searchInput = document.getElementById('search-input');
            if (searchInput && searchInput === document.activeElement) {
                searchInput.value = '';
                searchInput.blur();
            }
        }
        
        // Enter key to submit search
        if (e.key === 'Enter' && e.target.id === 'search-input') {
            const searchForm = document.getElementById('search-form');
            if (searchForm) {
                searchForm.submit();
            }
        }
    });
}

/**
 * Initialize status indicators with dynamic colors
 */
function initializeStatusIndicators() {
    const statusElements = document.querySelectorAll('.task-status');
    
    statusElements.forEach(element => {
        const status = element.textContent.toLowerCase().trim();
        
        // Add pulsing animation for in-progress tasks
        if (status.includes('progress') || status.includes('active')) {
            element.style.animation = 'pulse 2s infinite';
        }
    });
}

/**
 * Initialize priority indicators with visual enhancements
 */
function initializePriorityIndicators() {
    const priorityElements = document.querySelectorAll('.task-priority');
    
    priorityElements.forEach(element => {
        const priority = element.textContent.toLowerCase().trim();
        
        // Add special effects for high priority tasks
        if (priority.includes('high') || priority.includes('critical')) {
            element.style.animation = 'pulse 3s infinite';
        }
    });
}

/**
 * Initialize deadline warnings
 */
function initializeDeadlineWarnings() {
    const deadlineElements = document.querySelectorAll('.task-deadline');
    
    deadlineElements.forEach(element => {
        const deadlineText = element.textContent.trim();
        if (deadlineText && deadlineText !== '-') {
            const deadlineDate = new Date(deadlineText);
            const today = new Date();
            const timeDiff = deadlineDate.getTime() - today.getTime();
            const daysDiff = Math.ceil(timeDiff / (1000 * 3600 * 24));
            
            // Add warning classes based on deadline proximity
            if (daysDiff < 0) {
                element.classList.add('overdue');
                element.title = 'Task is overdue!';
            } else if (daysDiff <= 3) {
                element.style.color = '#dc2626';
                element.style.fontWeight = '700';
                element.title = `Due in ${daysDiff} day(s)`;
            } else if (daysDiff <= 7) {
                element.style.color = '#d97706';
                element.style.fontWeight = '600';
                element.title = `Due in ${daysDiff} day(s)`;
            }
        }
    });
}

/**
 * Utility function to format dates
 */
function formatDate(dateString) {
    if (!dateString || dateString === '-') return '-';
    
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
    });
}

/**
 * Utility function to get relative time
 */
function getRelativeTime(dateString) {
    if (!dateString || dateString === '-') return '-';
    
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
