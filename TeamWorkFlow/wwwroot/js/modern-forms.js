/**
 * Modern Forms JavaScript
 * Handles enhanced form interactions and validation
 */

document.addEventListener('DOMContentLoaded', function() {
    console.log('Modern forms initialized');

    initializeFormEnhancements();
    initializeValidation();
    initializeFormAnimations();
    initializeAccessibility();
    initializeResponsiveFeatures();
    initializeDateFormatting();
    resetAllSubmitButtons();
});

/**
 * Reset all submit buttons on page load
 */
function resetAllSubmitButtons() {
    const submitButtons = document.querySelectorAll('.modern-submit-btn');
    submitButtons.forEach(button => {
        resetSubmitButton(button);
    });

    // Also check for server-side validation errors and ensure buttons are enabled
    const hasValidationErrors = document.querySelector('.field-validation-error, .validation-summary-errors, .text-danger');
    if (hasValidationErrors) {
        console.log('Server-side validation errors detected, ensuring submit buttons are enabled');
        submitButtons.forEach(button => {
            resetSubmitButton(button);
        });
    }
}

/**
 * Initialize form enhancements
 */
function initializeFormEnhancements() {
    const forms = document.querySelectorAll('form');
    
    forms.forEach(form => {
        // Add loading state on submit with validation check
        form.addEventListener('submit', function(e) {
            const submitBtn = form.querySelector('.modern-submit-btn');

            // Validate form before setting loading state
            const isFormValid = validateFormBeforeSubmit(form);

            if (isFormValid && submitBtn) {
                submitBtn.classList.add('loading');
                submitBtn.disabled = true;

                // Reset button state after a timeout in case of server-side validation errors
                setTimeout(() => {
                    resetSubmitButton(submitBtn);
                }, 5000); // Reset after 5 seconds if no redirect occurs
            } else if (!isFormValid) {
                // Prevent submission if form is invalid
                e.preventDefault();

                // Ensure button is not in loading state
                if (submitBtn) {
                    resetSubmitButton(submitBtn);
                }

                // Show validation summary
                showValidationSummary(form);

                // Focus on first invalid field
                const firstInvalidField = form.querySelector('.modern-form-input.error, .modern-form-select.error, .modern-form-textarea.error, .modern-form-input.modern-form-error, .modern-form-select.modern-form-error, .modern-form-textarea.modern-form-error');
                if (firstInvalidField) {
                    firstInvalidField.focus();
                    firstInvalidField.scrollIntoView({ behavior: 'smooth', block: 'center' });
                }
            }
        });
        
        // Auto-save functionality and button state management
        const inputs = form.querySelectorAll('.modern-form-input, .modern-form-select, .modern-form-textarea');
        inputs.forEach(input => {
            input.addEventListener('change', function() {
                saveFormData(form);

                // Re-enable submit button if it was disabled due to validation errors
                const submitBtn = form.querySelector('.modern-submit-btn');
                if (submitBtn && submitBtn.disabled && !submitBtn.classList.contains('loading')) {
                    resetSubmitButton(submitBtn);
                }
            });

            input.addEventListener('input', function() {
                // Re-enable submit button on input change
                const submitBtn = form.querySelector('.modern-submit-btn');
                if (submitBtn && submitBtn.disabled && !submitBtn.classList.contains('loading')) {
                    resetSubmitButton(submitBtn);
                }
            });
        });
    });
}

/**
 * Initialize real-time validation
 */
