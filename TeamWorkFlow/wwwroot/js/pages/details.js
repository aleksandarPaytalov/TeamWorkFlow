/**
 * Details Pages JavaScript
 * Handles interactions and animations for all details pages
 */

document.addEventListener('DOMContentLoaded', function() {
    console.log('Details page initialized');
    
    // Initialize all functionality
    initializeAnimations();
    initializeImageInteractions();
    initializeActionButtons();
    initializeResponsiveFeatures();
    initializeTooltips();
    initializeKeyboardNavigation();
    initializePrintFeatures();
    initializeStatusIndicators();
    initializeCopyFeatures();
    initializeTouchFeatures();
    initializeAccessibilityFeatures();
});

/**
 * Initialize animations for details elements
 */
function initializeAnimations() {
    // Animate main content on page load
    const detailsCard = document.querySelector('.details-card');
    const imageContainer = document.querySelector('.details-image-container');
    
    if (detailsCard) {
        detailsCard.style.opacity = '0';
        detailsCard.style.transform = 'translateY(20px)';
        
        setTimeout(() => {
            detailsCard.style.transition = 'all 0.6s ease';
            detailsCard.style.opacity = '1';
            detailsCard.style.transform = 'translateY(0)';
        }, 100);
    }
    
    if (imageContainer) {
        imageContainer.style.opacity = '0';
        imageContainer.style.transform = 'translateX(20px)';
        
        setTimeout(() => {
            imageContainer.style.transition = 'all 0.6s ease';
            imageContainer.style.opacity = '1';
            imageContainer.style.transform = 'translateX(0)';
        }, 200);
    }

    // Animate detail sections
    const sections = document.querySelectorAll('.details-section');
    sections.forEach((section, index) => {
        section.style.opacity = '0';
        section.style.transform = 'translateY(20px)';
        
        setTimeout(() => {
            section.style.transition = 'all 0.4s ease';
            section.style.opacity = '1';
            section.style.transform = 'translateY(0)';
        }, 300 + (index * 100));
    });

    // Add fade-in class
    setTimeout(() => {
        document.querySelectorAll('.details-container').forEach(element => {
            element.classList.add('fade-in-details');
        });
    }, 100);
}

/**
 * Initialize image interactions
 */
function initializeImageInteractions() {
    const images = document.querySelectorAll('.details-image');
    
    images.forEach(image => {
        // Add click to zoom functionality
        image.addEventListener('click', function() {
            openImageModal(this.src, this.alt);
        });
        
        // Add loading state
        image.addEventListener('load', function() {
            this.style.opacity = '1';
        });
        
        image.addEventListener('error', function() {
            this.style.display = 'none';
            const placeholder = this.parentElement.querySelector('.details-image-placeholder');
            if (placeholder) {
                placeholder.style.display = 'flex';
            }
        });
    });
}

/**
 * Open image in modal for better viewing
 */
