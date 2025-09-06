/**
 * Time Tracking Manager
 * Handles all time tracking functionality including session management,
 * real-time updates, and UI interactions
 */
class TimeTrackingManager {
    constructor() {
        this.activeTimers = new Map();
        this.refreshInterval = 30000; // 30 seconds
        this.timerInterval = 1000; // 1 second
        this.currentTaskId = null;
        this.init();
    }

    /**
     * Initialize the time tracking manager
     */
    init() {
        this.bindEvents();
        this.loadActiveSessionsOnPageLoad();
        this.startPeriodicRefresh();
        console.log('TimeTrackingManager initialized');
    }

    /**
     * Bind all event handlers
     */
    bindEvents() {
        // Start session buttons
        $(document).on('click', '[id^="start-btn-"]', (e) => {
            const taskId = $(e.target).closest('button').data('task-id');
            this.showSessionTypeModal(taskId);
        });

        // Pause session buttons
        $(document).on('click', '[id^="pause-btn-"]', (e) => {
            const taskId = $(e.target).closest('button').data('task-id');
            this.pauseSession(taskId);
        });

        // Resume session buttons
        $(document).on('click', '[id^="resume-btn-"]', (e) => {
            const taskId = $(e.target).closest('button').data('task-id');
            this.resumeSession(taskId);
        });

        // Finish session buttons
        $(document).on('click', '[id^="finish-btn-"]', (e) => {
            const taskId = $(e.target).closest('button').data('task-id');
            this.showFinishSessionModal(taskId);
        });

        // History buttons
        $(document).on('click', '.history-btn', (e) => {
            const taskId = $(e.target).closest('button').data('task-id');
            this.showSessionHistory(taskId);
        });

        // Variance buttons
        $(document).on('click', '.variance-btn', (e) => {
            const taskId = $(e.target).closest('button').data('task-id');
            this.showTimeVariance(taskId);
        });

        // Modal event handlers
        this.bindModalEvents();
    }

    /**
     * Bind modal-specific event handlers
     */
    bindModalEvents() {
        // Session type confirmation
        $('#confirmStartBtn').on('click', () => {
            const sessionType = $('input[name="sessionType"]:checked').val();
            this.startSession(this.currentTaskId, sessionType);
            $('#sessionTypeModal').modal('hide');
        });

        // Finish session confirmation
        $('#confirmFinishBtn').on('click', () => {
            const notes = $('#sessionNotes').val();
            this.finishSession(this.currentTaskId, notes);
            $('#finishSessionModal').modal('hide');
        });

        // Quick note buttons
        $(document).on('click', '.quick-note-btn', (e) => {
            const note = $(e.target).data('note');
            const currentNotes = $('#sessionNotes').val();
            const newNotes = currentNotes ? `${currentNotes}\n${note}` : note;
            $('#sessionNotes').val(newNotes);
            this.updateCharacterCount();
        });

        // Character count for notes
        $('#sessionNotes').on('input', () => {
            this.updateCharacterCount();
        });

        // Refresh history button
        $('#refreshHistoryBtn').on('click', () => {
            if (this.currentTaskId) {
                this.loadSessionHistory(this.currentTaskId);
            }
        });
    }

    /**
     * Show session type selection modal
     */
    showSessionTypeModal(taskId) {
        this.currentTaskId = taskId;
        $('#sessionTypeModal').modal('show');
    }

    /**
     * Start a new work session
     */
    async startSession(taskId, sessionType = 'Development') {
        try {
            this.showLoading(taskId, true);
            
            const response = await $.ajax({
                url: '/Task/StartTimeTracking',
                method: 'POST',
                data: {
                    taskId: taskId,
                    sessionType: sessionType,
                    __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
                }
            });

            if (response.success) {
                this.updateSessionUI(taskId, 'active', response.data);
                this.startTimer(taskId, response.data.startTime);
                this.showNotification('Session started successfully!', 'success');
            } else {
                this.showNotification(response.message || 'Failed to start session', 'error');
            }
        } catch (error) {
            console.error('Error starting session:', error);
            this.showNotification('Error starting session', 'error');
        } finally {
            this.showLoading(taskId, false);
        }
    }

