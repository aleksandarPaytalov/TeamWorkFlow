/**
 * Machines (CMMs) Page JavaScript
 * Handles interactions and animations for the Machines listing page
 */

document.addEventListener('DOMContentLoaded', function() {
    console.log('Machines page initialized');
    
    // Initialize all functionality
    initializeSearchForm();
    initializeAnimations();
    initializeCardInteractions();
    initializePagination();
    initializeResponsiveFeatures();
    initializeImageLazyLoading();
    initializeTooltips();
    initializeKeyboardNavigation();
    initializeCalibrationAlerts();
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
        
        // Focus enhancement
        searchInput.addEventListener('focus', function() {
            this.parentElement.classList.add('focused');
        });
        
        searchInput.addEventListener('blur', function() {
            this.parentElement.classList.remove('focused');
        });
    }
    
    // Clear search functionality
    if (clearButton) {
        clearButton.addEventListener('click', function(e) {
            e.preventDefault();
            
            // Clear all form inputs
            if (searchInput) searchInput.value = '';
            if (sortingSelect) sortingSelect.selectedIndex = 0;
            
            // Submit form to refresh results
            searchForm.submit();
        });
    }
    
    // Form submission loading state
    searchForm.addEventListener('submit', function() {
        const submitButton = this.querySelector('input[type="submit"]');
        if (submitButton) {
            submitButton.value = 'Searching...';
            submitButton.disabled = true;
        }
    });
}

/**
 * Initialize page animations
 */
function initializeAnimations() {
    // Animate cards on scroll
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };
    
    const observer = new IntersectionObserver(function(entries) {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('fade-in');
                observer.unobserve(entry.target);
            }
        });
    }, observerOptions);
    
    // Observe all machine cards
    const machineCards = document.querySelectorAll('.machine-card');
    machineCards.forEach(card => {
        observer.observe(card);
    });
    
    // Animate search section
    const searchSection = document.querySelector('.search-section');
    if (searchSection) {
        setTimeout(() => {
            searchSection.classList.add('fade-in');
        }, 200);
    }
}

/**
 * Initialize card interactions
 */
function initializeCardInteractions() {
    const machineCards = document.querySelectorAll('.machine-card');
    
    machineCards.forEach(card => {
        // Add ripple effect on click
        card.addEventListener('click', function(e) {
            // Only add ripple if not clicking on action buttons
            if (!e.target.closest('.action-btn')) {
                addRippleEffect(this, e);
            }
        });
        
        // Enhanced hover effects
        card.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-8px) scale(1.02)';
        });
        
        card.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0) scale(1)';
        });
    });
    
    // Action button interactions
    const actionButtons = document.querySelectorAll('.action-btn');
    actionButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            addRippleEffect(this, e);
        });
        
        // Prevent card hover when hovering buttons
        button.addEventListener('mouseenter', function() {
            this.closest('.machine-card').style.transform = 'translateY(-8px)';
        });
    });
}

/**
 * Add ripple effect to elements
 */
function addRippleEffect(element, event) {
    const ripple = document.createElement('span');
    const rect = element.getBoundingClientRect();
    const size = Math.max(rect.width, rect.height);
    const x = event.clientX - rect.left - size / 2;
    const y = event.clientY - rect.top - size / 2;
    
    ripple.style.cssText = `
        position: absolute;
        width: ${size}px;
        height: ${size}px;
        left: ${x}px;
        top: ${y}px;
        background: rgba(255, 255, 255, 0.3);
        border-radius: 50%;
        transform: scale(0);
        animation: ripple 0.6s ease-out;
        pointer-events: none;
        z-index: 1;
    `;
    
    // Add ripple animation CSS if not exists
    if (!document.getElementById('ripple-styles')) {
        const style = document.createElement('style');
        style.id = 'ripple-styles';
        style.textContent = `
            @keyframes ripple {
                to {
                    transform: scale(2);
                    opacity: 0;
                }
            }
        `;
        document.head.appendChild(style);
    }
    
    element.style.position = 'relative';
    element.style.overflow = 'hidden';
    element.appendChild(ripple);
    
    setTimeout(() => {
        ripple.remove();
    }, 600);
}

/**
 * Initialize pagination functionality
 */
function initializePagination() {
    const paginationLinks = document.querySelectorAll('.page-link');
    
    paginationLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            if (this.closest('.page-item').classList.contains('disabled')) {
                e.preventDefault();
                return;
            }
            
            // Add loading state
            this.style.opacity = '0.6';
            this.style.pointerEvents = 'none';
        });
    });
}

/**
 * Initialize responsive features
 */
function initializeResponsiveFeatures() {
    // Mobile menu toggle for filters
    const searchSection = document.querySelector('.search-section');
    if (window.innerWidth <= 768 && searchSection) {
        // Add mobile-specific enhancements
        const formInputs = searchSection.querySelectorAll('.form-input, .form-select');
        formInputs.forEach(input => {
            input.addEventListener('focus', function() {
                // Scroll into view on mobile
                setTimeout(() => {
                    this.scrollIntoView({ behavior: 'smooth', block: 'center' });
                }, 300);
            });
        });
    }
    
    // Handle window resize
    let resizeTimeout;
    window.addEventListener('resize', function() {
        clearTimeout(resizeTimeout);
        resizeTimeout = setTimeout(() => {
            // Recalculate grid layout if needed
            const machinesGrid = document.querySelector('.machines-grid');
            if (machinesGrid) {
                machinesGrid.style.opacity = '0.8';
                setTimeout(() => {
                    machinesGrid.style.opacity = '1';
                }, 100);
            }
        }, 250);
    });
}

/**
 * Initialize image lazy loading for better performance
 */
