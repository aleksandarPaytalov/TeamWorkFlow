/**
 * Projects Page JavaScript
 * Handles interactive functionality for the Projects listing page
 */

class ProjectsPage {
    constructor() {
        this.init();
    }

    init() {
        this.setupEventListeners();
        this.setupAnimations();
        this.setupSearch();
        this.setupCardInteractions();
        this.setupResponsiveFeatures();
        this.checkEmptyState();
        this.setupAccessibility();
    }

    setupEventListeners() {
        // Auto-submit form when sorting changes
        const sortingSelect = document.getElementById('sorting-select');
        if (sortingSelect) {
            sortingSelect.addEventListener('change', (e) => {
                this.handleSortingChange(e);
            });
        }

        // Enhanced search functionality
        const searchInput = document.getElementById('search-input');
        if (searchInput) {
            searchInput.addEventListener('input', (e) => {
                this.handleSearchInput(e);
            });
        }

        // Form submission with loading state
        const searchForm = document.getElementById('search-form');
        if (searchForm) {
            searchForm.addEventListener('submit', (e) => {
                this.handleFormSubmit(e);
            });
        }

        // Clear search functionality
        const clearButton = document.getElementById('clear-search');
        if (clearButton) {
            clearButton.addEventListener('click', (e) => {
                this.handleClearSearch(e);
            });
        }

        // Pagination with smooth scrolling
        const paginationLinks = document.querySelectorAll('.page-link');
        paginationLinks.forEach(link => {
            link.addEventListener('click', (e) => {
                this.handlePaginationClick(e);
            });
        });
    }

    setupAnimations() {
        // Animate cards on page load
        this.animateCardsOnLoad();
        
        // Setup intersection observer for scroll animations
        this.setupScrollAnimations();
    }

    setupSearch() {
        // Add search suggestions/autocomplete if needed
        const searchInput = document.getElementById('search-input');
        if (searchInput) {
            // Add debounced search functionality
            let searchTimeout;
            searchInput.addEventListener('input', (e) => {
                clearTimeout(searchTimeout);
                searchTimeout = setTimeout(() => {
                    this.performLiveSearch(e.target.value);
                }, 300);
            });
        }
    }

    setupCardInteractions() {
        const projectCards = document.querySelectorAll('.project-card');
        
        projectCards.forEach(card => {
            // Add hover effects
            card.addEventListener('mouseenter', () => {
                this.handleCardHover(card, true);
            });
            
            card.addEventListener('mouseleave', () => {
                this.handleCardHover(card, false);
            });

            // Add click to expand functionality
            card.addEventListener('click', (e) => {
                if (!e.target.closest('.project-card-actions')) {
                    this.handleCardClick(card);
                }
            });
        });
    }

    setupResponsiveFeatures() {
        // Handle responsive layout changes
        window.addEventListener('resize', () => {
            this.handleResize();
        });

        // Setup mobile-specific interactions
        if (this.isMobile()) {
            this.setupMobileFeatures();
        }
    }

    handleSortingChange(e) {
        const form = e.target.closest('form');
        const submitButton = form.querySelector('input[type="submit"]');
        
        // Add loading state
        this.setLoadingState(submitButton, true);
        
        // Submit form
        form.submit();
    }

    handleSearchInput(e) {
        const value = e.target.value;
        
        // Add visual feedback for search
        if (value.length > 0) {
            e.target.classList.add('has-content');
        } else {
            e.target.classList.remove('has-content');
        }
    }

    handleFormSubmit(e) {
        const submitButton = e.target.querySelector('input[type="submit"]');
        this.setLoadingState(submitButton, true);
        
        // Add fade out effect to current results
        const projectsGrid = document.querySelector('.projects-grid');
        if (projectsGrid) {
            projectsGrid.style.opacity = '0.5';
        }
    }