    /**
     * Pause an active session
     */
    async pauseSession(taskId) {
        try {
            this.showLoading(taskId, true);
            
            const response = await $.ajax({
                url: '/Task/PauseTimeTracking',
                method: 'POST',
                data: {
                    taskId: taskId,
                    __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
                }
            });

            if (response.success) {
                this.updateSessionUI(taskId, 'paused', response.data);
                this.stopTimer(taskId);
                this.showNotification('Session paused', 'info');
            } else {
                this.showNotification(response.message || 'Failed to pause session', 'error');
            }
        } catch (error) {
            console.error('Error pausing session:', error);
            this.showNotification('Error pausing session', 'error');
        } finally {
            this.showLoading(taskId, false);
        }
    }

    /**
     * Resume a paused session
     */
    async resumeSession(taskId) {
        try {
            this.showLoading(taskId, true);
            
            const response = await $.ajax({
                url: '/Task/ResumeTimeTracking',
                method: 'POST',
                data: {
                    taskId: taskId,
                    __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
                }
            });

            if (response.success) {
                this.updateSessionUI(taskId, 'active', response.data);
                this.startTimer(taskId, response.data.resumeTime || new Date().toISOString());
                this.showNotification('Session resumed', 'success');
            } else {
                this.showNotification(response.message || 'Failed to resume session', 'error');
            }
        } catch (error) {
            console.error('Error resuming session:', error);
            this.showNotification('Error resuming session', 'error');
        } finally {
            this.showLoading(taskId, false);
        }
    }

    /**
     * Show finish session modal with current session data
     */
    async showFinishSessionModal(taskId) {
        try {
            this.currentTaskId = taskId;
            
            // Get current session data
            const response = await $.ajax({
                url: '/Task/GetTimeTracking',
                method: 'GET',
                data: { taskId: taskId }
            });

            if (response.success && response.data.hasActiveSession) {
                const sessionData = response.data.currentSession;
                
                // Update modal with session data
                $('#finishSessionDuration').text(this.formatDuration(sessionData.currentDuration));
                $('#finishSessionStartTime').text(this.formatDateTime(sessionData.startTime));
                $('#finishSessionType').text(sessionData.sessionType);
                
                // Clear previous notes
                $('#sessionNotes').val('');
                this.updateCharacterCount();
                
                $('#finishSessionModal').modal('show');
            } else {
                this.showNotification('No active session found', 'warning');
            }
        } catch (error) {
            console.error('Error loading session data:', error);
            this.showNotification('Error loading session data', 'error');
        }
    }

    /**
     * Finish the current session
     */
    async finishSession(taskId, notes = '') {
        try {
            this.showLoading(taskId, true);
            
            const response = await $.ajax({
                url: '/Task/FinishTimeTracking',
                method: 'POST',
                data: {
                    taskId: taskId,
                    notes: notes,
                    __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
                }
            });

            if (response.success) {
                this.updateSessionUI(taskId, 'inactive', response.data);
                this.stopTimer(taskId);
                this.refreshTimeTracking(taskId);
                this.showNotification('Session completed successfully!', 'success');
            } else {
                this.showNotification(response.message || 'Failed to finish session', 'error');
            }
        } catch (error) {
            console.error('Error finishing session:', error);
            this.showNotification('Error finishing session', 'error');
        } finally {
            this.showLoading(taskId, false);
        }
    }

    /**
     * Update character count for notes textarea
     */
    updateCharacterCount() {
        const notes = $('#sessionNotes').val();
        const count = notes.length;
        $('#notesCharCount').text(count);
        
        // Update styling based on character limit
        if (count > 450) {
            $('#notesCharCount').addClass('text-warning');
        } else {
            $('#notesCharCount').removeClass('text-warning');
        }
    }

    /**
     * Start timer for active session
     */
    startTimer(taskId, startTime) {
        this.stopTimer(taskId); // Clear any existing timer
        
        const timer = setInterval(() => {
            const elapsed = this.calculateElapsed(startTime);
            $(`#session-timer-${taskId}`).text(this.formatDuration(elapsed));
        }, this.timerInterval);
        
        this.activeTimers.set(taskId, timer);
    }

    /**
     * Stop timer for task
     */
    stopTimer(taskId) {
        if (this.activeTimers.has(taskId)) {
            clearInterval(this.activeTimers.get(taskId));
            this.activeTimers.delete(taskId);
        }
    }