function openImageModal(src, alt) {
    const modal = document.createElement('div');
    modal.className = 'image-modal';
    modal.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: rgba(0, 0, 0, 0.9);
        display: flex;
        align-items: center;
        justify-content: center;
        z-index: 1000;
        cursor: pointer;
    `;
    
    const img = document.createElement('img');
    img.src = src;
    img.alt = alt;
    img.style.cssText = `
        max-width: 90%;
        max-height: 90%;
        object-fit: contain;
        border-radius: 0.5rem;
    `;
    
    modal.appendChild(img);
    document.body.appendChild(modal);
    
    // Close on click
    modal.addEventListener('click', function() {
        document.body.removeChild(modal);
    });
    
    // Close on escape key
    document.addEventListener('keydown', function(e) {
        if (e.key === 'Escape' && document.body.contains(modal)) {
            document.body.removeChild(modal);
        }
    });
}

/**
 * Initialize action button interactions
 */
function initializeActionButtons() {
    const actionButtons = document.querySelectorAll('.details-btn');
    
    actionButtons.forEach(button => {
        // Enhanced hover effects
        button.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-2px) scale(1.02)';
        });
        
        button.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0) scale(1)';
        });
        
        // Click animation
        button.addEventListener('mousedown', function() {
            this.style.transform = 'translateY(-1px) scale(0.98)';
        });
        
        button.addEventListener('mouseup', function() {
            this.style.transform = 'translateY(-2px) scale(1.02)';
        });
    });

    // Confirmation for delete buttons
    const deleteButtons = document.querySelectorAll('.details-btn-delete');
    deleteButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            if (!confirm('Are you sure you want to delete this item? This action cannot be undone.')) {
                e.preventDefault();
                return false;
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
            adjustLayoutForScreenSize();
        }, 250);
    });
    
    // Initial adjustment
    adjustLayoutForScreenSize();
}

/**
 * Adjust layout based on screen size
 */
function adjustLayoutForScreenSize() {
    const content = document.querySelector('.details-content');
    const screenWidth = window.innerWidth;
    const screenHeight = window.innerHeight;

    if (content && content.classList.contains('has-image')) {
        if (screenWidth < 1024) {
            content.style.gridTemplateColumns = '1fr';
        } else {
            content.style.gridTemplateColumns = '1fr 400px';
        }
    }

    // Adjust for very small screens
    if (screenWidth <= 360) {
        document.body.classList.add('extra-small-screen');
    } else {
        document.body.classList.remove('extra-small-screen');
    }

    // Adjust for landscape mobile
    if (screenWidth > screenHeight && screenHeight <= 500) {
        document.body.classList.add('mobile-landscape');
    } else {
        document.body.classList.remove('mobile-landscape');
    }

    // Optimize image containers for mobile
    const imageContainers = document.querySelectorAll('.details-image-container');
    imageContainers.forEach(container => {
        if (screenWidth <= 768) {
            container.style.maxWidth = '100%';
            container.style.margin = '0';
        } else if (screenWidth <= 1024) {
            container.style.maxWidth = '400px';
            container.style.margin = '0 auto';
        }
    });

    // Adjust grid layout for different screen sizes
    const grids = document.querySelectorAll('.details-grid');
    grids.forEach(grid => {
        if (screenWidth <= 768) {
            grid.style.gridTemplateColumns = '1fr';
        } else if (screenWidth <= 1024) {
            grid.style.gridTemplateColumns = '1fr';
        } else {
            grid.style.gridTemplateColumns = 'repeat(auto-fit, minmax(300px, 1fr))';
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
            showDetailsTooltip(this, this.getAttribute('data-tooltip'));
        });
        
        element.addEventListener('mouseleave', function() {
            hideDetailsTooltip();
        });
    });
}

/**
 * Show details tooltip
 */
function showDetailsTooltip(element, text) {
    const tooltip = document.createElement('div');
    tooltip.className = 'details-tooltip';
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
 * Hide details tooltip
 */
function hideDetailsTooltip() {
    const tooltip = document.querySelector('.details-tooltip');
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
        // Escape key to close modals
        if (e.key === 'Escape') {
            const modal = document.querySelector('.image-modal');
            if (modal) {
                document.body.removeChild(modal);
            }
            hideDetailsTooltip();
        }
        
        // Ctrl+P for print
        if (e.ctrlKey && e.key === 'p') {
            e.preventDefault();
            window.print();
        }
    });
}

/**
 * Initialize print features
 */
function initializePrintFeatures() {
    // Add print button if needed
    const actionsContainer = document.querySelector('.details-actions');
    if (actionsContainer) {
        const printButton = document.createElement('button');
        printButton.className = 'details-btn details-btn-back';
        printButton.innerHTML = `
            <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 17h2a2 2 0 002-2v-4a2 2 0 00-2-2H5a2 2 0 00-2 2v4a2 2 0 002 2h2m2 4h6a2 2 0 002-2v-4a2 2 0 00-2-2H9a2 2 0 00-2 2v4a2 2 0 002 2zm8-12V5a2 2 0 00-2-2H9a2 2 0 00-2 2v4h10z"></path>
            </svg>
            Print
        `;
        printButton.addEventListener('click', function() {
            window.print();
        });
        actionsContainer.appendChild(printButton);
    }
}

/**
 * Initialize status indicators with animations
 */
function initializeStatusIndicators() {
    const statusBadges = document.querySelectorAll('.status-badge');
    
    statusBadges.forEach(badge => {
        const indicator = badge.querySelector('.status-indicator');
        if (indicator && badge.classList.contains('status-active')) {
            indicator.style.animation = 'pulse 2s infinite';
        }
    });
}

/**
 * Initialize copy to clipboard features
 */
function initializeCopyFeatures() {
    const copyableElements = document.querySelectorAll('[data-copyable]');
    
    copyableElements.forEach(element => {
        element.style.cursor = 'pointer';
        element.title = 'Click to copy';
        
        element.addEventListener('click', function() {
            const textToCopy = this.textContent.trim();
            navigator.clipboard.writeText(textToCopy).then(() => {
                showCopyFeedback(this);
            }).catch(() => {
                // Fallback for older browsers
                const textArea = document.createElement('textarea');
                textArea.value = textToCopy;
                document.body.appendChild(textArea);
                textArea.select();
                document.execCommand('copy');
                document.body.removeChild(textArea);
                showCopyFeedback(this);
            });
        });
    });
}

/**
 * Show copy feedback
 */
function showCopyFeedback(element) {
    const originalText = element.textContent;
    element.textContent = 'Copied!';
    element.style.color = '#059669';
    
    setTimeout(() => {
        element.textContent = originalText;
        element.style.color = '';
    }, 1000);
}

/**
 * Utility function to format dates
 */
function formatDate(dateString) {
    if (!dateString) return '-';
    
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
    });
}

/**
 * Utility function to get relative time
 */
function getRelativeTime(dateString) {
    if (!dateString) return '-';

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

/**
 * Initialize touch-friendly features for mobile devices
 */
function initializeTouchFeatures() {
    // Add touch feedback for buttons
    const buttons = document.querySelectorAll('.details-btn');
    buttons.forEach(button => {
        button.addEventListener('touchstart', function() {
            this.style.transform = 'scale(0.98)';
        });

        button.addEventListener('touchend', function() {
            this.style.transform = '';
        });
    });

    // Improve touch targets for small elements
    const smallElements = document.querySelectorAll('.status-badge, .priority-badge');
    smallElements.forEach(element => {
        if (window.innerWidth <= 768) {
            element.style.minHeight = '44px';
            element.style.display = 'inline-flex';
            element.style.alignItems = 'center';
            element.style.padding = '0.5rem 0.75rem';
        }
    });

    // Add swipe gesture for image modal
    let startX = 0;
    let startY = 0;

    document.addEventListener('touchstart', function(e) {
        startX = e.touches[0].clientX;
        startY = e.touches[0].clientY;
    });

    document.addEventListener('touchend', function(e) {
        if (!startX || !startY) return;

        const endX = e.changedTouches[0].clientX;
        const endY = e.changedTouches[0].clientY;

        const diffX = startX - endX;
        const diffY = startY - endY;

        // Close modal on swipe down
        const modal = document.querySelector('.image-modal');
        if (modal && Math.abs(diffY) > Math.abs(diffX) && diffY < -50) {
            document.body.removeChild(modal);
        }

        startX = 0;
        startY = 0;
    });
}

/**
 * Initialize accessibility features
 */
function initializeAccessibilityFeatures() {
    // Add ARIA labels for better screen reader support
    const statusBadges = document.querySelectorAll('.status-badge');
    statusBadges.forEach(badge => {
        const status = badge.textContent.trim();
        badge.setAttribute('aria-label', `Status: ${status}`);
    });

    const priorityBadges = document.querySelectorAll('.priority-badge');
    priorityBadges.forEach(badge => {
        const priority = badge.textContent.trim();
        badge.setAttribute('aria-label', `Priority: ${priority}`);
    });

    // Add skip links for keyboard navigation
    const mainContent = document.querySelector('.details-card-body');
    if (mainContent) {
        const skipLink = document.createElement('a');
        skipLink.href = '#main-content';
        skipLink.textContent = 'Skip to main content';
        skipLink.className = 'sr-only';
        skipLink.style.cssText = `
            position: absolute;
            left: -10000px;
            top: auto;
            width: 1px;
            height: 1px;
            overflow: hidden;
        `;

        skipLink.addEventListener('focus', function() {
            this.style.cssText = `
                position: absolute;
                left: 6px;
                top: 7px;
                z-index: 999999;
                padding: 8px 16px;
                background: #000;
                color: #fff;
                text-decoration: none;
                border-radius: 3px;
            `;
        });

        skipLink.addEventListener('blur', function() {
            this.style.cssText = `
                position: absolute;
                left: -10000px;
                top: auto;
                width: 1px;
                height: 1px;
                overflow: hidden;
            `;
        });

        document.body.insertBefore(skipLink, document.body.firstChild);
        mainContent.id = 'main-content';
    }

    // Improve focus management
    const focusableElements = document.querySelectorAll('a, button, input, textarea, select, [tabindex]:not([tabindex="-1"])');
    focusableElements.forEach(element => {
        element.addEventListener('focus', function() {
            this.style.outline = '2px solid #8b5cf6';
            this.style.outlineOffset = '2px';
        });

        element.addEventListener('blur', function() {
            this.style.outline = '';
            this.style.outlineOffset = '';
        });
    });

    // Add high contrast mode detection
    if (window.matchMedia && window.matchMedia('(prefers-contrast: high)').matches) {
        document.body.classList.add('high-contrast');
    }
}