function initializeImageLazyLoading() {
    const images = document.querySelectorAll('.machine-card-image');
    
    if ('IntersectionObserver' in window) {
        const imageObserver = new IntersectionObserver((entries, observer) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const img = entry.target;
                    img.style.opacity = '0';
                    img.style.transition = 'opacity 0.3s ease';
                    
                    img.onload = () => {
                        img.style.opacity = '1';
                    };
                    
                    // If image is already cached, show it immediately
                    if (img.complete) {
                        img.style.opacity = '1';
                    }
                    
                    observer.unobserve(img);
                }
            });
        });
        
        images.forEach(img => imageObserver.observe(img));
    }
}

/**
 * Initialize tooltips for better UX
 */
function initializeTooltips() {
    const actionButtons = document.querySelectorAll('.action-btn');
    
    actionButtons.forEach(button => {
        button.addEventListener('mouseenter', function() {
            const tooltip = document.createElement('div');
            tooltip.className = 'action-tooltip';
            tooltip.textContent = this.textContent.trim();
            tooltip.style.cssText = `
                position: absolute;
                bottom: 100%;
                left: 50%;
                transform: translateX(-50%);
                background: #1f2937;
                color: white;
                padding: 0.5rem 0.75rem;
                border-radius: 0.375rem;
                font-size: 0.75rem;
                white-space: nowrap;
                z-index: 1000;
                opacity: 0;
                animation: tooltipFadeIn 0.2s ease-out forwards;
                pointer-events: none;
            `;
            
            // Add tooltip animation CSS if not exists
            if (!document.getElementById('tooltip-styles')) {
                const style = document.createElement('style');
                style.id = 'tooltip-styles';
                style.textContent = `
                    @keyframes tooltipFadeIn {
                        to {
                            opacity: 1;
                            transform: translateX(-50%) translateY(-4px);
                        }
                    }
                `;
                document.head.appendChild(style);
            }
            
            this.style.position = 'relative';
            this.appendChild(tooltip);
        });
        
        button.addEventListener('mouseleave', function() {
            const tooltip = this.querySelector('.action-tooltip');
            if (tooltip) {
                tooltip.remove();
            }
        });
    });
}

/**
 * Initialize keyboard navigation
 */
function initializeKeyboardNavigation() {
    document.addEventListener('keydown', function(e) {
        // Escape key clears search
        if (e.key === 'Escape') {
            const searchInput = document.getElementById('search-input');
            if (searchInput && searchInput === document.activeElement) {
                searchInput.value = '';
                searchInput.blur();
            }
        }
        
        // Enter key on cards triggers first action
        if (e.key === 'Enter') {
            const focusedCard = document.activeElement.closest('.machine-card');
            if (focusedCard) {
                const firstAction = focusedCard.querySelector('.action-btn');
                if (firstAction) {
                    firstAction.click();
                }
            }
        }
    });
}

/**
 * Utility function to show loading state
 */
function showLoadingState() {
    const machinesGrid = document.querySelector('.machines-grid');
    if (machinesGrid) {
        machinesGrid.style.opacity = '0.6';
        machinesGrid.style.pointerEvents = 'none';
    }
}

/**
 * Utility function to hide loading state
 */
function hideLoadingState() {
    const machinesGrid = document.querySelector('.machines-grid');
    if (machinesGrid) {
        machinesGrid.style.opacity = '1';
        machinesGrid.style.pointerEvents = 'auto';
    }
}

/**
 * Initialize calibration alerts
 */
function initializeCalibrationAlerts() {
    const calibrationWarnings = document.querySelectorAll('.calibration-warning');

    calibrationWarnings.forEach(warning => {
        // Add pulsing animation to critical warnings
        warning.style.animation = 'pulse 2s infinite';

        // Add click handler for more info
        warning.addEventListener('click', function() {
            alert('This machine requires calibration before it can be used for measurements. Please contact maintenance.');
        });

        warning.style.cursor = 'pointer';
        warning.title = 'Click for more information';
    });

    // Add pulse animation CSS if not exists
    if (!document.getElementById('pulse-styles')) {
        const style = document.createElement('style');
        style.id = 'pulse-styles';
        style.textContent = `
            @keyframes pulse {
                0%, 100% {
                    opacity: 1;
                }
                50% {
                    opacity: 0.7;
                }
            }
        `;
        document.head.appendChild(style);
    }
}

/**
 * Initialize capacity indicators with enhanced visuals
 */
function initializeCapacityIndicators() {
    const capacityIndicators = document.querySelectorAll('.capacity-indicator');

    capacityIndicators.forEach(indicator => {
        const capacityText = indicator.textContent;
        const capacityValue = parseInt(capacityText.match(/\d+/)?.[0] || '0');

        // Color code based on capacity
        if (capacityValue >= 100) {
            indicator.style.background = 'linear-gradient(135deg, #dcfce7 0%, #bbf7d0 100%)';
            indicator.style.color = '#166534';
            indicator.style.borderColor = '#bbf7d0';
        } else if (capacityValue >= 50) {
            indicator.style.background = 'linear-gradient(135deg, #fef3c7 0%, #fde68a 100%)';
            indicator.style.color = '#92400e';
            indicator.style.borderColor = '#fde68a';
        } else {
            indicator.style.background = 'linear-gradient(135deg, #fee2e2 0%, #fecaca 100%)';
            indicator.style.color = '#991b1b';
            indicator.style.borderColor = '#fecaca';
        }

        // Add tooltip with capacity details
        indicator.title = `Daily capacity: ${capacityValue} parts per day`;
    });
}

/**
 * Handle form errors gracefully
 */
window.addEventListener('error', function(e) {
    console.error('Machines page error:', e.error);
    hideLoadingState();
});
