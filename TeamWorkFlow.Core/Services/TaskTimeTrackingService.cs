using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.TimeTracking;
using TeamWorkFlow.Infrastructure.Common;
using TeamWorkFlow.Infrastructure.Data.Models;
using Task = System.Threading.Tasks.Task;

namespace TeamWorkFlow.Core.Services
{
    /// <summary>
    /// Service implementation for managing task time tracking operations
    /// </summary>
    public class TaskTimeTrackingService : ITaskTimeTrackingService
    {
        private readonly IRepository _repository;
        private readonly ILogger<TaskTimeTrackingService> _logger;

        public TaskTimeTrackingService(IRepository repository, ILogger<TaskTimeTrackingService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // ===== WORK SESSION MANAGEMENT =====

        public async Task<TimeTrackingActionResult<TaskTimeTrackingModel>> StartWorkSessionAsync(int taskId, int operatorId, string sessionType = "Development")
        {
            try
            {
                _logger.LogInformation("Starting work session for Task {TaskId}, Operator {OperatorId}", taskId, operatorId);

                // Validate task and operator
                if (!await IsTaskValidForTrackingAsync(taskId))
                {
                    return TimeTrackingActionResult<TaskTimeTrackingModel>.CreateFailure("Task not found or not available for time tracking.");
                }

                if (!await IsOperatorValidForTrackingAsync(operatorId))
                {
                    return TimeTrackingActionResult<TaskTimeTrackingModel>.CreateFailure("Operator not found or not active.");
                }

                // Check if operator already has an active session for this task
                if (await HasActiveSessionAsync(taskId, operatorId))
                {
                    return TimeTrackingActionResult<TaskTimeTrackingModel>.CreateFailure("An active work session already exists for this task and operator.");
                }

                // Create new session
                var session = new TaskTimeSession
                {
                    TaskId = taskId,
                    OperatorId = operatorId,
                    StartTime = DateTime.UtcNow,
                    TotalPausedMinutes = 0,
                    IsPaused = false,
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _repository.AddAsync(session);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Work session started successfully. Session ID: {SessionId}", session.Id);

                // Get updated tracking model
                var trackingModel = await GetTaskTimeTrackingAsync(taskId, operatorId);
                return TimeTrackingActionResult<TaskTimeTrackingModel>.CreateSuccess("Work session started successfully.", trackingModel!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting work session for Task {TaskId}, Operator {OperatorId}", taskId, operatorId);
                return TimeTrackingActionResult<TaskTimeTrackingModel>.CreateFailure("An error occurred while starting the work session.", ex);
            }
        }

        public async Task<TimeTrackingActionResult<TaskTimeTrackingModel>> PauseWorkSessionAsync(int taskId, int operatorId)
        {
            try
            {
                _logger.LogInformation("Pausing work session for Task {TaskId}, Operator {OperatorId}", taskId, operatorId);

                var session = await _repository.All<TaskTimeSession>()
                    .FirstOrDefaultAsync(s => s.TaskId == taskId && s.OperatorId == operatorId);

                if (session == null)
                {
                    return TimeTrackingActionResult<TaskTimeTrackingModel>.CreateFailure("No active work session found for this task and operator.");
                }

                if (session.IsPaused)
                {
                    return TimeTrackingActionResult<TaskTimeTrackingModel>.CreateFailure("Work session is already paused.");
                }

                // Update session to paused state
                session.IsPaused = true;
                session.LastPauseTime = DateTime.UtcNow;
                session.Status = "Paused";
                session.UpdatedAt = DateTime.UtcNow;

                await _repository.SaveChangesAsync();

                _logger.LogInformation("Work session paused successfully. Session ID: {SessionId}", session.Id);

                // Get updated tracking model
                var trackingModel = await GetTaskTimeTrackingAsync(taskId, operatorId);
                return TimeTrackingActionResult<TaskTimeTrackingModel>.CreateSuccess("Work session paused successfully.", trackingModel!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error pausing work session for Task {TaskId}, Operator {OperatorId}", taskId, operatorId);
                return TimeTrackingActionResult<TaskTimeTrackingModel>.CreateFailure("An error occurred while pausing the work session.", ex);
            }
        }

        public async Task<TimeTrackingActionResult<TaskTimeTrackingModel>> ResumeWorkSessionAsync(int taskId, int operatorId)
        {
            try
            {
                _logger.LogInformation("Resuming work session for Task {TaskId}, Operator {OperatorId}", taskId, operatorId);

                var session = await _repository.All<TaskTimeSession>()
                    .FirstOrDefaultAsync(s => s.TaskId == taskId && s.OperatorId == operatorId);

                if (session == null)
                {
                    return TimeTrackingActionResult<TaskTimeTrackingModel>.CreateFailure("No active work session found for this task and operator.");
                }

                if (!session.IsPaused)
                {
                    return TimeTrackingActionResult<TaskTimeTrackingModel>.CreateFailure("Work session is not currently paused.");
                }

                // Calculate paused duration and update session
                if (session.LastPauseTime.HasValue)
                {
                    var pausedDuration = (int)(DateTime.UtcNow - session.LastPauseTime.Value).TotalMinutes;
                    session.TotalPausedMinutes += pausedDuration;
                }

                session.IsPaused = false;
                session.LastPauseTime = null;
                session.Status = "Active";
                session.UpdatedAt = DateTime.UtcNow;

                await _repository.SaveChangesAsync();

                _logger.LogInformation("Work session resumed successfully. Session ID: {SessionId}", session.Id);

                // Get updated tracking model
                var trackingModel = await GetTaskTimeTrackingAsync(taskId, operatorId);
                return TimeTrackingActionResult<TaskTimeTrackingModel>.CreateSuccess("Work session resumed successfully.", trackingModel!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resuming work session for Task {TaskId}, Operator {OperatorId}", taskId, operatorId);
                return TimeTrackingActionResult<TaskTimeTrackingModel>.CreateFailure("An error occurred while resuming the work session.", ex);
            }
        }

        public async Task<TimeTrackingActionResult<TaskTimeTrackingModel>> FinishWorkSessionAsync(int taskId, int operatorId, string? notes = null)
        {
            try
            {
                _logger.LogInformation("Finishing work session for Task {TaskId}, Operator {OperatorId}", taskId, operatorId);

                var session = await _repository.All<TaskTimeSession>()
                    .FirstOrDefaultAsync(s => s.TaskId == taskId && s.OperatorId == operatorId);

                if (session == null)
                {
                    return TimeTrackingActionResult<TaskTimeTrackingModel>.CreateFailure("No active work session found for this task and operator.");
                }

                var endTime = DateTime.UtcNow;

                // Calculate final paused time if session is currently paused
                if (session.IsPaused && session.LastPauseTime.HasValue)
                {
                    var finalPausedDuration = (int)(endTime - session.LastPauseTime.Value).TotalMinutes;
                    session.TotalPausedMinutes += finalPausedDuration;
                }

                // Calculate total session duration
                var totalDuration = (int)(endTime - session.StartTime).TotalMinutes - session.TotalPausedMinutes;
                totalDuration = Math.Max(0, totalDuration); // Ensure non-negative

                // Create time entry record
                var timeEntry = new TaskTimeEntry
                {
                    TaskId = taskId,
                    OperatorId = operatorId,
                    StartTime = session.StartTime,
                    EndTime = endTime,
                    DurationMinutes = totalDuration,
                    Notes = notes,
                    SessionType = "Development", // Default session type
                    CreatedAt = DateTime.UtcNow
                };

                await _repository.AddAsync(timeEntry);

                // Remove the active session
                await _repository.DeleteAsync<TaskTimeSession>(session.Id);

                await _repository.SaveChangesAsync();

                _logger.LogInformation("Work session finished successfully. Duration: {Duration} minutes", totalDuration);

                // Get updated tracking model
                var trackingModel = await GetTaskTimeTrackingAsync(taskId, operatorId);
                return TimeTrackingActionResult<TaskTimeTrackingModel>.CreateSuccess($"Work session completed. Duration: {totalDuration} minutes.", trackingModel!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finishing work session for Task {TaskId}, Operator {OperatorId}", taskId, operatorId);
                return TimeTrackingActionResult<TaskTimeTrackingModel>.CreateFailure("An error occurred while finishing the work session.", ex);
            }
        }

        // ===== DATA RETRIEVAL =====

        public async Task<TaskTimeTrackingModel?> GetTaskTimeTrackingAsync(int taskId, int operatorId)
        {
            try
            {
                var task = await _repository.AllReadOnly<Infrastructure.Data.Models.Task>()
                    .Include(t => t.Project)
                    .FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null) return null;

                var operatorEntity = await _repository.AllReadOnly<Operator>()
                    .FirstOrDefaultAsync(o => o.Id == operatorId);

                if (operatorEntity == null) return null;

                // Get active session
                var activeSession = await _repository.AllReadOnly<TaskTimeSession>()
                    .FirstOrDefaultAsync(s => s.TaskId == taskId && s.OperatorId == operatorId);

                // Get completed sessions
                var completedSessions = await _repository.AllReadOnly<TaskTimeEntry>()
                    .Where(e => e.TaskId == taskId && e.OperatorId == operatorId)
                    .OrderByDescending(e => e.CreatedAt)
                    .Take(5)
                    .ToListAsync();

                // Calculate totals
                var totalActualMinutes = completedSessions.Sum(s => s.DurationMinutes);
                var totalSessions = completedSessions.Count;

                // Map to view model
                var model = new TaskTimeTrackingModel
                {
                    TaskId = taskId,
                    TaskName = task.Name,
                    TaskDescription = task.Description,
                    EstimatedTimeHours = task.EstimatedTime,
                    OperatorId = operatorId,
                    OperatorName = operatorEntity.FullName,
                    OperatorEmail = operatorEntity.Email,
                    HasActiveSession = activeSession != null,
                    CurrentSessionStartTime = activeSession?.StartTime,
                    IsCurrentSessionPaused = activeSession?.IsPaused ?? false,
                    CurrentSessionPausedMinutes = activeSession?.TotalPausedMinutes ?? 0,
                    CurrentSessionStatus = activeSession?.Status ?? string.Empty,
                    TotalActualTimeMinutes = totalActualMinutes,
                    TotalCompletedSessions = totalSessions,
                    CompletionPercentage = task.EstimatedTime > 0 ? Math.Min(100, (totalActualMinutes / (task.EstimatedTime * 60m)) * 100) : 0,
                    RecentSessions = completedSessions.Select(s => new WorkSessionModel
                    {
                        Id = s.Id,
                        TaskId = s.TaskId,
                        TaskName = task.Name,
                        OperatorId = s.OperatorId,
                        OperatorName = operatorEntity.FullName,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        DurationMinutes = s.DurationMinutes,
                        Notes = s.Notes ?? string.Empty,
                        SessionType = s.SessionType,
                        CreatedAt = s.CreatedAt
                    }).ToList()
                };

                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task time tracking for Task {TaskId}, Operator {OperatorId}", taskId, operatorId);
                return null;
            }
        }

        public async Task<List<WorkSessionModel>> GetWorkSessionHistoryAsync(int taskId, int operatorId, int limit = 10)
        {
            try
            {
                var sessions = await _repository.AllReadOnly<TaskTimeEntry>()
                    .Include(e => e.Task)
                    .Include(e => e.Operator)
                    .Where(e => e.TaskId == taskId && e.OperatorId == operatorId)
                    .OrderByDescending(e => e.CreatedAt)
                    .Take(limit)
                    .Select(e => new WorkSessionModel
                    {
                        Id = e.Id,
                        TaskId = e.TaskId,
                        TaskName = e.Task.Name,
                        OperatorId = e.OperatorId,
                        OperatorName = e.Operator.FullName,
                        StartTime = e.StartTime,
                        EndTime = e.EndTime,
                        DurationMinutes = e.DurationMinutes,
                        Notes = e.Notes ?? string.Empty,
                        SessionType = e.SessionType,
                        CreatedAt = e.CreatedAt
                    })
                    .ToListAsync();

                return sessions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting work session history for Task {TaskId}, Operator {OperatorId}", taskId, operatorId);
                return new List<WorkSessionModel>();
            }
        }

        public async Task<List<WorkSessionModel>> GetTaskWorkSessionHistoryAsync(int taskId, int limit = 20)
        {
            try
            {
                var sessions = await _repository.AllReadOnly<TaskTimeEntry>()
                    .Include(e => e.Task)
                    .Include(e => e.Operator)
                    .Where(e => e.TaskId == taskId)
                    .OrderByDescending(e => e.CreatedAt)
                    .Take(limit)
                    .Select(e => new WorkSessionModel
                    {
                        Id = e.Id,
                        TaskId = e.TaskId,
                        TaskName = e.Task.Name,
                        OperatorId = e.OperatorId,
                        OperatorName = e.Operator.FullName,
                        StartTime = e.StartTime,
                        EndTime = e.EndTime,
                        DurationMinutes = e.DurationMinutes,
                        Notes = e.Notes ?? string.Empty,
                        SessionType = e.SessionType,
                        CreatedAt = e.CreatedAt
                    })
                    .ToListAsync();

                return sessions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task work session history for Task {TaskId}", taskId);
                return new List<WorkSessionModel>();
            }
        }

        public async Task<TimeVarianceModel?> GetTimeVarianceAsync(int taskId, int operatorId)
        {
            try
            {
                var task = await _repository.AllReadOnly<Infrastructure.Data.Models.Task>()
                    .FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null) return null;

                var operatorEntity = await _repository.AllReadOnly<Operator>()
                    .FirstOrDefaultAsync(o => o.Id == operatorId);

                if (operatorEntity == null) return null;

                var sessions = await _repository.AllReadOnly<TaskTimeEntry>()
                    .Where(e => e.TaskId == taskId && e.OperatorId == operatorId)
                    .ToListAsync();

                var totalActualMinutes = sessions.Sum(s => s.DurationMinutes);
                var estimatedMinutes = (int)(task.EstimatedTime * 60);
                var varianceMinutes = totalActualMinutes - estimatedMinutes;
                var variancePercentage = estimatedMinutes > 0 ? (decimal)varianceMinutes / estimatedMinutes * 100 : 0;

                var model = new TimeVarianceModel
                {
                    TaskId = taskId,
                    TaskName = task.Name,
                    OperatorId = operatorId,
                    OperatorName = operatorEntity.FullName,
                    EstimatedTimeHours = task.EstimatedTime,
                    ActualTimeMinutes = totalActualMinutes,
                    VarianceMinutes = varianceMinutes,
                    VariancePercentage = variancePercentage,
                    TotalSessions = sessions.Count,
                    AverageSessionMinutes = sessions.Count > 0 ? (decimal)totalActualMinutes / sessions.Count : 0,
                    TaskStartDate = sessions.Count > 0 ? sessions.Min(s => s.StartTime) : null,
                    TaskCompletedDate = task.EndDate
                };

                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting time variance for Task {TaskId}, Operator {OperatorId}", taskId, operatorId);
                return null;
            }
        }

        public async Task<List<TimeVarianceModel>> GetTaskTimeVarianceAsync(int taskId)
        {
            try
            {
                var task = await _repository.AllReadOnly<Infrastructure.Data.Models.Task>()
                    .FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null) return new List<TimeVarianceModel>();

                var operatorSessions = await _repository.AllReadOnly<TaskTimeEntry>()
                    .Include(e => e.Operator)
                    .Where(e => e.TaskId == taskId)
                    .GroupBy(e => e.OperatorId)
                    .ToListAsync();

                var variances = new List<TimeVarianceModel>();

                foreach (var group in operatorSessions)
                {
                    var sessions = group.ToList();
                    var operatorEntity = sessions.First().Operator;
                    var totalActualMinutes = sessions.Sum(s => s.DurationMinutes);
                    var estimatedMinutes = (int)(task.EstimatedTime * 60);
                    var varianceMinutes = totalActualMinutes - estimatedMinutes;
                    var variancePercentage = estimatedMinutes > 0 ? (decimal)varianceMinutes / estimatedMinutes * 100 : 0;

                    var model = new TimeVarianceModel
                    {
                        TaskId = taskId,
                        TaskName = task.Name,
                        OperatorId = group.Key,
                        OperatorName = operatorEntity.FullName,
                        EstimatedTimeHours = task.EstimatedTime,
                        ActualTimeMinutes = totalActualMinutes,
                        VarianceMinutes = varianceMinutes,
                        VariancePercentage = variancePercentage,
                        TotalSessions = sessions.Count,
                        AverageSessionMinutes = sessions.Count > 0 ? (decimal)totalActualMinutes / sessions.Count : 0,
                        TaskStartDate = sessions.Min(s => s.StartTime),
                        TaskCompletedDate = task.EndDate
                    };

                    variances.Add(model);
                }

                return variances.OrderBy(v => v.OperatorName).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task time variance for Task {TaskId}", taskId);
                return new List<TimeVarianceModel>();
            }
        }

        // ===== UTILITY METHODS =====

        public async Task<bool> HasActiveSessionAsync(int taskId, int operatorId)
        {
            try
            {
                return await _repository.AllReadOnly<TaskTimeSession>()
                    .AnyAsync(s => s.TaskId == taskId && s.OperatorId == operatorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking active session for Task {TaskId}, Operator {OperatorId}", taskId, operatorId);
                return false;
            }
        }

        public async Task<List<TaskTimeTrackingModel>> GetActiveSessionsForOperatorAsync(int operatorId)
        {
            try
            {
                var activeSessions = await _repository.AllReadOnly<TaskTimeSession>()
                    .Include(s => s.Task)
                    .Include(s => s.Operator)
                    .Where(s => s.OperatorId == operatorId)
                    .ToListAsync();

                var trackingModels = new List<TaskTimeTrackingModel>();

                foreach (var session in activeSessions)
                {
                    var model = await GetTaskTimeTrackingAsync(session.TaskId, operatorId);
                    if (model != null)
                    {
                        trackingModels.Add(model);
                    }
                }

                return trackingModels;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active sessions for Operator {OperatorId}", operatorId);
                return new List<TaskTimeTrackingModel>();
            }
        }

        public async Task<int> GetTotalTimeSpentAsync(int taskId, int operatorId)
        {
            try
            {
                return await _repository.AllReadOnly<TaskTimeEntry>()
                    .Where(e => e.TaskId == taskId && e.OperatorId == operatorId)
                    .SumAsync(e => e.DurationMinutes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total time spent for Task {TaskId}, Operator {OperatorId}", taskId, operatorId);
                return 0;
            }
        }

        public async Task<int> GetTaskTotalTimeSpentAsync(int taskId)
        {
            try
            {
                return await _repository.AllReadOnly<TaskTimeEntry>()
                    .Where(e => e.TaskId == taskId)
                    .SumAsync(e => e.DurationMinutes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task total time spent for Task {TaskId}", taskId);
                return 0;
            }
        }

        public async Task<bool> IsTaskValidForTrackingAsync(int taskId)
        {
            try
            {
                return await _repository.AllReadOnly<Infrastructure.Data.Models.Task>()
                    .AnyAsync(t => t.Id == taskId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating task for tracking. Task {TaskId}", taskId);
                return false;
            }
        }

        public async Task<bool> IsOperatorValidForTrackingAsync(int operatorId)
        {
            try
            {
                return await _repository.AllReadOnly<Operator>()
                    .AnyAsync(o => o.Id == operatorId && o.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating operator for tracking. Operator {OperatorId}", operatorId);
                return false;
            }
        }

        public async Task<Dictionary<string, object>> GetOperatorTimeTrackingStatsAsync(int operatorId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var query = _repository.AllReadOnly<TaskTimeEntry>()
                    .Where(e => e.OperatorId == operatorId);

                if (fromDate.HasValue)
                    query = query.Where(e => e.CreatedAt >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(e => e.CreatedAt <= toDate.Value);

                var entries = await query.ToListAsync();

                var stats = new Dictionary<string, object>
                {
                    ["TotalTimeMinutes"] = entries.Sum(e => e.DurationMinutes),
                    ["TotalSessions"] = entries.Count,
                    ["AverageSessionMinutes"] = entries.Count > 0 ? entries.Average(e => e.DurationMinutes) : 0,
                    ["UniqueTasks"] = entries.Select(e => e.TaskId).Distinct().Count(),
                    ["TotalDays"] = entries.Select(e => e.CreatedAt.Date).Distinct().Count(),
                    ["FirstSession"] = entries.Count > 0 ? (object)entries.Min(e => e.CreatedAt) : (object?)null,
                    ["LastSession"] = entries.Count > 0 ? (object)entries.Max(e => e.CreatedAt) : (object?)null
                };

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting operator time tracking stats for Operator {OperatorId}", operatorId);
                return new Dictionary<string, object>();
            }
        }

        public async Task<Dictionary<string, object>> GetTaskTimeTrackingStatsAsync(int taskId)
        {
            try
            {
                var entries = await _repository.AllReadOnly<TaskTimeEntry>()
                    .Where(e => e.TaskId == taskId)
                    .ToListAsync();

                var task = await _repository.AllReadOnly<Infrastructure.Data.Models.Task>()
                    .FirstOrDefaultAsync(t => t.Id == taskId);

                var totalActualMinutes = entries.Sum(e => e.DurationMinutes);
                var estimatedMinutes = task?.EstimatedTime * 60 ?? 0;

                var stats = new Dictionary<string, object>
                {
                    ["TotalTimeMinutes"] = totalActualMinutes,
                    ["EstimatedTimeMinutes"] = estimatedMinutes,
                    ["VarianceMinutes"] = totalActualMinutes - estimatedMinutes,
                    ["VariancePercentage"] = estimatedMinutes > 0 ? (decimal)((totalActualMinutes - estimatedMinutes) * 100) / estimatedMinutes : 0,
                    ["TotalSessions"] = entries.Count,
                    ["UniqueOperators"] = entries.Select(e => e.OperatorId).Distinct().Count(),
                    ["AverageSessionMinutes"] = entries.Count > 0 ? entries.Average(e => e.DurationMinutes) : 0,
                    ["FirstSession"] = entries.Count > 0 ? (object)entries.Min(e => e.CreatedAt) : (object?)null,
                    ["LastSession"] = entries.Count > 0 ? (object)entries.Max(e => e.CreatedAt) : (object?)null,
                    ["CompletionPercentage"] = estimatedMinutes > 0 ? Math.Min(100, (decimal)totalActualMinutes / estimatedMinutes * 100) : 0
                };

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task time tracking stats for Task {TaskId}", taskId);
                return new Dictionary<string, object>();
            }
        }
    }
}