    handleClearSearch(e) {
        e.preventDefault();
        
        // Clear form inputs
        const form = document.getElementById('search-form');
        const inputs = form.querySelectorAll('input[type="text"], select');
        
        inputs.forEach(input => {
            if (input.type === 'text') {
                input.value = '';
            } else if (input.tagName === 'SELECT') {
                input.selectedIndex = 0;
            }
        });

        // Submit form to show all results
        form.submit();
    }

    handlePaginationClick(e) {
        e.preventDefault();
        
        // Add loading state
        const link = e.target;
        const originalText = link.textContent;
        link.innerHTML = '<span class="loading-spinner"></span>';
        
        // Smooth scroll to top
        window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
        
        // Navigate after scroll
        setTimeout(() => {
            window.location.href = link.href;
        }, 300);
    }

    handleCardHover(card, isHovering) {
        if (isHovering) {
            card.style.transform = 'translateY(-8px) scale(1.02)';
            card.style.boxShadow = '0 20px 40px rgba(0, 0, 0, 0.15)';
        } else {
            card.style.transform = '';
            card.style.boxShadow = '';
        }
    }

    handleCardClick(card) {
        // Add ripple effect
        this.createRippleEffect(card);
        
        // Optional: Navigate to details page
        const detailsLink = card.querySelector('.action-btn-details');
        if (detailsLink) {
            setTimeout(() => {
                window.location.href = detailsLink.href;
            }, 200);
        }
    }

    handleResize() {
        // Adjust grid layout if needed
        const projectsGrid = document.querySelector('.projects-grid');
        if (projectsGrid) {
            // Recalculate grid columns based on screen size
            this.adjustGridLayout();
        }
    }

    setupMobileFeatures() {
        // Add touch-friendly interactions
        const cards = document.querySelectorAll('.project-card');
        
        cards.forEach(card => {
            card.addEventListener('touchstart', () => {
                card.classList.add('touch-active');
            });
            
            card.addEventListener('touchend', () => {
                setTimeout(() => {
                    card.classList.remove('touch-active');
                }, 150);
            });
        });
    }

    animateCardsOnLoad() {
        const cards = document.querySelectorAll('.project-card');
        
        cards.forEach((card, index) => {
            card.style.opacity = '0';
            card.style.transform = 'translateY(30px)';
            
            setTimeout(() => {
                card.style.transition = 'all 0.6s ease';
                card.style.opacity = '1';
                card.style.transform = 'translateY(0)';
            }, index * 100);
        });
    }

