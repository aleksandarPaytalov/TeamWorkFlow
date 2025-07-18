/**
 * Responsive Header JavaScript
 * Handles mobile navigation toggle and responsive behavior
 */

document.addEventListener('DOMContentLoaded', function() {
    console.log('Responsive header initialized');
    
    initializeMobileNavigation();
    initializeResponsiveFeatures();
    initializeAccessibilityFeatures();
    initializePerformanceOptimizations();
});

/**
 * Initialize mobile navigation toggle functionality
 */
function initializeMobileNavigation() {
    const toggler = document.querySelector('.navbar-toggler');
    const collapse = document.querySelector('#navbarNav');

    if (toggler && collapse) {
        // Toggle navigation on button click
        toggler.addEventListener('click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            toggleNavigation();
        });

        // Close navigation when clicking outside
        document.addEventListener('click', function(e) {
            if (!toggler.contains(e.target) && !collapse.contains(e.target)) {
                closeNavigation();
            }
        });

        // Close navigation when pressing Escape key
        document.addEventListener('keydown', function(e) {
            if (e.key === 'Escape') {
                closeNavigation();
            }
        });

        // Close navigation when clicking on nav links (mobile)
        const navLinks = collapse.querySelectorAll('.nav-link');
        navLinks.forEach(link => {
            link.addEventListener('click', function() {
                if (window.innerWidth <= 991) {
                    setTimeout(() => closeNavigation(), 150);
                }
            });
        });
    }
}

/**
 * Toggle navigation visibility
 */
function toggleNavigation() {
    const collapse = document.querySelector('#navbarNav');
    const toggler = document.querySelector('.navbar-toggler');

    if (collapse && toggler) {
        const isOpen = collapse.classList.contains('show');

        if (isOpen) {
            closeNavigation();
        } else {
            openNavigation();
        }
    }
}

/**
 * Open navigation
 */
function openNavigation() {
    const collapse = document.querySelector('#navbarNav');
    const toggler = document.querySelector('.navbar-toggler');

    if (collapse && toggler) {
        collapse.classList.add('show');
        toggler.setAttribute('aria-expanded', 'true');

        // Add animation
        collapse.style.opacity = '0';
        collapse.style.transform = 'translateY(-10px)';

        setTimeout(() => {
            collapse.style.transition = 'all 0.3s ease';
            collapse.style.opacity = '1';
            collapse.style.transform = 'translateY(0)';
        }, 10);

        // Focus first nav link for accessibility
        const firstNavLink = collapse.querySelector('.nav-link');
        if (firstNavLink) {
            setTimeout(() => firstNavLink.focus(), 100);
        }
    }
}

/**
 * Close navigation
 */
function closeNavigation() {
    const collapse = document.querySelector('#navbarNav');
    const toggler = document.querySelector('.navbar-toggler');

    if (collapse && toggler) {
        collapse.style.transition = 'all 0.3s ease';
        collapse.style.opacity = '0';
        collapse.style.transform = 'translateY(-10px)';

        setTimeout(() => {
            collapse.classList.remove('show');
            toggler.setAttribute('aria-expanded', 'false');
            collapse.style.transition = '';
            collapse.style.opacity = '';
            collapse.style.transform = '';
        }, 300);
    }
}

/**
 * Initialize responsive features
 */
function initializeResponsiveFeatures() {
    let resizeTimeout;
    
    // Handle window resize
    window.addEventListener('resize', function() {
        clearTimeout(resizeTimeout);
        resizeTimeout = setTimeout(() => {
            handleResize();
        }, 250);
    });
    
    // Initial setup
    handleResize();
}

/**
 * Handle window resize events
 */
function handleResize() {
    const screenWidth = window.innerWidth;
    const collapse = document.querySelector('#navbarNav');
    const toggler = document.querySelector('.navbar-toggler');

    // Close mobile menu when switching to desktop
    if (screenWidth > 991 && collapse && collapse.classList.contains('show')) {
        closeNavigation();
    }

    // Update ARIA attributes based on screen size
    if (toggler) {
        if (screenWidth <= 991) {
            toggler.style.display = 'block';
            toggler.setAttribute('aria-hidden', 'false');
        } else {
            toggler.style.display = 'none';
            toggler.setAttribute('aria-hidden', 'true');
        }
    }
    
    // Optimize navigation layout
    optimizeNavigationLayout(screenWidth);
}

/**
 * Optimize navigation layout based on screen size
 */
