// Bulk Operations JavaScript
// Handles checkbox selection, select all/none functionality, and AJAX calls for bulk operations

class BulkOperations {
    constructor(entityType) {
        this.entityType = entityType; // 'task', 'project', 'machine', 'operator', 'part'
        this.selectedItems = new Set();
        this.init();
    }

    init() {
        this.bindEvents();
        this.updateUI();
    }

    bindEvents() {
        // Select all checkbox
        const selectAllCheckbox = document.getElementById('select-all-checkbox');
        if (selectAllCheckbox) {
            selectAllCheckbox.addEventListener('change', (e) => {
                this.handleSelectAll(e.target.checked);
            });
        }

        // Individual item checkboxes
        const itemCheckboxes = document.querySelectorAll(`.${this.entityType}-select-checkbox`);
        itemCheckboxes.forEach(checkbox => {
            checkbox.addEventListener('change', (e) => {
                this.handleItemSelection(e.target);
            });
        });

        // Bulk action buttons
        const bulkDeleteBtn = document.getElementById('bulk-delete-btn');
        if (bulkDeleteBtn) {
            bulkDeleteBtn.addEventListener('click', () => {
                this.handleBulkDelete();
            });
        }

        const bulkArchiveBtn = document.getElementById('bulk-archive-btn');
        if (bulkArchiveBtn) {
            bulkArchiveBtn.addEventListener('click', () => {
                this.handleBulkArchive();
            });
        }
    }

    handleSelectAll(isChecked) {
        const itemCheckboxes = document.querySelectorAll(`.${this.entityType}-select-checkbox`);
        
        itemCheckboxes.forEach(checkbox => {
            checkbox.checked = isChecked;
            const itemId = parseInt(checkbox.dataset[`${this.entityType}Id`]);
            
            if (isChecked) {
                this.selectedItems.add(itemId);
            } else {
                this.selectedItems.delete(itemId);
            }
        });

        this.updateUI();
    }

    handleItemSelection(checkbox) {
        const itemId = parseInt(checkbox.dataset[`${this.entityType}Id`]);
        
        if (checkbox.checked) {
            this.selectedItems.add(itemId);
        } else {
            this.selectedItems.delete(itemId);
        }

        // Update select all checkbox state
        const selectAllCheckbox = document.getElementById('select-all-checkbox');
        const itemCheckboxes = document.querySelectorAll(`.${this.entityType}-select-checkbox`);
        const checkedCount = document.querySelectorAll(`.${this.entityType}-select-checkbox:checked`).length;
        
        if (selectAllCheckbox) {
            selectAllCheckbox.checked = checkedCount === itemCheckboxes.length;
            selectAllCheckbox.indeterminate = checkedCount > 0 && checkedCount < itemCheckboxes.length;
        }

        this.updateUI();
    }

    updateUI() {
        const bulkOperations = document.getElementById('bulk-operations');
        const selectedCount = document.getElementById('selected-count');
        
        if (this.selectedItems.size > 0) {
            if (bulkOperations) bulkOperations.style.display = 'block';
            if (selectedCount) selectedCount.textContent = `${this.selectedItems.size} selected`;
        } else {
            if (bulkOperations) bulkOperations.style.display = 'none';
            if (selectedCount) selectedCount.textContent = '0 selected';
        }
    }

