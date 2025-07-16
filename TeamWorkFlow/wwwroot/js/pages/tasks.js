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
    initializeAssignmentButtons();
    initializeEstimatedTimeButtons();
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

/**
 * Initialize machine and operator assignment buttons
 */
function initializeAssignmentButtons() {
    // Machine assignment
    initializeMachineAssignment();

    // Operator assignment
    initializeOperatorAssignment();
}

/**
 * Initialize machine assignment functionality
 */
function initializeMachineAssignment() {
    // Assign machine buttons
    const assignMachineButtons = document.querySelectorAll('.assign-machine-btn');
    assignMachineButtons.forEach(button => {
        button.addEventListener('click', function() {
            const taskId = this.getAttribute('data-task-id');
            showMachineSelectionModal(taskId);
        });
    });

    // Unassign machine buttons
    const unassignMachineButtons = document.querySelectorAll('.unassign-machine-btn');
    unassignMachineButtons.forEach(button => {
        button.addEventListener('click', function() {
            const taskId = this.getAttribute('data-task-id');
            unassignMachine(taskId);
        });
    });
}

/**
 * Show machine selection modal
 */
function showMachineSelectionModal(taskId) {
    // Create modal container
    const modalContainer = document.createElement('div');
    modalContainer.className = 'modal-container';
    modalContainer.id = 'machine-selection-modal';

    // Create modal content
    modalContainer.innerHTML = `
        <div class="modal-content">
            <div class="modal-header">
                <h3>Assign Machine</h3>
                <button class="modal-close-btn">&times;</button>
            </div>
            <div class="modal-body">
                <p>Select a machine to assign to this task:</p>
                <div class="loading-spinner">Loading available machines...</div>
                <div class="machines-list"></div>
            </div>
            <div class="modal-footer">
                <button class="modal-cancel-btn">Cancel</button>
            </div>
        </div>
    `;

    // Add modal to body
    document.body.appendChild(modalContainer);

    // Add event listeners
    modalContainer.querySelector('.modal-close-btn').addEventListener('click', () => {
        document.body.removeChild(modalContainer);
    });

    modalContainer.querySelector('.modal-cancel-btn').addEventListener('click', () => {
        document.body.removeChild(modalContainer);
    });

    // Load available machines
    fetch(`/Task/GetAvailableMachines?taskId=${taskId}`)
        .then(response => response.json())
        .then(data => {
            const loadingSpinner = modalContainer.querySelector('.loading-spinner');
            const machinesList = modalContainer.querySelector('.machines-list');

            loadingSpinner.style.display = 'none';

            if (data.success && data.machines.length > 0) {
                data.machines.forEach(machine => {
                    const machineItem = document.createElement('div');
                    machineItem.className = 'machine-item';
                    machineItem.innerHTML = `
                        <div class="machine-info">
                            <span class="machine-name">${machine.name}</span>
                            <span class="machine-capacity">${machine.capacity} hours/day</span>
                        </div>
                        <button class="assign-btn" data-machine-id="${machine.id}">Assign</button>
                    `;
                    machinesList.appendChild(machineItem);

                    // Add event listener to assign button
                    machineItem.querySelector('.assign-btn').addEventListener('click', () => {
                        assignMachine(taskId, machine.id);
                        document.body.removeChild(modalContainer);
                    });
                });
            } else {
                machinesList.innerHTML = '<p class="no-machines">No available machines found. Machines must be calibrated and not assigned to other tasks.</p>';
            }
        })
        .catch(error => {
            console.error('Error loading machines:', error);
            const machinesList = modalContainer.querySelector('.machines-list');
            const loadingSpinner = modalContainer.querySelector('.loading-spinner');
            loadingSpinner.style.display = 'none';
            machinesList.innerHTML = '<p class="error-message">Error loading machines. Please try again.</p>';
        });
}

/**
 * Get anti-forgery token
 */
function getAntiForgeryToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
}

/**
 * Assign machine to task
 */