function initializeValidation() {
    const inputs = document.querySelectorAll('.modern-form-input, .modern-form-select, .modern-form-textarea');
    
    inputs.forEach(input => {
        // Real-time validation on blur
        input.addEventListener('blur', function() {
            validateField(this);
        });

        // Real-time validation on input for immediate feedback
        input.addEventListener('input', function() {
            // Only validate if field has been touched (blurred at least once)
            if (this.dataset.touched === 'true') {
                validateField(this);
            }
        });

        // Mark field as touched on first blur
        input.addEventListener('blur', function() {
            this.dataset.touched = 'true';
        }, { once: true });

        // Clear validation on focus
        input.addEventListener('focus', function() {
            clearFieldValidation(this);
        });
        
        // Input formatting
        if (input.type === 'email') {
            input.addEventListener('input', function() {
                // Only convert to lowercase while typing, don't trim yet
                this.value = this.value.toLowerCase();
            });

            input.addEventListener('blur', function() {
                // Trim spaces when user finishes typing
                this.value = this.value.trim();
            });
        }
        
        if (input.type === 'tel') {
            input.addEventListener('input', function() {
                formatPhoneNumber(this);
            });
        }

        // Special handling for select elements
        if (input.tagName === 'SELECT') {
            input.addEventListener('change', function() {
                validateSelectField(this);
            });

            input.addEventListener('blur', function() {
                validateSelectField(this);
            });
        }

        // Add character counter for text inputs with maxlength
        if ((input.type === 'text' || input.tagName === 'TEXTAREA') && input.hasAttribute('maxlength')) {
            addCharacterCounter(input);
        }
        
        if (input.type === 'date' || input.classList.contains('date-input')) {
            input.addEventListener('input', function() {
                if (this.placeholder === 'dd/MM/yyyy') {
                    // This is a converted custom date input
                    validateCustomDate(this);
                } else {
                    validateDate(this);
                }
            });

            input.addEventListener('change', function() {
                if (this.placeholder === 'dd/MM/yyyy') {
                    // This is a converted custom date input
                    validateCustomDate(this);
                } else {
                    validateDate(this);
                }
            });
        }
    });
}

/**
 * Validate individual field based on model constraints
 */
function validateField(field) {
    const value = field.value.trim();
    const isRequired = field.hasAttribute('aria-required') || field.required;
    const fieldName = field.getAttribute('name') || field.getAttribute('id') || 'Field';

    // Clear previous validation
    clearFieldValidation(field);

    // Required field validation
    if (isRequired && !value) {
        showFieldError(field, `${getFieldDisplayName(fieldName)} is required`);
        return false;
    }

    // Skip further validation if field is empty and not required
    if (!value && !isRequired) {
        return true;
    }

    // Model-specific validation based on field names
    if (fieldName.toLowerCase() === 'name' || fieldName.toLowerCase() === 'taskname') {
        // Task Name: 5-100 characters
        if (value.length < 5) {
            showFieldError(field, 'Task name must be at least 5 characters');
            return false;
        }
        if (value.length > 100) {
            showFieldError(field, 'Task name cannot exceed 100 characters');
            return false;
        }
    }

    if (fieldName.toLowerCase() === 'description') {
        // Task Description: 5-1500 characters
        if (value.length < 5) {
            showFieldError(field, 'Description must be at least 5 characters');
            return false;
        }
        if (value.length > 1500) {
            showFieldError(field, 'Description cannot exceed 1500 characters');
            return false;
        }
    }

    if (fieldName.toLowerCase() === 'projectnumber') {
        // Project Number: 6-10 characters, digits only
        if (value.length < 6) {
            showFieldError(field, 'Project number must be at least 6 characters');
            return false;
        }
        if (value.length > 10) {
            showFieldError(field, 'Project number cannot exceed 10 characters');
            return false;
        }
        if (!/^\d+$/.test(value)) {
            showFieldError(field, 'Project number must contain only digits');
            return false;
        }
    }

    // Machine name validation (3-50 characters)
    if (fieldName.toLowerCase() === 'machinename') {
        if (value.length < 3) {
            showFieldError(field, 'Machine name must be at least 3 characters');
            return false;
        }
        if (value.length > 50) {
            showFieldError(field, 'Machine name cannot exceed 50 characters');
            return false;
        }
    }

    // Capacity validation (positive number)
    if (fieldName.toLowerCase() === 'capacity') {
        const numValue = parseFloat(value);
        if (isNaN(numValue) || numValue <= 0) {
            showFieldError(field, 'Capacity must be a positive number');
            return false;
        }
    }

    // Email validation
    if (field.type === 'email' && value) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(value)) {
            showFieldError(field, 'Please enter a valid email address');
            return false;
        }
    }

    // Number validation
    if (field.type === 'number' && value) {
        const numValue = parseFloat(value);
        if (isNaN(numValue)) {
            showFieldError(field, 'Please enter a valid number');
            return false;
        }

        // Check min/max constraints
        const min = field.getAttribute('min');
        const max = field.getAttribute('max');

        if (min !== null && numValue < parseFloat(min)) {
            showFieldError(field, `Value must be at least ${min}`);
            return false;
        }

        if (max !== null && numValue > parseFloat(max)) {
            showFieldError(field, `Value must be no more than ${max}`);
            return false;
        }
    }

    // Custom date validation for dd/MM/yyyy format
    if (field.placeholder === 'dd/MM/yyyy' && value) {
        return validateCustomDate(field);
    }

    // Pattern validation
    const pattern = field.getAttribute('pattern');
    if (pattern && value) {
        const regex = new RegExp(pattern);
        if (!regex.test(value)) {
            const title = field.getAttribute('title') || 'Please enter a valid format';
            showFieldError(field, title);
            return false;
        }
    }

    // Success state - only show for important validations
    if (value && (fieldName.toLowerCase() === 'name' || fieldName.toLowerCase() === 'description' || fieldName.toLowerCase() === 'projectnumber')) {
        showFieldSuccess(field, false); // Don't show success message, just green border
    }

    return true;
}