    async handleBulkDelete() {
        if (this.selectedItems.size === 0) {
            this.showNotification('No items selected for deletion', 'warning');
            return;
        }

        const entityName = this.entityType === 'machine' ? 'CMM' : this.entityType;
        const confirmMessage = `Are you sure you want to delete ${this.selectedItems.size} ${entityName}${this.selectedItems.size > 1 ? 's' : ''}? This action cannot be undone.`;
        
        if (!confirm(confirmMessage)) {
            return;
        }

        try {
            this.showLoading(true);
            
            const response = await fetch(`/${this.capitalizeFirst(this.entityType)}/BulkDelete`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify({
                    ItemIds: Array.from(this.selectedItems)
                })
            });

            const result = await response.json();
            
            if (result.success) {
                this.showNotification(result.message, 'success');
                this.refreshPage();
            } else {
                this.showNotification(result.message || 'An error occurred during deletion', 'error');
                if (result.errors && result.errors.length > 0) {
                    console.error('Deletion errors:', result.errors);
                }
            }
        } catch (error) {
            console.error('Bulk delete error:', error);
            this.showNotification('An error occurred during deletion', 'error');
        } finally {
            this.showLoading(false);
        }
    }

    async handleBulkArchive() {
        if (this.entityType !== 'task') {
            console.error('Bulk archive is only available for tasks');
            return;
        }

        if (this.selectedItems.size === 0) {
            this.showNotification('No tasks selected for archiving', 'warning');
            return;
        }

        const confirmMessage = `Are you sure you want to archive ${this.selectedItems.size} task${this.selectedItems.size > 1 ? 's' : ''}? This will mark them as finished.`;
        
        if (!confirm(confirmMessage)) {
            return;
        }

        try {
            this.showLoading(true);
            
            const response = await fetch('/Task/BulkArchive', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify({
                    TaskIds: Array.from(this.selectedItems)
                })
            });

            const result = await response.json();
            
            if (result.success) {
                this.showNotification(result.message, 'success');
                this.refreshPage();
            } else {
                this.showNotification(result.message || 'An error occurred during archiving', 'error');
                if (result.errors && result.errors.length > 0) {
                    console.error('Archiving errors:', result.errors);
                }
            }
        } catch (error) {
            console.error('Bulk archive error:', error);
            this.showNotification('An error occurred during archiving', 'error');
        } finally {
            this.showLoading(false);
        }
    }

    getAntiForgeryToken() {
        const token = document.querySelector('input[name="__RequestVerificationToken"]');
        return token ? token.value : '';
    }

    capitalizeFirst(str) {
        return str.charAt(0).toUpperCase() + str.slice(1);
    }

    showLoading(show) {
        const bulkDeleteBtn = document.getElementById('bulk-delete-btn');
        const bulkArchiveBtn = document.getElementById('bulk-archive-btn');
        
        if (show) {
            if (bulkDeleteBtn) {
                bulkDeleteBtn.disabled = true;
                bulkDeleteBtn.innerHTML = '<span class="spinner"></span> Processing...';
            }
            if (bulkArchiveBtn) {
                bulkArchiveBtn.disabled = true;
                bulkArchiveBtn.innerHTML = '<span class="spinner"></span> Processing...';
            }
        } else {
            if (bulkDeleteBtn) {
                bulkDeleteBtn.disabled = false;
                bulkDeleteBtn.innerHTML = `
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path>
                    </svg>
                    Delete Selected
                `;
            }
            if (bulkArchiveBtn) {
                bulkArchiveBtn.disabled = false;
                bulkArchiveBtn.innerHTML = `
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 8l6 6 6-6"></path>
                    </svg>
                    Archive Selected
                `;
            }
        }
    }

    showNotification(message, type = 'info') {
        // Create notification element
        const notification = document.createElement('div');
        notification.className = `notification notification-${type}`;
        notification.innerHTML = `
            <div class="notification-content">
                <span class="notification-message">${message}</span>
                <button class="notification-close">&times;</button>
            </div>
        `;

        // Add to page
        document.body.appendChild(notification);

        // Auto remove after 5 seconds
        setTimeout(() => {
            if (notification.parentNode) {
                notification.parentNode.removeChild(notification);
            }
        }, 5000);

        // Close button functionality
        const closeBtn = notification.querySelector('.notification-close');
        closeBtn.addEventListener('click', () => {
            if (notification.parentNode) {
                notification.parentNode.removeChild(notification);
            }
        });
    }

    refreshPage() {
        setTimeout(() => {
            window.location.reload();
        }, 1500);
    }
}

// Initialize bulk operations based on current page
document.addEventListener('DOMContentLoaded', function() {
    const currentPath = window.location.pathname.toLowerCase();
    
    let entityType = null;
    if (currentPath.includes('/task')) {
        entityType = 'task';
    } else if (currentPath.includes('/project')) {
        entityType = 'project';
    } else if (currentPath.includes('/machine')) {
        entityType = 'machine';
    } else if (currentPath.includes('/operator')) {
        entityType = 'operator';
    } else if (currentPath.includes('/part')) {
        entityType = 'part';
    }

    if (entityType && document.getElementById('select-all-checkbox')) {
        window.bulkOperations = new BulkOperations(entityType);
    }
});