function optimizeNavigationLayout(screenWidth) {
    const navbar = document.querySelector('.navbar');
    const navLinks = document.querySelectorAll('.navbar-nav .nav-link');
    
    if (navbar) {
        // Add screen size class for CSS targeting
        navbar.classList.remove('screen-xs', 'screen-sm', 'screen-md', 'screen-lg', 'screen-xl');
        
        if (screenWidth <= 360) {
            navbar.classList.add('screen-xs');
        } else if (screenWidth <= 575) {
            navbar.classList.add('screen-sm');
        } else if (screenWidth <= 768) {
            navbar.classList.add('screen-md');
        } else if (screenWidth <= 992) {
            navbar.classList.add('screen-lg');
        } else {
            navbar.classList.add('screen-xl');
        }
    }
    
    // Optimize text length for small screens
    if (screenWidth <= 768) {
        navLinks.forEach(link => {
            const originalText = link.getAttribute('data-original-text') || link.textContent;
            link.setAttribute('data-original-text', originalText);

            // Shorten text for very small screens
            if (screenWidth <= 360) {
                const shortText = originalText.length > 6 ? originalText.substring(0, 6) + '...' : originalText;
                link.textContent = shortText;
            } else {
                link.textContent = originalText;
            }
        });
    } else {
        // Restore original text on larger screens
        navLinks.forEach(link => {
            const originalText = link.getAttribute('data-original-text');
            if (originalText) {
                link.textContent = originalText;
            }
        });
    }
}

/**
 * Initialize accessibility features
 */
function initializeAccessibilityFeatures() {
    const navbar = document.querySelector('.navbar');
    const toggler = document.querySelector('.navbar-toggler');
    const collapse = document.querySelector('.navbar-collapse');
    
    // Add ARIA labels
    if (navbar) {
        navbar.setAttribute('role', 'navigation');
        navbar.setAttribute('aria-label', 'Main navigation');
    }
    
    if (toggler) {
        toggler.setAttribute('aria-label', 'Toggle navigation menu');
        toggler.setAttribute('aria-expanded', 'false');
        toggler.setAttribute('aria-controls', 'navbar-collapse');
    }
    
    if (collapse) {
        collapse.setAttribute('id', 'navbar-collapse');
    }
    
    // Add keyboard navigation
    const navLinks = document.querySelectorAll('.navbar-nav .nav-link');
    navLinks.forEach((link, index) => {
        link.addEventListener('keydown', function(e) {
            if (e.key === 'ArrowDown' || e.key === 'ArrowRight') {
                e.preventDefault();
                const nextLink = navLinks[index + 1] || navLinks[0];
                nextLink.focus();
            } else if (e.key === 'ArrowUp' || e.key === 'ArrowLeft') {
                e.preventDefault();
                const prevLink = navLinks[index - 1] || navLinks[navLinks.length - 1];
                prevLink.focus();
            }
        });
    });
}

/**
 * Initialize performance optimizations
 */
function initializePerformanceOptimizations() {
    // Debounce scroll events for better performance
    let scrollTimeout;
    let lastScrollY = window.scrollY;
    
    window.addEventListener('scroll', function() {
        clearTimeout(scrollTimeout);
        scrollTimeout = setTimeout(() => {
            handleScroll();
        }, 10);
    });
    
    function handleScroll() {
        const currentScrollY = window.scrollY;
        const navbar = document.querySelector('.navbar');
        
        if (navbar) {
            // Add scrolled class for styling
            if (currentScrollY > 50) {
                navbar.classList.add('scrolled');
            } else {
                navbar.classList.remove('scrolled');
            }
            
            // Hide/show navbar on scroll (mobile only)
            if (window.innerWidth <= 768) {
                if (currentScrollY > lastScrollY && currentScrollY > 100) {
                    navbar.style.transform = 'translateY(-100%)';
                } else {
                    navbar.style.transform = 'translateY(0)';
                }
            } else {
                navbar.style.transform = 'translateY(0)';
            }
        }
        
        lastScrollY = currentScrollY;
    }
    
    // Optimize animations for low-end devices
    if (navigator.hardwareConcurrency && navigator.hardwareConcurrency <= 2) {
        document.body.classList.add('low-performance');
    }
    
    // Reduce animations on battery saver mode
    if (navigator.getBattery) {
        navigator.getBattery().then(function(battery) {
            if (battery.charging === false && battery.level < 0.2) {
                document.body.classList.add('battery-saver');
            }
        });
    }
}

/**
 * Utility function to check if device is touch-enabled
 */
function isTouchDevice() {
    return 'ontouchstart' in window || navigator.maxTouchPoints > 0;
}

/**
 * Add touch-specific enhancements
 */
if (isTouchDevice()) {
    document.addEventListener('DOMContentLoaded', function() {
        document.body.classList.add('touch-device');
        
        // Add touch feedback for navigation buttons
        const navButtons = document.querySelectorAll('.navbar-nav .nav-link');
        navButtons.forEach(button => {
            button.addEventListener('touchstart', function() {
                this.style.transform = 'scale(0.98)';
            });
            
            button.addEventListener('touchend', function() {
                this.style.transform = '';
            });
        });
    });
}
