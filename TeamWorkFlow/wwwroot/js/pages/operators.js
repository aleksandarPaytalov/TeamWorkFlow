/**
 * Operators Page JavaScript
 * Handles interactions and animations for the Operators listing page
 */

document.addEventListener('DOMContentLoaded', function() {
    console.log('Operators page initialized');
    
    // Initialize all functionality
    initializeSearchForm();
    initializeAnimations();
    initializeCardInteractions();
    initializePagination();
    initializeResponsiveFeatures();
    initializeTooltips();
    initializeKeyboardNavigation();
    initializeStatusIndicators();
    initializeCapacityIndicators();
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
        const operatorsGrid = document.querySelector('.operators-grid');
        if (operatorsGrid) {
            operatorsGrid.classList.add('loading');
        }
    });
}

/**
 * Initialize card animations and interactions
 */
function initializeAnimations() {
    // Animate cards on page load
    const cards = document.querySelectorAll('.operator-card');
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
    const cards = document.querySelectorAll('.operator-card');
    
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
            const operatorsGrid = document.querySelector('.operators-grid');
            if (operatorsGrid) {
                operatorsGrid.classList.add('loading');
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
    const grid = document.querySelector('.operators-grid');
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
        grid.style.gridTemplateColumns = 'repeat(auto-fill, minmax(380px, 1fr))';
        grid.style.gap = '2rem';
    }
}

/**
 * Adjust card sizes for better mobile experience
 */
function adjustCardSizes() {
    const cards = document.querySelectorAll('.operator-card');
    const screenWidth = window.innerWidth;
    
    cards.forEach(card => {
        if (screenWidth < 480) {
            card.style.minHeight = 'auto';
        } else {
            card.style.minHeight = '400px';
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
    const statusElements = document.querySelectorAll('.operator-status');
    
    statusElements.forEach(element => {
        const status = element.textContent.toLowerCase().trim();
        
        // Add pulsing animation for active statuses
        if (status.includes('available') || status.includes('ready')) {
            element.style.animation = 'pulse 2s infinite';
        }
    });
}

/**
 * Initialize capacity indicators with visual enhancements
 */
function initializeCapacityIndicators() {
    const capacityElements = document.querySelectorAll('.capacity-indicator');
    
    capacityElements.forEach(element => {
        const capacityText = element.textContent;
        const capacityValue = parseInt(capacityText);
        
        if (!isNaN(capacityValue)) {
            // Add capacity level classes
            if (capacityValue >= 80) {
                element.classList.add('capacity-high');
            } else if (capacityValue >= 50) {
                element.classList.add('capacity-medium');
            } else {
                element.classList.add('capacity-low');
            }
            
            // Add capacity bar
            const bar = document.createElement('div');
            bar.style.cssText = `
                width: ${capacityValue}%;
                height: 3px;
                background: currentColor;
                border-radius: 2px;
                margin-top: 4px;
                transition: width 0.3s ease;
            `;
            element.appendChild(bar);
        }
    });
}

/**
 * Utility function to get operator initials for avatar
 */
function getOperatorInitials(fullName) {
    return fullName
        .split(' ')
        .map(name => name.charAt(0))
        .join('')
        .toUpperCase()
        .substring(0, 2);
}

/**
 * Utility function to format phone numbers
 */
function formatPhoneNumber(phone) {
    const cleaned = phone.replace(/\D/g, '');
    const match = cleaned.match(/^(\d{3})(\d{3})(\d{4})$/);
    if (match) {
        return '(' + match[1] + ') ' + match[2] + '-' + match[3];
    }
    return phone;
}
