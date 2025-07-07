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
});

/**
 * Initialize form enhancements
 */
function initializeFormEnhancements() {
    const forms = document.querySelectorAll('form');
    
    forms.forEach(form => {
        // Add loading state on submit
        form.addEventListener('submit', function() {
            const submitBtn = form.querySelector('.modern-submit-btn');
            if (submitBtn) {
                submitBtn.classList.add('loading');
                submitBtn.disabled = true;
            }
        });
        
        // Auto-save functionality (optional)
        const inputs = form.querySelectorAll('.modern-form-input, .modern-form-select, .modern-form-textarea');
        inputs.forEach(input => {
            input.addEventListener('change', function() {
                saveFormData(form);
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
        
        // Clear validation on focus
        input.addEventListener('focus', function() {
            clearFieldValidation(this);
        });
        
        // Input formatting
        if (input.type === 'email') {
            input.addEventListener('input', function() {
                this.value = this.value.toLowerCase().trim();
            });
        }
        
        if (input.type === 'tel') {
            input.addEventListener('input', function() {
                formatPhoneNumber(this);
            });
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
 * Validate individual field
 */
function validateField(field) {
    const value = field.value.trim();
    const isRequired = field.hasAttribute('aria-required') || field.required;
    
    // Clear previous validation
    clearFieldValidation(field);
    
    // Required field validation
    if (isRequired && !value) {
        showFieldError(field, 'This field is required');
        return false;
    }
    
    // Email validation
    if (field.type === 'email' && value) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(value)) {
            showFieldError(field, 'Please enter a valid email address');
            return false;
        }
    }
    
    // Phone validation
    if (field.type === 'tel' && value) {
        const phoneRegex = /^[\+]?[1-9][\d]{0,15}$/;
        if (!phoneRegex.test(value.replace(/\s/g, ''))) {
            showFieldError(field, 'Please enter a valid phone number');
            return false;
        }
    }
    
    // URL validation
    if (field.type === 'url' && value) {
        try {
            new URL(value);
        } catch {
            showFieldError(field, 'Please enter a valid URL');
            return false;
        }
    }
    
    // Success state
    showFieldSuccess(field);
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
function showFieldSuccess(field) {
    field.classList.add('modern-form-success');
    field.classList.remove('modern-form-error');
    
    // Remove error message
    const errorElement = field.parentNode.querySelector('.modern-validation-message');
    if (errorElement) {
        errorElement.remove();
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
    if (!value) return;

    const date = new Date(value);
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    if (isNaN(date.getTime())) {
        showFieldError(input, 'Please enter a valid date');
        return;
    }

    // Check if date is in the future (for future dates)
    if (input.classList.contains('future-date') && date < today) {
        showFieldError(input, 'Date must be in the future');
        return;
    }

    showFieldSuccess(input);
}

/**
 * Validate date input
 */
function validateDate(input) {
    const value = input.value;
    if (!value) return;

    const date = new Date(value);
    const today = new Date();
    today.setHours(0, 0, 0, 0); // Reset time for accurate comparison

    if (isNaN(date.getTime())) {
        showFieldError(input, 'Please enter a valid date');
        return;
    }

    // Check if date is in the past (for future dates)
    if (input.classList.contains('future-date') && date < today) {
        showFieldError(input, 'Date must be in the future');
        return;
    }

    showFieldSuccess(input);
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
        showFieldError(input, 'Please enter date in dd/MM/yyyy format');
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
        showFieldError(input, 'Please enter a valid date');
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