    /**
     * Calculate elapsed time from start time
     */
    calculateElapsed(startTime) {
        const start = new Date(startTime);
        const now = new Date();
        return Math.floor((now - start) / 1000 / 60); // minutes
    }

    /**
     * Format duration in minutes to HH:MM:SS
     */
    formatDuration(minutes) {
        const hours = Math.floor(minutes / 60);
        const mins = minutes % 60;
        const secs = 0; // For display purposes, we'll show 0 seconds
        return `${hours.toString().padStart(2, '0')}:${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
    }

    /**
     * Format date time for display
     */
    formatDateTime(dateTimeString) {
        const date = new Date(dateTimeString);
        return date.toLocaleString();
    }

    /**
     * Update session UI based on state
     */
    updateSessionUI(taskId, state, data = null) {
        const widget = $(`.time-tracking-widget[data-task-id="${taskId}"]`);
        const statusElement = $(`#tracking-status-${taskId}`);
        const currentSession = $(`#current-session-${taskId}`);
        
        // Update status indicator
        const statusIndicator = statusElement.find('.status-indicator');
        const statusText = statusElement.find('.status-text');
        
        statusIndicator.removeClass('status-inactive status-active status-paused');
        
        // Show/hide buttons based on state
        const startBtn = $(`#start-btn-${taskId}`);
        const pauseBtn = $(`#pause-btn-${taskId}`);
        const resumeBtn = $(`#resume-btn-${taskId}`);
        const finishBtn = $(`#finish-btn-${taskId}`);
        
        switch (state) {
            case 'active':
                statusIndicator.addClass('status-active');
                statusText.text('Active');
                currentSession.show();
                startBtn.hide();
                pauseBtn.show();
                resumeBtn.hide();
                finishBtn.show();
                
                if (data && data.sessionType) {
                    $(`#session-type-${taskId}`).text(data.sessionType);
                }
                break;
                
            case 'paused':
                statusIndicator.addClass('status-paused');
                statusText.text('Paused');
                currentSession.show();
                startBtn.hide();
                pauseBtn.hide();
                resumeBtn.show();
                finishBtn.show();
                break;
                
            case 'inactive':
            default:
                statusIndicator.addClass('status-inactive');
                statusText.text('Not Started');
                currentSession.hide();
                startBtn.show();
                pauseBtn.hide();
                resumeBtn.hide();
                finishBtn.hide();
                break;
        }
    }

    /**
     * Show session history modal
     */
    async showSessionHistory(taskId) {
        this.currentTaskId = taskId;
        $('#sessionHistoryModal').modal('show');
        await this.loadSessionHistory(taskId);
    }

    /**
     * Load session history data
     */
    async loadSessionHistory(taskId) {
        try {
            $('#historyLoading').show();
            $('#historyError').hide();
            $('#historyContent').hide();

            const response = await $.ajax({
                url: '/Task/GetWorkSessionHistory',
                method: 'GET',
                data: { taskId: taskId }
            });

            if (response.success) {
                this.populateSessionHistory(response.data);
                $('#historyContent').show();
            } else {
                this.showHistoryError(response.message || 'Failed to load session history');
            }
        } catch (error) {
            console.error('Error loading session history:', error);
            this.showHistoryError('Error loading session history');
        } finally {
            $('#historyLoading').hide();
        }
    }

    /**
     * Populate session history table
     */
    populateSessionHistory(data) {
        const tbody = $('#sessionHistoryTableBody');
        tbody.empty();

        // Update header info
        $('#historyTaskName').text(data.taskName || 'Task');
        $('#historyTotalSessions').text(`${data.sessions.length} Sessions`);
        $('#historyTotalTime').text(this.formatDurationHours(data.totalMinutes));

        if (data.sessions.length === 0) {
            $('#historyEmptyState').show();
            $('table').hide();
            return;
        }

        $('#historyEmptyState').hide();
        $('table').show();

        data.sessions.forEach(session => {
            const row = $(`
                <tr>
                    <td>
                        <div class="d-flex align-items-center">
                            <div class="operator-avatar-small me-2">
                                ${this.getOperatorInitials(session.operatorName)}
                            </div>
                            ${session.operatorName}
                        </div>
                    </td>
                    <td>${this.formatDate(session.startTime)}</td>
                    <td>
                        <span class="badge bg-primary">
                            ${this.formatDurationHours(session.durationMinutes)}
                        </span>
                    </td>
                    <td>
                        <span class="badge bg-secondary">
                            ${session.sessionType}
                        </span>
                    </td>
                    <td>
                        <small class="text-muted">
                            ${session.notes || 'No notes'}
                        </small>
                    </td>
                </tr>
            `);
            tbody.append(row);
        });
    }

