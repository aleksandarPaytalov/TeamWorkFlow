// Sprint To Do - JavaScript functionality
document.addEventListener('DOMContentLoaded', function() {
    initializeSprint();
});

let sprintSortable, backlogSortable;

function initializeSprint() {
    initializeDragAndDrop();
    initializeEventHandlers();
    initializeFilters();
    initializePagination();
}

// Drag and Drop Functionality
function initializeDragAndDrop() {
    const sprintTasks = document.getElementById('sprintTasks');
    const backlogTasks = document.getElementById('backlogTasks');

    if (sprintTasks) {
        sprintSortable = new Sortable(sprintTasks, {
            group: {
                name: 'tasks',
                pull: true,
                put: true
            },
            animation: 150,
            ghostClass: 'sortable-ghost',
            chosenClass: 'sortable-chosen',
            dragClass: 'sortable-drag',
            onAdd: function(evt) {
                const taskId = parseInt(evt.item.dataset.taskId);
                const newIndex = evt.newIndex;
                addTaskToSprint(taskId, newIndex + 1);
            },
            onUpdate: function(evt) {
                updateSprintTaskOrder();
            },
            onMove: function(evt) {
                // Validate if task can be moved to sprint
                if (evt.to.id === 'sprintTasks' && evt.from.id === 'backlogTasks') {
                    const taskId = parseInt(evt.dragged.dataset.taskId);
                    return validateTaskForSprint(taskId);
                }
                return true;
            }
        });
    }

    if (backlogTasks) {
        backlogSortable = new Sortable(backlogTasks, {
            group: {
                name: 'tasks',
                pull: true,
                put: true
            },
            animation: 150,
            ghostClass: 'sortable-ghost',
            chosenClass: 'sortable-chosen',
            dragClass: 'sortable-drag',
            onAdd: function(evt) {
                const taskId = parseInt(evt.item.dataset.taskId);
                removeTaskFromSprint(taskId);
            }
        });
    }
}

// Event Handlers
function initializeEventHandlers() {
    // Auto Assign button
    document.getElementById('autoAssignBtn')?.addEventListener('click', autoAssignTasks);
    
    // Clear Sprint button
    document.getElementById('clearSprintBtn')?.addEventListener('click', clearSprint);
    
    // Refresh button
    document.getElementById('refreshBtn')?.addEventListener('click', refreshPage);
    
    // Task action buttons
    document.addEventListener('click', function(e) {
        if (e.target.closest('.add-task-btn')) {
            const taskCard = e.target.closest('.task-card');
            const taskId = parseInt(taskCard.dataset.taskId);
            const sprintTasks = document.getElementById('sprintTasks');
            const newOrder = sprintTasks.children.length + 1;
            addTaskToSprint(taskId, newOrder);
        }
        
        if (e.target.closest('.remove-task-btn')) {
            const taskCard = e.target.closest('.task-card');
            const taskId = parseInt(taskCard.dataset.taskId);
            removeTaskFromSprint(taskId);
        }
        
        if (e.target.closest('.edit-time-btn')) {
            toggleTimeEditor(e.target.closest('.task-card'));
        }
        
        if (e.target.closest('.save-time-btn')) {
            saveEstimatedTime(e.target.closest('.task-card'));
        }
        
        if (e.target.closest('.cancel-time-btn')) {
            cancelTimeEdit(e.target.closest('.task-card'));
        }
    });
}

// Filters
function initializeFilters() {
    const searchInput = document.getElementById('searchInput');
    const statusFilter = document.getElementById('statusFilter');
    const priorityFilter = document.getElementById('priorityFilter');
    const projectFilter = document.getElementById('projectFilter');
    
    let filterTimeout;
    
    function applyFilters() {
        clearTimeout(filterTimeout);
        filterTimeout = setTimeout(() => {
            const params = new URLSearchParams();
            
            if (searchInput?.value) params.set('searchTerm', searchInput.value);
            if (statusFilter?.value) params.set('statusFilter', statusFilter.value);
            if (priorityFilter?.value) params.set('priorityFilter', priorityFilter.value);
            if (projectFilter?.value) params.set('projectFilter', projectFilter.value);
            
            window.location.search = params.toString();
        }, 500);
    }
    
    searchInput?.addEventListener('input', applyFilters);
    statusFilter?.addEventListener('change', applyFilters);
    priorityFilter?.addEventListener('change', applyFilters);
    projectFilter?.addEventListener('change', applyFilters);
}

// Pagination
function initializePagination() {
    document.addEventListener('click', function(e) {
        if (e.target.closest('.page-link')) {
            e.preventDefault();
            const page = e.target.dataset.page;
            if (page) {
                const params = new URLSearchParams(window.location.search);
                params.set('page', page);
                window.location.search = params.toString();
            }
        }
    });
}

// API Functions
async function addTaskToSprint(taskId, sprintOrder) {
    try {
        showLoading();
        
        const formData = new FormData();
        formData.append('taskId', taskId);
        formData.append('sprintOrder', sprintOrder);
        formData.append('__RequestVerificationToken', getAntiForgeryToken());
        
        const response = await fetch('/Sprint/AddTaskToSprint', {
            method: 'POST',
            body: formData
        });
        
        const result = await response.json();
        
        if (result.success) {
            showToast('success', result.message);
            setTimeout(() => window.location.reload(), 1000);
        } else {
            showToast('error', result.message);
            // Revert the drag operation
            setTimeout(() => window.location.reload(), 500);
        }
    } catch (error) {
        showToast('error', 'An error occurred while adding the task');
        setTimeout(() => window.location.reload(), 500);
    } finally {
        hideLoading();
    }
}

