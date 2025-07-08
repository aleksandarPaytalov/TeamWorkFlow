/**
 * Delete Confirmation Pages JavaScript
 * Enhanced user interactions and animations for delete confirmation pages
 */

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    initializeDeletePage();
});

/**
 * Main initialization function
 */
function initializeDeletePage() {
    initializeAnimations();
    initializeConfirmationHandlers();
    initializeKeyboardNavigation();
    initializeAccessibility();
    initializeResponsiveFeatures();
}

/**
 * Initialize page animations
 */
function initializeAnimations() {
    // Add staggered animation to detail rows
    const detailRows = document.querySelectorAll('.delete-detail-row');
    detailRows.forEach((row, index) => {
        row.style.animationDelay = `${index * 0.1}s`;
        row.classList.add('fade-in-delete');
    });

    // Add hover effects to buttons
    const buttons = document.querySelectorAll('.delete-btn');
    buttons.forEach(button => {
        button.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-2px) scale(1.02)';
        });

        button.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0) scale(1)';
        });
    });

    // Add pulse animation to warning icon
    const warningIcon = document.querySelector('.delete-warning-icon');
    if (warningIcon) {
        setInterval(() => {
            warningIcon.style.transform = 'scale(1.1)';
            setTimeout(() => {
                warningIcon.style.transform = 'scale(1)';
            }, 200);
        }, 3000);
    }
}

/**
 * Initialize confirmation handlers
 */
function initializeConfirmationHandlers() {
    const confirmButtons = document.querySelectorAll('.delete-btn-confirm');
    
    confirmButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            showConfirmationDialog(this);
        });
    });
}

/**
 * Show enhanced confirmation dialog
 */
function showConfirmationDialog(button) {
    const itemName = document.querySelector('.delete-item-title')?.textContent || 'this item';
    const itemType = getItemType();
    
    const dialog = createConfirmationDialog(itemName, itemType);
    document.body.appendChild(dialog);
    
    // Animate dialog in
    setTimeout(() => {
        dialog.classList.add('show');
    }, 10);
    
    // Focus on cancel button for accessibility
    const cancelBtn = dialog.querySelector('.dialog-cancel');
    if (cancelBtn) {
        cancelBtn.focus();
    }
    
    // Handle dialog actions
    const confirmBtn = dialog.querySelector('.dialog-confirm');
    const cancelBtnDialog = dialog.querySelector('.dialog-cancel');
    
    confirmBtn.addEventListener('click', () => {
        closeDialog(dialog);
        submitDeleteForm(button);
    });
    
    cancelBtnDialog.addEventListener('click', () => {
        closeDialog(dialog);
    });
    
    // Close on escape key
    document.addEventListener('keydown', function escapeHandler(e) {
        if (e.key === 'Escape') {
            closeDialog(dialog);
            document.removeEventListener('keydown', escapeHandler);
        }
    });
    
    // Close on backdrop click
    dialog.addEventListener('click', (e) => {
        if (e.target === dialog) {
            closeDialog(dialog);
        }
    });
}

/**
 * Create confirmation dialog element
 */
function createConfirmationDialog(itemName, itemType) {
    const dialog = document.createElement('div');
    dialog.className = 'delete-confirmation-dialog';
    dialog.innerHTML = `
        <div class="dialog-backdrop"></div>
        <div class="dialog-content">
            <div class="dialog-header">
                <div class="dialog-icon">
                    <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z"></path>
                    </svg>
                </div>
                <h3 class="dialog-title">Confirm Deletion</h3>
            </div>
            <div class="dialog-body">
                <p class="dialog-message">
                    Are you absolutely sure you want to delete <strong>"${itemName}"</strong>?
                </p>
                <p class="dialog-submessage">
                    This ${itemType} will be permanently removed from the system. This action cannot be undone.
                </p>
            </div>
            <div class="dialog-actions">
                <button type="button" class="dialog-btn dialog-cancel">
                    <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                    </svg>
                    Cancel
                </button>
                <button type="button" class="dialog-btn dialog-confirm">
                    <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path>
                    </svg>
                    Delete Permanently
                </button>
            </div>
        </div>
    `;
    
    return dialog;
}

/**
 * Close confirmation dialog
 */
function closeDialog(dialog) {
    dialog.classList.remove('show');
    setTimeout(() => {
        if (dialog.parentNode) {
            dialog.parentNode.removeChild(dialog);
        }
    }, 300);
}

/**
 * Submit delete form with loading state
 */
function submitDeleteForm(button) {
    // Add loading state
    button.classList.add('delete-btn-loading');
    button.disabled = true;
    
    // Find and submit the form
    const form = button.closest('form');
    if (form) {
        form.submit();
    }
}

/**
 * Get item type from page title
 */
function getItemType() {
    const title = document.querySelector('.delete-title')?.textContent || '';
    if (title.includes('Machine')) return 'machine';
    if (title.includes('Task')) return 'task';
    if (title.includes('Project')) return 'project';
    if (title.includes('Part')) return 'part';
    if (title.includes('Operator')) return 'operator';
    return 'item';
}

/**
 * Initialize keyboard navigation
 */
function initializeKeyboardNavigation() {
    const buttons = document.querySelectorAll('.delete-btn');
    
    buttons.forEach((button, index) => {
        button.addEventListener('keydown', function(e) {
            if (e.key === 'ArrowLeft' || e.key === 'ArrowRight') {
                e.preventDefault();
                const nextIndex = e.key === 'ArrowRight' ? 
                    (index + 1) % buttons.length : 
                    (index - 1 + buttons.length) % buttons.length;
                buttons[nextIndex].focus();
            }
        });
    });
}

/**
 * Initialize accessibility features
 */
function initializeAccessibility() {
    // Add ARIA labels
    const confirmButton = document.querySelector('.delete-btn-confirm');
    if (confirmButton) {
        const itemName = document.querySelector('.delete-item-title')?.textContent || 'item';
        confirmButton.setAttribute('aria-label', `Confirm deletion of ${itemName}`);
    }
    
    const cancelButton = document.querySelector('.delete-btn-cancel');
    if (cancelButton) {
        cancelButton.setAttribute('aria-label', 'Cancel deletion and return to list');
    }
    
    // Add live region for screen readers
    const liveRegion = document.createElement('div');
    liveRegion.setAttribute('aria-live', 'polite');
    liveRegion.setAttribute('aria-atomic', 'true');
    liveRegion.className = 'sr-only';
    liveRegion.id = 'delete-live-region';
    document.body.appendChild(liveRegion);
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
    const container = document.querySelector('.delete-container');
    const actions = document.querySelector('.delete-actions');
    
    if (window.innerWidth <= 768) {
        container?.classList.add('mobile-layout');
        actions?.classList.add('mobile-actions');
    } else {
        container?.classList.remove('mobile-layout');
        actions?.classList.remove('mobile-actions');
    }
}

/**
 * Global confirmation function for onclick handlers
 */
function confirmDelete(button) {
    // This function is called from onclick handlers in the HTML
    // It prevents the default form submission and shows our custom dialog
    return false;
}

/**
 * Utility function to announce to screen readers
 */
function announceToScreenReader(message) {
    const liveRegion = document.getElementById('delete-live-region');
    if (liveRegion) {
        liveRegion.textContent = message;
        setTimeout(() => {
            liveRegion.textContent = '';
        }, 1000);
    }
}