    /**
     * Show time variance modal
     */
    async showTimeVariance(taskId) {
        this.currentTaskId = taskId;
        $('#timeVarianceModal').modal('show');
        await this.loadTimeVariance(taskId);
    }

    /**
     * Load time variance data
     */
    async loadTimeVariance(taskId) {
        try {
            $('#varianceLoading').show();
            $('#varianceError').hide();
            $('#varianceContent').hide();

            const response = await $.ajax({
                url: '/Task/GetTimeVariance',
                method: 'GET',
                data: { taskId: taskId }
            });

            if (response.success) {
                this.populateTimeVariance(response.data);
                $('#varianceContent').show();
            } else {
                this.showVarianceError(response.message || 'Failed to load variance data');
            }
        } catch (error) {
            console.error('Error loading variance data:', error);
            this.showVarianceError('Error loading variance data');
        } finally {
            $('#varianceLoading').hide();
        }
    }

    /**
     * Populate time variance analysis
     */
    populateTimeVariance(data) {
        if (!data.hasData) {
            $('#varianceEmptyState').show();
            $('.variance-card').hide();
            $('.variance-analysis').hide();
            return;
        }

        $('#varianceEmptyState').hide();
        $('.variance-card').show();
        $('.variance-analysis').show();

        // Update variance cards
        $('#varianceEstimatedTime').text(`${data.estimatedHours}h`);
        $('#varianceActualTime').text(this.formatDurationHours(data.actualMinutes));
        $('#varianceDifference').text(`${data.variancePercentage > 0 ? '+' : ''}${data.variancePercentage}%`);

        // Update variance card styling
        const diffCard = $('#varianceDifferenceCard');
        diffCard.removeClass('variance-over variance-under variance-on-track');

        if (data.variancePercentage > 10) {
            diffCard.addClass('variance-over');
        } else if (data.variancePercentage < -10) {
            diffCard.addClass('variance-under');
        } else {
            diffCard.addClass('variance-on-track');
        }

        // Update analysis
        this.updateVarianceAnalysis(data);
    }

    /**
     * Update variance analysis text
     */
    updateVarianceAnalysis(data) {
        const indicator = $('#varianceIndicator');
        const analysisText = $('#varianceAnalysisText');

        indicator.removeClass('analysis-good analysis-warning analysis-danger');

        let analysis = '';
        let indicatorClass = '';

        if (data.variancePercentage > 25) {
            indicatorClass = 'analysis-danger';
            analysis = `Task is significantly over estimate by ${data.variancePercentage}%. Consider reviewing task complexity or breaking it into smaller tasks.`;
        } else if (data.variancePercentage > 10) {
            indicatorClass = 'analysis-warning';
            analysis = `Task is over estimate by ${data.variancePercentage}%. This is within acceptable range but worth monitoring.`;
        } else if (data.variancePercentage < -25) {
            indicatorClass = 'analysis-warning';
            analysis = `Task completed much faster than estimated (${Math.abs(data.variancePercentage)}% under). Consider if estimate was too conservative.`;
        } else if (data.variancePercentage < -10) {
            indicatorClass = 'analysis-good';
            analysis = `Task completed faster than estimated (${Math.abs(data.variancePercentage)}% under). Good efficiency!`;
        } else {
            indicatorClass = 'analysis-good';
            analysis = `Task time is very close to estimate (${Math.abs(data.variancePercentage)}% variance). Excellent estimation accuracy!`;
        }

        indicator.addClass(indicatorClass);
        analysisText.text(analysis);
    }