    setupScrollAnimations() {
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('fade-in');
                }
            });
        }, {
            threshold: 0.1,
            rootMargin: '0px 0px -50px 0px'
        });

        // Observe cards that might be below the fold
        const cards = document.querySelectorAll('.project-card');
        cards.forEach(card => {
            observer.observe(card);
        });
    }

    performLiveSearch(query) {
        // This could be enhanced to perform AJAX search
        // For now, just provide visual feedback
        const searchInput = document.getElementById('search-input');
        
        if (query.length > 2) {
            searchInput.classList.add('searching');
        } else {
            searchInput.classList.remove('searching');
        }
    }

    setLoadingState(button, isLoading) {
        if (isLoading) {
            button.disabled = true;
            button.innerHTML = '<span class="loading-spinner"></span> Loading...';
        } else {
            button.disabled = false;
            button.innerHTML = 'Search';
        }
    }

    createRippleEffect(element) {
        const ripple = document.createElement('div');
        ripple.className = 'ripple-effect';
        ripple.style.cssText = `
            position: absolute;
            border-radius: 50%;
            background: rgba(59, 130, 246, 0.3);
            transform: scale(0);
            animation: ripple 0.6s linear;
            pointer-events: none;
            width: 100px;
            height: 100px;
            left: 50%;
            top: 50%;
            margin-left: -50px;
            margin-top: -50px;
        `;
        
        element.style.position = 'relative';
        element.style.overflow = 'hidden';
        element.appendChild(ripple);
        
        setTimeout(() => {
            ripple.remove();
        }, 600);
    }

    adjustGridLayout() {
        const grid = document.querySelector('.projects-grid');
        const containerWidth = grid.offsetWidth;
        const cardMinWidth = 350;
        const gap = 32; // 2rem
        
        const columns = Math.floor((containerWidth + gap) / (cardMinWidth + gap));
        grid.style.gridTemplateColumns = `repeat(${Math.max(1, columns)}, 1fr)`;
    }

    isMobile() {
        return window.innerWidth <= 768;
    }

    // Utility method for smooth transitions
    smoothTransition(element, properties, duration = 300) {
        return new Promise(resolve => {
            element.style.transition = `all ${duration}ms ease`;
            
            Object.keys(properties).forEach(prop => {
                element.style[prop] = properties[prop];
            });
            
            setTimeout(resolve, duration);
        });
    }

    // Method to show notifications (if toastr is available)
    showNotification(message, type = 'info') {
        if (typeof toastr !== 'undefined') {
            toastr[type](message);
        } else {
            console.log(`${type.toUpperCase()}: ${message}`);
        }
    }

    checkEmptyState() {
        const projectsGrid = document.querySelector('.projects-grid');
        const projectCards = document.querySelectorAll('.project-card');

        if (projectCards.length === 0 && projectsGrid) {
            this.showEmptyState(projectsGrid);
        }
    }

    showEmptyState(container) {
        const emptyState = document.createElement('div');
        emptyState.className = 'empty-state';
        emptyState.innerHTML = `
            <div class="empty-state-icon">
                <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                          d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10"></path>
                </svg>
            </div>
            <h3 class="empty-state-title">No Projects Found</h3>
            <p class="empty-state-description">
                ${this.getEmptyStateMessage()}
            </p>
        `;

        container.appendChild(emptyState);
    }

    getEmptyStateMessage() {
        const searchInput = document.getElementById('search-input');
        const hasSearch = searchInput && searchInput.value.trim().length > 0;

        if (hasSearch) {
            return 'Try adjusting your search criteria or clear the search to see all projects.';
        }

        return 'Get started by creating your first project to track manufacturing workflows.';
    }

    setupAccessibility() {
        // Add ARIA labels and roles
        const searchForm = document.getElementById('search-form');
        if (searchForm) {
            searchForm.setAttribute('role', 'search');
            searchForm.setAttribute('aria-label', 'Search and filter projects');
        }

        const projectsGrid = document.querySelector('.projects-grid');
        if (projectsGrid) {
            projectsGrid.setAttribute('role', 'grid');
            projectsGrid.setAttribute('aria-label', 'Projects list');
        }

        // Add keyboard navigation for cards
        const projectCards = document.querySelectorAll('.project-card');
        projectCards.forEach((card, index) => {
            card.setAttribute('tabindex', '0');
            card.setAttribute('role', 'gridcell');
            card.setAttribute('aria-label', `Project ${index + 1}`);

            card.addEventListener('keydown', (e) => {
                if (e.key === 'Enter' || e.key === ' ') {
                    e.preventDefault();
                    this.handleCardClick(card);
                }
            });
        });
    }
}

// CSS for ripple effect animation
const rippleCSS = `
@keyframes ripple {
    to {
        transform: scale(4);
        opacity: 0;
    }
}

.touch-active {
    transform: scale(0.98) !important;
    transition: transform 0.1s ease !important;
}

.searching {
    background-image: linear-gradient(45deg, transparent 33%, rgba(59, 130, 246, 0.1) 33%, rgba(59, 130, 246, 0.1) 66%, transparent 66%);
    background-size: 20px 20px;
    animation: searchingAnimation 1s linear infinite;
}

@keyframes searchingAnimation {
    0% { background-position: 0 0; }
    100% { background-position: 20px 20px; }
}
`;

// Inject CSS
const style = document.createElement('style');
style.textContent = rippleCSS;
document.head.appendChild(style);

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    new ProjectsPage();
});

// Export for potential use in other scripts
window.ProjectsPage = ProjectsPage;