function assignMachine(taskId, machineId) {
    // Validate assignment first
    fetch(`/Task/ValidateMachineAssignment?taskId=${taskId}&machineId=${machineId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(validation => {
            if (validation.canAssign) {
                // Proceed with assignment
                const token = getAntiForgeryToken();
                fetch('/Task/AssignMachine', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded',
                        'RequestVerificationToken': token
                    },
                    body: `taskId=${taskId}&machineId=${machineId}&__RequestVerificationToken=${encodeURIComponent(token)}`
                })
                .then(response => {
                    if (!response.ok) {
                        throw new Error(`HTTP error! status: ${response.status}`);
                    }
                    return response.json();
                })
                .then(data => {
                    if (data.success) {
                        showNotification('Machine assigned successfully', 'success');
                        // Reload page to reflect changes
                        window.location.reload();
                    } else {
                        showNotification(data.message, 'error');
                    }
                })
                .catch(error => {
                    console.error('Error assigning machine:', error);
                    showNotification('Error assigning machine. Please try again.', 'error');
                });
            } else {
                showNotification(validation.reason, 'error');
            }
        })
        .catch(error => {
            console.error('Error validating machine assignment:', error);
            showNotification('Error validating machine assignment. Please try again.', 'error');
        });
}

/**
 * Unassign machine from task
 */
function unassignMachine(taskId) {
    if (confirm('Are you sure you want to unassign this machine from the task?')) {
        const token = getAntiForgeryToken();
        fetch('/Task/UnassignMachine', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': token
            },
            body: `taskId=${taskId}&__RequestVerificationToken=${encodeURIComponent(token)}`
        })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            if (data.success) {
                showNotification('Machine unassigned successfully', 'success');
                // Reload page to reflect changes
                window.location.reload();
            } else {
                showNotification(data.message, 'error');
            }
        })
        .catch(error => {
            console.error('Error unassigning machine:', error);
            showNotification('Error unassigning machine. Please try again.', 'error');
        });
    }
}

/**
 * Initialize operator assignment functionality
 */
function initializeOperatorAssignment() {
    // Assign operator buttons
    const assignOperatorButtons = document.querySelectorAll('.assign-operator-btn');
    assignOperatorButtons.forEach(button => {
        button.addEventListener('click', function() {
            const taskId = this.getAttribute('data-task-id');
            showOperatorSelectionModal(taskId);
        });
    });

    // Unassign operator buttons
    const unassignOperatorButtons = document.querySelectorAll('.unassign-operator-btn');
    unassignOperatorButtons.forEach(button => {
        button.addEventListener('click', function() {
            const taskId = this.getAttribute('data-task-id');
            const operatorId = this.getAttribute('data-operator-id');
            unassignOperator(taskId, operatorId);
        });
    });
}

/**
 * Show operator selection modal
 */
function showOperatorSelectionModal(taskId) {
    // Create modal container
    const modalContainer = document.createElement('div');
    modalContainer.className = 'modal-container';
    modalContainer.id = 'operator-selection-modal';

    // Create modal content
    modalContainer.innerHTML = `
        <div class="modal-content">
            <div class="modal-header">
                <h3>Assign Operator</h3>
                <button class="modal-close-btn">&times;</button>
            </div>
            <div class="modal-body">
                <p>Select an operator to assign to this task:</p>
                <div class="loading-spinner">Loading available operators...</div>
                <div class="operators-list"></div>
            </div>
            <div class="modal-footer">
                <button class="modal-cancel-btn">Cancel</button>
            </div>
        </div>
    `;

    // Add modal to body
    document.body.appendChild(modalContainer);

    // Add event listeners
    modalContainer.querySelector('.modal-close-btn').addEventListener('click', () => {
        document.body.removeChild(modalContainer);
    });

    modalContainer.querySelector('.modal-cancel-btn').addEventListener('click', () => {
        document.body.removeChild(modalContainer);
    });

    // Load available operators
    fetch(`/Task/GetAvailableOperators?taskId=${taskId}`)
        .then(response => response.json())
        .then(data => {
            const loadingSpinner = modalContainer.querySelector('.loading-spinner');
            const operatorsList = modalContainer.querySelector('.operators-list');

            loadingSpinner.style.display = 'none';

            if (data.success && data.operators.length > 0) {
                data.operators.forEach(operator => {
                    const operatorItem = document.createElement('div');
                    operatorItem.className = 'operator-item';
                    operatorItem.innerHTML = `
                        <div class="operator-info">
                            <span class="operator-name">${operator.fullName}</span>
                            <span class="operator-status">${operator.availabilityStatus}</span>
                        </div>
                        <button class="assign-btn" data-operator-id="${operator.id}">Assign</button>
                    `;
                    operatorsList.appendChild(operatorItem);

                    // Add event listener to assign button
                    operatorItem.querySelector('.assign-btn').addEventListener('click', () => {
                        assignOperator(taskId, operator.id);
                        document.body.removeChild(modalContainer);
                    });
                });
            } else {
                operatorsList.innerHTML = '<p class="no-operators">No available operators found.</p>';
            }
        })
        .catch(error => {
            console.error('Error loading operators:', error);
            const operatorsList = modalContainer.querySelector('.operators-list');
            const loadingSpinner = modalContainer.querySelector('.loading-spinner');
            loadingSpinner.style.display = 'none';
            operatorsList.innerHTML = '<p class="error-message">Error loading operators. Please try again.</p>';
        });
}

/**
 * Assign operator to task
 */
function assignOperator(taskId, operatorId) {
    const token = getAntiForgeryToken();
    fetch('/Task/AssignOperator', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': token
        },
        body: `taskId=${taskId}&operatorId=${operatorId}&__RequestVerificationToken=${encodeURIComponent(token)}`
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
    })
    .then(data => {
        if (data.success) {
            showNotification('Operator assigned successfully', 'success');
            // Reload page to reflect changes
            window.location.reload();
        } else {
            showNotification(data.message, 'error');
        }
    })
    .catch(error => {
        console.error('Error assigning operator:', error);
        showNotification('Error assigning operator. Please try again.', 'error');
    });
}

/**
 * Unassign operator from task
 */
function unassignOperator(taskId, operatorId) {
    if (confirm('Are you sure you want to unassign this operator from the task?')) {
        const token = getAntiForgeryToken();
        fetch('/Task/UnassignOperator', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': token
            },
            body: `taskId=${taskId}&operatorId=${operatorId}&__RequestVerificationToken=${encodeURIComponent(token)}`
        })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            if (data.success) {
                showNotification('Operator unassigned successfully', 'success');
                // Reload page to reflect changes
                window.location.reload();
            } else {
                showNotification(data.message, 'error');
            }
        })
        .catch(error => {
            console.error('Error unassigning operator:', error);
            showNotification('Error unassigning operator. Please try again.', 'error');
        });
    }
}

/**
 * Show notification
 */
function showNotification(message, type = 'info') {
    const notification = document.createElement('div');
    notification.className = `notification notification-${type}`;
    notification.innerHTML = `
        <div class="notification-content">
            <span class="notification-message">${message}</span>
            <button class="notification-close">&times;</button>
        </div>
    `;

    document.body.appendChild(notification);

    // Add event listener to close button
    notification.querySelector('.notification-close').addEventListener('click', () => {
        document.body.removeChild(notification);
    });

    // Auto-remove after 5 seconds
    setTimeout(() => {
        if (document.body.contains(notification)) {
            document.body.removeChild(notification);
        }
    }, 5000);
}

/**
 * Initialize estimated time buttons functionality
 */
function initializeEstimatedTimeButtons() {
    const setTimeButtons = document.querySelectorAll('.set-time-btn');
    setTimeButtons.forEach(button => {
        button.addEventListener('click', function() {
            const taskId = this.getAttribute('data-task-id');
            const currentTime = this.getAttribute('data-current-time');
            showSetTimeModal(taskId, currentTime);
        });
    });
}

let currentTaskId = null;

/**
 * Show the set estimated time modal
 */
function showSetTimeModal(taskId, currentTime) {
    currentTaskId = taskId;
    const modal = document.getElementById('setTimeModal');
    const input = document.getElementById('estimatedTimeInput');

    if (modal && input) {
        input.value = currentTime || '';
        modal.style.display = 'flex';
        input.focus();

        // Handle Enter key
        input.addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                saveEstimatedTime();
            }
        });

        // Handle Escape key
        document.addEventListener('keydown', function(e) {
            if (e.key === 'Escape') {
                closeSetTimeModal();
            }
        });
    }
}

/**
 * Close the set estimated time modal
 */
function closeSetTimeModal() {
    const modal = document.getElementById('setTimeModal');
    if (modal) {
        modal.style.display = 'none';
        currentTaskId = null;
    }
}

/**
 * Save the estimated time
 */
function saveEstimatedTime() {
    const input = document.getElementById('estimatedTimeInput');
    const estimatedTime = parseInt(input.value);

    if (!estimatedTime || estimatedTime < 1 || estimatedTime > 1000) {
        showNotification('Please enter a valid time between 1 and 1000 hours', 'error');
        return;
    }

    if (!currentTaskId) {
        showNotification('Task ID not found', 'error');
        return;
    }

    const token = getAntiForgeryToken();
    fetch('/Task/SetEstimatedTime', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': token
        },
        body: `taskId=${currentTaskId}&estimatedTime=${estimatedTime}&__RequestVerificationToken=${encodeURIComponent(token)}`
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
    })
    .then(data => {
        if (data.success) {
            showNotification('Estimated time updated successfully', 'success');
            closeSetTimeModal();
            // Reload page to reflect changes
            window.location.reload();
        } else {
            showNotification(data.message || 'Failed to update estimated time', 'error');
        }
    })
    .catch(error => {
        console.error('Error updating estimated time:', error);
        showNotification('An error occurred while updating estimated time', 'error');
    });
}

// Make functions globally available
window.closeSetTimeModal = closeSetTimeModal;
window.saveEstimatedTime = saveEstimatedTime;
