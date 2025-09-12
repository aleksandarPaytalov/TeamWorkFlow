using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamWorkFlow.Core.Models.TimeTracking;

namespace TeamWorkFlow.Core.Contracts
{
    /// <summary>
    /// Service interface for managing task time tracking operations
    /// </summary>
    public interface ITaskTimeTrackingService
    {
        // ===== WORK SESSION MANAGEMENT =====

        /// <summary>
        /// Starts a new work session for a task and operator
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <param name="operatorId">Operator identifier</param>
        /// <param name="sessionType">Type of work session (optional, defaults to "Development")</param>
        /// <returns>Result indicating success/failure with session details</returns>
        Task<TimeTrackingActionResult<TaskTimeTrackingModel>> StartWorkSessionAsync(int taskId, int operatorId, string sessionType = "Development");

        /// <summary>
        /// Pauses an active work session
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <param name="operatorId">Operator identifier</param>
        /// <returns>Result indicating success/failure with updated session details</returns>
        Task<TimeTrackingActionResult<TaskTimeTrackingModel>> PauseWorkSessionAsync(int taskId, int operatorId);

        /// <summary>
        /// Resumes a paused work session
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <param name="operatorId">Operator identifier</param>
        /// <returns>Result indicating success/failure with updated session details</returns>
        Task<TimeTrackingActionResult<TaskTimeTrackingModel>> ResumeWorkSessionAsync(int taskId, int operatorId);

        /// <summary>
        /// Finishes an active work session and creates a time entry record
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <param name="operatorId">Operator identifier</param>
        /// <param name="notes">Optional notes about the work session</param>
        /// <returns>Result indicating success/failure with final session details</returns>
        Task<TimeTrackingActionResult<TaskTimeTrackingModel>> FinishWorkSessionAsync(int taskId, int operatorId, string? notes = null);

        // ===== DATA RETRIEVAL =====

        /// <summary>
        /// Gets comprehensive time tracking information for a task and operator
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <param name="operatorId">Operator identifier</param>
        /// <returns>Complete time tracking model with current session and history</returns>
        Task<TaskTimeTrackingModel?> GetTaskTimeTrackingAsync(int taskId, int operatorId);

        /// <summary>
        /// Gets work session history for a task and operator
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <param name="operatorId">Operator identifier</param>
        /// <param name="limit">Maximum number of sessions to return (default: 10)</param>
        /// <returns>List of work sessions ordered by most recent first</returns>
        Task<List<WorkSessionModel>> GetWorkSessionHistoryAsync(int taskId, int operatorId, int limit = 10);

        /// <summary>
        /// Gets work session history for all operators on a task
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <param name="limit">Maximum number of sessions to return (default: 20)</param>
        /// <returns>List of work sessions from all operators ordered by most recent first</returns>
        Task<List<WorkSessionModel>> GetTaskWorkSessionHistoryAsync(int taskId, int limit = 20);

        /// <summary>
        /// Gets time variance analysis for a task and operator
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <param name="operatorId">Operator identifier</param>
        /// <returns>Time variance analysis model with calculations and status</returns>
        Task<TimeVarianceModel?> GetTimeVarianceAsync(int taskId, int operatorId);

        /// <summary>
        /// Gets time variance analysis for all operators on a task
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <returns>List of time variance models for each operator who worked on the task</returns>
        Task<List<TimeVarianceModel>> GetTaskTimeVarianceAsync(int taskId);

        // ===== UTILITY METHODS =====

        /// <summary>
        /// Checks if an operator has an active work session for a specific task
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <param name="operatorId">Operator identifier</param>
        /// <returns>True if active session exists, false otherwise</returns>
        Task<bool> HasActiveSessionAsync(int taskId, int operatorId);

        /// <summary>
        /// Gets all active work sessions for an operator across all tasks
        /// </summary>
        /// <param name="operatorId">Operator identifier</param>
        /// <returns>List of active sessions with task information</returns>
        Task<List<TaskTimeTrackingModel>> GetActiveSessionsForOperatorAsync(int operatorId);

        /// <summary>
        /// Gets total time spent by an operator on a specific task
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <param name="operatorId">Operator identifier</param>
        /// <returns>Total time in minutes</returns>
        Task<int> GetTotalTimeSpentAsync(int taskId, int operatorId);

        /// <summary>
        /// Gets total time spent by all operators on a specific task
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <returns>Total time in minutes from all operators</returns>
        Task<int> GetTaskTotalTimeSpentAsync(int taskId);

        /// <summary>
        /// Validates if a task exists and is available for time tracking
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <returns>True if task exists and can be tracked, false otherwise</returns>
        Task<bool> IsTaskValidForTrackingAsync(int taskId);

        /// <summary>
        /// Validates if an operator exists and is active
        /// </summary>
        /// <param name="operatorId">Operator identifier</param>
        /// <returns>True if operator exists and is active, false otherwise</returns>
        Task<bool> IsOperatorValidForTrackingAsync(int operatorId);

        /// <summary>
        /// Gets summary statistics for an operator's time tracking
        /// </summary>
        /// <param name="operatorId">Operator identifier</param>
        /// <param name="fromDate">Start date for statistics (optional)</param>
        /// <param name="toDate">End date for statistics (optional)</param>
        /// <returns>Dictionary with statistics like total time, sessions count, etc.</returns>
        Task<Dictionary<string, object>> GetOperatorTimeTrackingStatsAsync(int operatorId, DateTime? fromDate = null, DateTime? toDate = null);

        /// <summary>
        /// Gets summary statistics for a task's time tracking across all operators
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <returns>Dictionary with statistics like total time, operators count, variance, etc.</returns>
        Task<Dictionary<string, object>> GetTaskTimeTrackingStatsAsync(int taskId);
    }
}