/**
 * Get user-friendly field display name
 */
function getFieldDisplayName(fieldName) {
    const displayNames = {
        'email': 'Email',
        'password': 'Password',
        'confirmPassword': 'Confirm Password',
        'firstName': 'First Name',
        'lastName': 'Last Name',
        'phoneNumber': 'Phone Number',
        'startDate': 'Start Date',
        'endDate': 'End Date',
        'deadline': 'Deadline',
        'projectNumber': 'Project Number',
        'name': 'Name',
        'description': 'Description',
        'capacity': 'Capacity',
        'totalHours': 'Total Hours',
        'priorityId': 'Priority',
        'statusId': 'Status',
        'operatorId': 'Operator'
    };

    return displayNames[fieldName] || fieldName.charAt(0).toUpperCase() + fieldName.slice(1);
}

/**
 * Add character counter to input field
 */
function addCharacterCounter(input) {
    const maxLength = parseInt(input.getAttribute('maxlength'));
    const minLength = parseInt(input.getAttribute('minlength')) || 0;

    if (!maxLength) return;

    // Create counter element
    const counter = document.createElement('div');
    counter.className = 'character-counter';
    counter.style.cssText = `
        font-size: 0.75rem;
        color: rgba(255, 255, 255, 0.6);
        text-align: right;
        margin-top: 0.25rem;
        font-weight: 400;
    `;

    // Update counter function
    function updateCounter() {
        const currentLength = input.value.length;
        const remaining = maxLength - currentLength;

        if (currentLength < minLength) {
            counter.textContent = `${currentLength}/${maxLength} (min: ${minLength})`;
            counter.style.color = 'rgba(255, 255, 255, 0.6)';
        } else if (remaining <= 10) {
            counter.textContent = `${currentLength}/${maxLength}`;
            counter.style.color = '#fbbf24'; // Warning color
        } else {
            counter.textContent = `${currentLength}/${maxLength}`;
            counter.style.color = 'rgba(255, 255, 255, 0.6)';
        }
    }

    // Add counter after the input
    input.parentNode.appendChild(counter);

    // Update counter on input
    input.addEventListener('input', updateCounter);

    // Initial update
    updateCounter();
}

/**
 * Validate select field
 */
function validateSelectField(selectField) {
    const value = selectField.value;
    const isRequired = selectField.hasAttribute('aria-required') || selectField.required;
    const fieldName = selectField.getAttribute('name') || selectField.getAttribute('id') || 'Field';

    // Clear previous validation
    clearFieldValidation(selectField);

    // Required field validation
    if (isRequired && (!value || value === '' || value === '0')) {
        showFieldError(selectField, `${getFieldDisplayName(fieldName)} is required`);
        return false;
    }

    // Success state - no message, just clean validation
    if (value && value !== '' && value !== '0') {
        showFieldSuccess(selectField, false);
        return true;
    }

    return true;
}