    /**
     * Refresh time tracking data for a task
     */
    async refreshTimeTracking(taskId) {
        try {
            const response = await $.ajax({
                url: '/Task/GetTimeTracking',
                method: 'GET',
                data: { taskId: taskId }
            });

            if (response.success) {
                this.updateProgressDisplay(taskId, response.data);

                if (response.data.hasActiveSession) {
                    const sessionState = response.data.currentSession.isPaused ? 'paused' : 'active';
                    this.updateSessionUI(taskId, sessionState, response.data.currentSession);

                    if (!response.data.currentSession.isPaused) {
                        this.startTimer(taskId, response.data.currentSession.startTime);
                    }
                } else {
                    this.updateSessionUI(taskId, 'inactive');
                }
            }
        } catch (error) {
            console.error('Error refreshing time tracking:', error);
        }
    }

    /**
     * Update progress display
     */
    updateProgressDisplay(taskId, data) {
        const progressPercentage = $(`#progress-percentage-${taskId}`);
        const actualTime = $(`#actual-time-${taskId}`);
        const progressBar = $(`#progress-bar-${taskId} .progress-fill`);

        progressPercentage.text(`${data.progressPercentage}%`);
        actualTime.text(this.formatDurationHours(data.totalActualMinutes));
        progressBar.css('width', `${Math.min(data.progressPercentage, 100)}%`);

        // Update progress bar color based on percentage
        progressBar.removeClass('progress-normal progress-warning progress-danger');
        if (data.progressPercentage > 100) {
            progressBar.addClass('progress-danger');
        } else if (data.progressPercentage > 80) {
            progressBar.addClass('progress-warning');
        } else {
            progressBar.addClass('progress-normal');
        }
    }

    /**
     * Load active sessions on page load
     */
    async loadActiveSessionsOnPageLoad() {
        $('.time-tracking-widget').each(async (_, element) => {
            const taskId = $(element).data('task-id');
            if (taskId) {
                await this.refreshTimeTracking(taskId);
            }
        });
    }

    /**
     * Start periodic refresh of time tracking data
     */
    startPeriodicRefresh() {
        setInterval(() => {
            $('.time-tracking-widget').each((_, element) => {
                const taskId = $(element).data('task-id');
                if (taskId) {
                    this.refreshTimeTracking(taskId);
                }
            });
        }, this.refreshInterval);
    }

    /**
     * Format duration in minutes to hours and minutes (e.g., "2h 30m")
     */
    formatDurationHours(minutes) {
        const hours = Math.floor(minutes / 60);
        const mins = minutes % 60;

        if (hours === 0) {
            return `${mins}m`;
        } else if (mins === 0) {
            return `${hours}h`;
        } else {
            return `${hours}h ${mins}m`;
        }
    }

    /**
     * Format date for display
     */
    formatDate(dateString) {
        const date = new Date(dateString);
        return date.toLocaleDateString() + ' ' + date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    }

    /**
     * Get operator initials for avatar
     */
    getOperatorInitials(name) {
        return name.split(' ').map(n => n[0]).join('').toUpperCase().substring(0, 2);
    }

    /**
     * Show loading state
     */
    showLoading(taskId, show) {
        const loadingElement = $(`#loading-${taskId}`);
        if (show) {
            loadingElement.show();
        } else {
            loadingElement.hide();
        }
    }

    /**
     * Show notification to user
     */
    showNotification(message, type = 'info') {
        // Create notification element
        const notification = $(`
            <div class="alert alert-${type === 'error' ? 'danger' : type} alert-dismissible fade show time-tracking-notification" role="alert">
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            </div>
        `);

        // Add to page
        $('body').append(notification);

        // Auto-dismiss after 5 seconds
        setTimeout(() => {
            notification.alert('close');
        }, 5000);
    }

    /**
     * Show history error
     */
    showHistoryError(message) {
        $('#historyErrorMessage').text(message);
        $('#historyError').show();
        $('#historyContent').hide();
    }

    /**
     * Show variance error
     */
    showVarianceError(message) {
        $('#varianceErrorMessage').text(message);
        $('#varianceError').show();
        $('#varianceContent').hide();
    }
}

// Initialize time tracking when document is ready
$(document).ready(function() {
    // Only initialize if we have time tracking widgets on the page
    if ($('.time-tracking-widget').length > 0) {
        window.timeTrackingManager = new TimeTrackingManager();
    }
});

// Export for global access
window.TimeTrackingManager = TimeTrackingManager;