async function removeTaskFromSprint(taskId) {
    try {
        showLoading();
        
        const formData = new FormData();
        formData.append('taskId', taskId);
        formData.append('__RequestVerificationToken', getAntiForgeryToken());
        
        const response = await fetch('/Sprint/RemoveTaskFromSprint', {
            method: 'POST',
            body: formData
        });
        
        const result = await response.json();
        
        if (result.success) {
            showToast('success', result.message);
            setTimeout(() => window.location.reload(), 1000);
        } else {
            showToast('error', result.message);
            setTimeout(() => window.location.reload(), 500);
        }
    } catch (error) {
        showToast('error', 'An error occurred while removing the task');
        setTimeout(() => window.location.reload(), 500);
    } finally {
        hideLoading();
    }
}

async function updateSprintTaskOrder() {
    try {
        const sprintTasks = document.getElementById('sprintTasks');
        const taskOrders = {};
        
        Array.from(sprintTasks.children).forEach((card, index) => {
            if (card.dataset.taskId) {
                taskOrders[parseInt(card.dataset.taskId)] = index + 1;
            }
        });
        
        const response = await fetch('/Sprint/UpdateTaskOrder', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiForgeryToken()
            },
            body: JSON.stringify(taskOrders)
        });
        
        const result = await response.json();
        
        if (!result.success) {
            showToast('error', result.message);
            setTimeout(() => window.location.reload(), 500);
        }
    } catch (error) {
        console.error('Error updating task order:', error);
    }
}

async function autoAssignTasks() {
    if (!confirm('Auto-assign tasks to sprint based on priority and capacity?')) {
        return;
    }
    
    try {
        showLoading();
        
        const formData = new FormData();
        formData.append('maxTasks', 10);
        formData.append('__RequestVerificationToken', getAntiForgeryToken());
        
        const response = await fetch('/Sprint/AutoAssignTasks', {
            method: 'POST',
            body: formData
        });
        
        const result = await response.json();
        
        if (result.success) {
            showToast('success', result.message);
            setTimeout(() => window.location.reload(), 1000);
        } else {
            showToast('warning', result.message);
        }
    } catch (error) {
        showToast('error', 'An error occurred during auto-assignment');
    } finally {
        hideLoading();
    }
}

async function clearSprint() {
    if (!confirm('Remove all tasks from the sprint? This action cannot be undone.')) {
        return;
    }
    
    try {
        showLoading();
        
        const formData = new FormData();
        formData.append('__RequestVerificationToken', getAntiForgeryToken());
        
        const response = await fetch('/Sprint/ClearSprint', {
            method: 'POST',
            body: formData
        });
        
        const result = await response.json();
        
        if (result.success) {
            showToast('success', result.message);
            setTimeout(() => window.location.reload(), 1000);
        } else {
            showToast('error', result.message);
        }
    } catch (error) {
        showToast('error', 'An error occurred while clearing the sprint');
    } finally {
        hideLoading();
    }
}

// Time Editor Functions
function toggleTimeEditor(taskCard) {
    const editor = taskCard.querySelector('.task-time-editor');
    const isVisible = editor.style.display !== 'none';
    
    if (isVisible) {
        editor.style.display = 'none';
    } else {
        editor.style.display = 'flex';
        const input = editor.querySelector('input');
        input.focus();
        input.select();
    }
}

async function saveEstimatedTime(taskCard) {
    const taskId = parseInt(taskCard.dataset.taskId);
    const input = taskCard.querySelector('.task-time-editor input');
    const estimatedTime = parseInt(input.value);
    
    if (estimatedTime <= 0) {
        showToast('error', 'Estimated time must be greater than 0');
        return;
    }
    
    try {
        showLoading();
        
        const formData = new FormData();
        formData.append('taskId', taskId);
        formData.append('estimatedTime', estimatedTime);
        formData.append('__RequestVerificationToken', getAntiForgeryToken());
        
        const response = await fetch('/Sprint/UpdateEstimatedTime', {
            method: 'POST',
            body: formData
        });
        
        const result = await response.json();
        
        if (result.success) {
            showToast('success', result.message);
            // Update the duration display
            const durationSpan = taskCard.querySelector('.task-duration');
            if (durationSpan) {
                durationSpan.textContent = formatDuration(estimatedTime);
            }
            cancelTimeEdit(taskCard);
        } else {
            showToast('error', result.message);
        }
    } catch (error) {
        showToast('error', 'An error occurred while updating estimated time');
    } finally {
        hideLoading();
    }
}

function cancelTimeEdit(taskCard) {
    const editor = taskCard.querySelector('.task-time-editor');
    editor.style.display = 'none';
    
    // Reset input value to original
    const input = editor.querySelector('input');
    const originalValue = taskCard.querySelector('.task-duration').textContent;
    // Parse duration back to hours (simplified)
    const hours = parseInt(originalValue) || 1;
    input.value = hours;
}

// Utility Functions
function validateTaskForSprint(taskId) {
    // This would typically make an API call to validate
    // For now, return true to allow the drag operation
    return true;
}

function formatDuration(hours) {
    if (hours < 8) return `${hours}h`;
    const days = Math.floor(hours / 8);
    const remainingHours = hours % 8;
    if (remainingHours === 0) return `${days}d`;
    return `${days}d ${remainingHours}h`;
}

function getAntiForgeryToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]')?.value || 
           document.querySelector('meta[name="__RequestVerificationToken"]')?.content || '';
}

function showLoading() {
    document.body.classList.add('loading');
}

function hideLoading() {
    document.body.classList.remove('loading');
}

function refreshPage() {
    window.location.reload();
}

function showToast(type, message) {
    // Use existing toastr if available, otherwise use alert
    if (typeof toastr !== 'undefined') {
        toastr[type](message);
    } else {
        alert(message);
    }
}