/**
 * Show field error
 */
function showFieldError(field, message) {
    field.classList.add('modern-form-error');
    field.classList.remove('modern-form-success');
    
    // Create or update error message
    let errorElement = field.parentNode.querySelector('.modern-validation-message');
    if (!errorElement) {
        errorElement = document.createElement('div');
        errorElement.className = 'modern-validation-message';
        field.parentNode.appendChild(errorElement);
    }
    errorElement.textContent = message;
}

/**
 * Show field success
 */
function showFieldSuccess(field, showMessage = false) {
    field.classList.add('modern-form-success');
    field.classList.remove('modern-form-error');

    // Remove error message
    const errorElement = field.parentNode.querySelector('.modern-validation-message');
    if (errorElement) {
        errorElement.remove();
    }

    // Optionally show success message for important fields
    if (showMessage) {
        const successElement = field.parentNode.querySelector('.modern-validation-success');
        if (!successElement) {
            const successDiv = document.createElement('div');
            successDiv.className = 'modern-validation-success';
            successDiv.textContent = 'Valid';
            field.parentNode.appendChild(successDiv);

            // Auto-remove success message after 2 seconds
            setTimeout(() => {
                if (successDiv.parentNode) {
                    successDiv.remove();
                }
            }, 2000);
        }
    }
}

/**
 * Clear field validation
 */
function clearFieldValidation(field) {
    field.classList.remove('modern-form-error', 'modern-form-success');

    const errorElement = field.parentNode.querySelector('.modern-validation-message');
    if (errorElement) {
        errorElement.remove();
    }

    const successElement = field.parentNode.querySelector('.modern-validation-success');
    if (successElement) {
        successElement.remove();
    }
}

/**
 * Format phone number
 */
function formatPhoneNumber(input) {
    let value = input.value.replace(/\D/g, '');
    
    if (value.length >= 10) {
        value = value.replace(/(\d{3})(\d{3})(\d{4})/, '$1-$2-$3');
    } else if (value.length >= 6) {
        value = value.replace(/(\d{3})(\d{3})/, '$1-$2');
    } else if (value.length >= 3) {
        value = value.replace(/(\d{3})/, '$1');
    }
    
    input.value = value;
}

/**
 * Validate date input
 */
function validateDate(input) {
    const value = input.value;
    if (!value) return true;

    const date = new Date(value);
    const today = new Date();
    today.setHours(0, 0, 0, 0); // Reset time for accurate comparison

    if (isNaN(date.getTime())) {
        showFieldError(input, 'Please enter a valid date');
        return false;
    }

    // Check if date is in the future (for future dates)
    if (input.classList.contains('future-date') && date < today) {
        showFieldError(input, 'Date must be in the future');
        return false;
    }

    showFieldSuccess(input);
    return true;
}

/**
 * Validate email field
 */
function validateEmail(input) {
    const value = input.value.trim();
    if (!value) return true;

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(value)) {
        showFieldError(input, 'Please enter a valid email address');
        return false;
    }

    showFieldSuccess(input);
    return true;
}

/**
 * Validate number field
 */
function validateNumber(input) {
    const value = input.value.trim();
    if (!value) return true;

    const numValue = parseFloat(value);
    if (isNaN(numValue)) {
        showFieldError(input, 'Please enter a valid number');
        return false;
    }

    // Check min/max constraints
    const min = input.getAttribute('min');
    const max = input.getAttribute('max');

    if (min !== null && numValue < parseFloat(min)) {
        showFieldError(input, `Value must be at least ${min}`);
        return false;
    }

    if (max !== null && numValue > parseFloat(max)) {
        showFieldError(input, `Value must be no more than ${max}`);
        return false;
    }

    showFieldSuccess(input);
    return true;
}

/**
 * Initialize form animations
 */
function initializeFormAnimations() {
    // Animate form groups on scroll
    const formGroups = document.querySelectorAll('.modern-form-group');
    
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, { threshold: 0.1 });
    
    formGroups.forEach((group, index) => {
        group.style.opacity = '0';
        group.style.transform = 'translateY(20px)';
        group.style.transition = `all 0.6s ease ${index * 0.1}s`;
        observer.observe(group);
    });
    
    // Floating label effect
    const inputs = document.querySelectorAll('.modern-form-input, .modern-form-textarea');
    inputs.forEach(input => {
        input.addEventListener('focus', function() {
            this.parentNode.classList.add('focused');
        });
        
        input.addEventListener('blur', function() {
            if (!this.value) {
                this.parentNode.classList.remove('focused');
            }
        });
        
        // Check if input has value on load
        if (input.value) {
            input.parentNode.classList.add('focused');
        }
    });
}

/**
 * Initialize accessibility features
 */
function initializeAccessibility() {
    // Add ARIA labels
    const inputs = document.querySelectorAll('.modern-form-input, .modern-form-select, .modern-form-textarea');
    inputs.forEach(input => {
        const label = input.parentNode.querySelector('.modern-form-label');
        if (label && !input.getAttribute('aria-label')) {
            input.setAttribute('aria-label', label.textContent);
        }
    });
    
    // Keyboard navigation
    document.addEventListener('keydown', function(e) {
        if (e.key === 'Enter' && e.target.classList.contains('modern-form-input')) {
            const form = e.target.closest('form');
            const inputs = Array.from(form.querySelectorAll('.modern-form-input, .modern-form-select, .modern-form-textarea'));
            const currentIndex = inputs.indexOf(e.target);
            
            if (currentIndex < inputs.length - 1) {
                e.preventDefault();
                inputs[currentIndex + 1].focus();
            }
        }
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
            adjustFormLayout();
        }, 250);
    });
    
    // Initial adjustment
    adjustFormLayout();
}

/**
 * Adjust form layout based on screen size
 */
function adjustFormLayout() {
    const screenWidth = window.innerWidth;
    const formContainers = document.querySelectorAll('.modern-form-container');
    
    formContainers.forEach(container => {
        if (screenWidth <= 768) {
            container.classList.add('mobile-layout');
        } else {
            container.classList.remove('mobile-layout');
        }
    });
}

/**
 * Save form data to localStorage
 */
function saveFormData(form) {
    const formData = new FormData(form);
    const data = {};
    
    for (let [key, value] of formData.entries()) {
        data[key] = value;
    }
    
    const formId = form.id || 'default-form';
    localStorage.setItem(`form-data-${formId}`, JSON.stringify(data));
}

/**
 * Load form data from localStorage
 */
function loadFormData(form) {
    const formId = form.id || 'default-form';
    const savedData = localStorage.getItem(`form-data-${formId}`);
    
    if (savedData) {
        const data = JSON.parse(savedData);
        
        Object.keys(data).forEach(key => {
            const input = form.querySelector(`[name="${key}"]`);
            if (input && !input.value) {
                input.value = data[key];
            }
        });
    }
}

/**
 * Clear saved form data
 */
function clearFormData(form) {
    const formId = form.id || 'default-form';
    localStorage.removeItem(`form-data-${formId}`);
}

/**
 * Show form success message
 */
function showFormSuccess(message) {
    const successDiv = document.createElement('div');
    successDiv.className = 'form-success-message';
    successDiv.textContent = message;
    successDiv.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: linear-gradient(135deg, #10b981 0%, #059669 100%);
        color: white;
        padding: 1rem 1.5rem;
        border-radius: 0.5rem;
        box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
        z-index: 9999;
        animation: slideIn 0.3s ease;
    `;
    
    document.body.appendChild(successDiv);
    
    setTimeout(() => {
        successDiv.remove();
    }, 5000);
}

/**
 * Initialize date formatting
 */
function initializeDateFormatting() {
    // Set document locale for date formatting
    document.documentElement.setAttribute('lang', 'en-GB');

    // Try to set browser locale preference
    try {
        if (navigator.language !== 'en-GB') {
            console.log('Browser locale:', navigator.language, '- Setting preference for en-GB');
        }
    } catch (e) {
        // Ignore if not supported
    }

    const dateInputs = document.querySelectorAll('input[type="date"]');
    dateInputs.forEach(input => {
        // Add format hint to show expected format
        addDateFormatHint(input);

        // Set up locale-aware formatting
        setupDateLocale(input);
    });
}

/**
 * Add date format hint to label
 */
function addDateFormatHint(input) {
    const label = input.parentNode.querySelector('.modern-form-label');
    if (label && !label.querySelector('.date-format-hint')) {
        const hint = document.createElement('span');
        hint.className = 'date-format-hint';
        hint.textContent = ' (dd/MM/yyyy)';
        hint.style.cssText = 'font-size: 0.75rem; color: rgba(255, 255, 255, 0.7); font-weight: 500; margin-left: 0.5rem; background: rgba(139, 92, 246, 0.2); padding: 0.125rem 0.375rem; border-radius: 0.25rem; border: 1px solid rgba(139, 92, 246, 0.3);';
        label.appendChild(hint);

        // Add additional format guidance
        const formatNote = document.createElement('div');
        formatNote.className = 'date-format-note';
        formatNote.innerHTML = '<small style="color: rgba(255, 255, 255, 0.6); font-size: 0.7rem; margin-top: 0.25rem; display: block;">📅 Click calendar icon to select date</small>';
        input.parentNode.appendChild(formatNote);
    }
}

/**
 * Setup date locale formatting
 */
function setupDateLocale(input) {
    // Force the input to use en-GB locale for dd/MM/yyyy format
    input.setAttribute('lang', 'en-GB');

    // Convert the input to text type and add date picker functionality
    convertToCustomDateInput(input);
}

/**
 * Convert date input to custom format while keeping calendar functionality
 */
function convertToCustomDateInput(originalInput) {
    // Create a hidden date input for the calendar functionality
    const hiddenDateInput = document.createElement('input');
    hiddenDateInput.type = 'date';
    hiddenDateInput.style.cssText = 'position: absolute; opacity: 0; pointer-events: none; width: 0; height: 0;';

    // Convert original input to text
    originalInput.type = 'text';
    originalInput.placeholder = 'dd/MM/yyyy';
    originalInput.setAttribute('maxlength', '10');
    originalInput.setAttribute('pattern', '\\d{2}/\\d{2}/\\d{4}');

    // Add the hidden input to the DOM
    originalInput.parentNode.appendChild(hiddenDateInput);

    // Create calendar icon overlay
    const calendarOverlay = document.createElement('div');
    calendarOverlay.style.cssText = `
        position: absolute;
        right: 1rem;
        top: 50%;
        transform: translateY(-50%);
        width: 1.5rem;
        height: 1.5rem;
        cursor: pointer;
        z-index: 10;
        pointer-events: auto;
    `;

    // Make the parent container relative if it isn't already
    const parent = originalInput.parentNode;
    if (getComputedStyle(parent).position === 'static') {
        parent.style.position = 'relative';
    }

    parent.appendChild(calendarOverlay);

    // Handle calendar icon click
    calendarOverlay.addEventListener('click', function(e) {
        e.preventDefault();
        e.stopPropagation();

        // Convert current dd/MM/yyyy value to yyyy-mm-dd for the hidden input
        const currentValue = originalInput.value;
        if (currentValue && currentValue.match(/^\d{2}\/\d{2}\/\d{4}$/)) {
            const [day, month, year] = currentValue.split('/');
            hiddenDateInput.value = `${year}-${month}-${day}`;
        }

        // Trigger the hidden date input
        hiddenDateInput.focus();
        hiddenDateInput.click();

        // For modern browsers
        if (hiddenDateInput.showPicker) {
            hiddenDateInput.showPicker();
        }
    });

    // Handle date selection from hidden input
    hiddenDateInput.addEventListener('change', function() {
        if (this.value) {
            // Convert from yyyy-mm-dd to dd/MM/yyyy
            const date = new Date(this.value);
            const day = String(date.getDate()).padStart(2, '0');
            const month = String(date.getMonth() + 1).padStart(2, '0');
            const year = date.getFullYear();

            originalInput.value = `${day}/${month}/${year}`;

            // Trigger validation and events
            validateCustomDate(originalInput);
            originalInput.dispatchEvent(new Event('input', { bubbles: true }));
            originalInput.dispatchEvent(new Event('change', { bubbles: true }));
        }
    });

    // Handle manual typing in the text input
    originalInput.addEventListener('input', function() {
        formatDateInput(this);
    });

    originalInput.addEventListener('blur', function() {
        validateCustomDate(this);
    });

    // Set up form submission - keep dd/MM/yyyy format as server expects it
    const form = originalInput.closest('form');
    if (form) {
        form.addEventListener('submit', function(e) {
            const value = originalInput.value;
            if (value && value.match(/^\d{2}\/\d{2}\/\d{4}$/)) {
                console.log('Form submission - Date value:', value);
                console.log('Server expects dd/MM/yyyy format - keeping as is');
                // No conversion needed - server expects dd/MM/yyyy format
                // The value is already in the correct format: dd/MM/yyyy
            }
        });
    }
}

/**
 * Format date input as user types (dd/MM/yyyy)
 */
function formatDateInput(input) {
    let value = input.value.replace(/\D/g, ''); // Remove non-digits

    if (value.length >= 2) {
        value = value.substring(0, 2) + '/' + value.substring(2);
    }
    if (value.length >= 5) {
        value = value.substring(0, 5) + '/' + value.substring(5, 9);
    }

    input.value = value;
}

/**
 * Validate custom date input (dd/MM/yyyy format)
 */
function validateCustomDate(input) {
    const value = input.value.trim();
    if (!value) return;

    // Check format dd/MM/yyyy
    const dateRegex = /^(\d{2})\/(\d{2})\/(\d{4})$/;
    const match = value.match(dateRegex);

    if (!match) {
        showFieldError(input, 'Date format should be dd/MM/yyyy');
        return false;
    }

    const day = parseInt(match[1], 10);
    const month = parseInt(match[2], 10);
    const year = parseInt(match[3], 10);

    // Validate date components
    if (month < 1 || month > 12) {
        showFieldError(input, 'Month must be between 01 and 12');
        return false;
    }

    if (day < 1 || day > 31) {
        showFieldError(input, 'Day must be between 01 and 31');
        return false;
    }

    // Create date object and validate
    const date = new Date(year, month - 1, day);
    if (date.getDate() !== day || date.getMonth() !== month - 1 || date.getFullYear() !== year) {
        showFieldError(input, 'Invalid date');
        return false;
    }

    // Check if date is in the future (for future dates)
    if (input.classList.contains('future-date')) {
        const today = new Date();
        today.setHours(0, 0, 0, 0);

        if (date < today) {
            showFieldError(input, 'Date must be in the future');
            return false;
        }
    }

    showFieldSuccess(input);
    return true;
}

/**
 * Show form error message
 */
function showFormError(message) {
    const errorDiv = document.createElement('div');
    errorDiv.className = 'form-error-message';
    errorDiv.textContent = message;
    errorDiv.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%);
        color: white;
        padding: 1rem 1.5rem;
        border-radius: 0.5rem;
        box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
        z-index: 9999;
        animation: slideIn 0.3s ease;
    `;
    
    document.body.appendChild(errorDiv);
    
    setTimeout(() => {
        errorDiv.remove();
    }, 5000);
}

/**
 * Validate entire form before submission
 */
function validateFormBeforeSubmit(form) {
    let isValid = true;

    // Check all required fields
    const requiredFields = form.querySelectorAll('.modern-form-input[required], .modern-form-select[required], .modern-form-textarea[required]');
    requiredFields.forEach(field => {
        if (!field.value.trim()) {
            showFieldError(field, 'This field is required');
            isValid = false;
        }
    });

    // Check all fields with error class
    const errorFields = form.querySelectorAll('.modern-form-input.error, .modern-form-select.error, .modern-form-textarea.error, .modern-form-input.modern-form-error, .modern-form-select.modern-form-error, .modern-form-textarea.modern-form-error');
    if (errorFields.length > 0) {
        isValid = false;
    }

    // Validate date fields specifically
    const dateFields = form.querySelectorAll('input[placeholder="dd/MM/yyyy"]');
    dateFields.forEach(field => {
        if (field.value && !validateCustomDate(field)) {
            isValid = false;
        }
    });

    // Validate email fields
    const emailFields = form.querySelectorAll('input[type="email"]');
    emailFields.forEach(field => {
        if (field.value && !validateEmail(field)) {
            isValid = false;
        }
    });

    // Validate number fields
    const numberFields = form.querySelectorAll('input[type="number"]');
    numberFields.forEach(field => {
        if (field.value && !validateNumber(field)) {
            isValid = false;
        }
    });

    return isValid;
}

/**
 * Reset submit button state
 */
function resetSubmitButton(button) {
    if (button) {
        button.classList.remove('loading');
        button.disabled = false;
    }
}

/**
 * Handle page visibility change to reset button states
 */
function handleVisibilityChange() {
    if (!document.hidden) {
        // Page became visible again, reset any loading buttons
        const loadingButtons = document.querySelectorAll('.modern-submit-btn.loading');
        loadingButtons.forEach(button => {
            resetSubmitButton(button);
        });
    }
}

// Add visibility change listener
document.addEventListener('visibilitychange', handleVisibilityChange);

// Add window focus listener to reset button states
window.addEventListener('focus', function() {
    const loadingButtons = document.querySelectorAll('.modern-submit-btn.loading');
    loadingButtons.forEach(button => {
        resetSubmitButton(button);
    });
});

/**
 * Show validation summary for form errors
 */
function showValidationSummary(form) {
    // Remove existing summary
    const existingSummary = form.querySelector('.validation-summary');
    if (existingSummary) {
        existingSummary.remove();
    }

    // Collect all error messages
    const errorMessages = [];
    const errorFields = form.querySelectorAll('.modern-form-input.modern-form-error, .modern-form-select.modern-form-error, .modern-form-textarea.modern-form-error');

    errorFields.forEach(field => {
        const errorElement = field.parentNode.querySelector('.modern-validation-message');
        if (errorElement) {
            const fieldName = getFieldDisplayName(field.getAttribute('name') || field.getAttribute('id') || 'Field');
            errorMessages.push(`${fieldName}: ${errorElement.textContent.replace('⚠️', '').trim()}`);
        }
    });

    if (errorMessages.length > 0) {
        const summaryDiv = document.createElement('div');
        summaryDiv.className = 'validation-summary';
        summaryDiv.style.cssText = `
            background: rgba(239, 68, 68, 0.1);
            border: 1px solid rgba(239, 68, 68, 0.3);
            border-radius: 0.5rem;
            padding: 1rem;
            margin-bottom: 1rem;
            color: #ef4444;
            font-size: 0.875rem;
            animation: slideDown 0.3s ease;
        `;

        const title = document.createElement('h4');
        title.textContent = 'Please correct the following errors:';
        title.style.cssText = 'margin: 0 0 0.5rem 0; font-size: 0.9rem; font-weight: 600;';
        summaryDiv.appendChild(title);

        const errorList = document.createElement('ul');
        errorList.style.cssText = 'margin: 0; padding-left: 1.5rem;';

        errorMessages.forEach(message => {
            const listItem = document.createElement('li');
            listItem.textContent = message;
            listItem.style.cssText = 'margin-bottom: 0.25rem;';
            errorList.appendChild(listItem);
        });

        summaryDiv.appendChild(errorList);

        // Insert at the top of the form
        form.insertBefore(summaryDiv, form.firstChild);

        // Scroll to summary
        summaryDiv.scrollIntoView({ behavior: 'smooth', block: 'start' });

        // Auto-remove after 10 seconds
        setTimeout(() => {
            if (summaryDiv.parentNode) {
                summaryDiv.remove();
            }
        }, 10000);
    }
}

// CSS animations
const style = document.createElement('style');
style.textContent = `
    @keyframes slideIn {
        from {
            transform: translateX(100%);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
`;
document.head.appendChild(style);
