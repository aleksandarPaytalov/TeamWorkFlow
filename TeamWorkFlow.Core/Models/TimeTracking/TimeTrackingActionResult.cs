using System;

namespace TeamWorkFlow.Core.Models.TimeTracking
{
    /// <summary>
    /// Standard result model for time tracking service operations
    /// </summary>
    public class TimeTrackingActionResult
    {
        /// <summary>
        /// Indicates if the operation was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Message describing the result of the operation
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Optional data payload for successful operations
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// Error details if operation failed
        /// </summary>
        public string? ErrorDetails { get; set; }

        /// <summary>
        /// Timestamp when the operation was performed
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Creates a successful result with message
        /// </summary>
        /// <param name="message">Success message</param>
        /// <returns>Successful TimeTrackingActionResult</returns>
        public static TimeTrackingActionResult CreateSuccess(string message)
        {
            return new TimeTrackingActionResult
            {
                Success = true,
                Message = message
            };
        }

        /// <summary>
        /// Creates a successful result with message and data
        /// </summary>
        /// <param name="message">Success message</param>
        /// <param name="data">Data payload</param>
        /// <returns>Successful TimeTrackingActionResult with data</returns>
        public static TimeTrackingActionResult CreateSuccess(string message, object data)
        {
            return new TimeTrackingActionResult
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// Creates a failed result with error message
        /// </summary>
        /// <param name="message">Error message</param>
        /// <returns>Failed TimeTrackingActionResult</returns>
        public static TimeTrackingActionResult CreateFailure(string message)
        {
            return new TimeTrackingActionResult
            {
                Success = false,
                Message = message
            };
        }

        /// <summary>
        /// Creates a failed result with error message and details
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="errorDetails">Detailed error information</param>
        /// <returns>Failed TimeTrackingActionResult with error details</returns>
        public static TimeTrackingActionResult CreateFailure(string message, string errorDetails)
        {
            return new TimeTrackingActionResult
            {
                Success = false,
                Message = message,
                ErrorDetails = errorDetails
            };
        }

        /// <summary>
        /// Creates a failed result from an exception
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="exception">Exception that occurred</param>
        /// <returns>Failed TimeTrackingActionResult with exception details</returns>
        public static TimeTrackingActionResult CreateFailure(string message, Exception exception)
        {
            return new TimeTrackingActionResult
            {
                Success = false,
                Message = message,
                ErrorDetails = exception.Message
            };
        }
    }

    /// <summary>
    /// Generic result model for time tracking service operations with typed data
    /// </summary>
    /// <typeparam name="T">Type of data payload</typeparam>
    public class TimeTrackingActionResult<T> : TimeTrackingActionResult
    {
        /// <summary>
        /// Strongly typed data payload
        /// </summary>
        public new T? Data { get; set; }

        /// <summary>
        /// Creates a successful result with typed data
        /// </summary>
        /// <param name="message">Success message</param>
        /// <param name="data">Typed data payload</param>
        /// <returns>Successful TimeTrackingActionResult with typed data</returns>
        public static TimeTrackingActionResult<T> CreateSuccess(string message, T data)
        {
            return new TimeTrackingActionResult<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// Creates a failed result with typed return type
        /// </summary>
        /// <param name="message">Error message</param>
        /// <returns>Failed TimeTrackingActionResult with typed return</returns>
        public static new TimeTrackingActionResult<T> CreateFailure(string message)
        {
            return new TimeTrackingActionResult<T>
            {
                Success = false,
                Message = message
            };
        }

        /// <summary>
        /// Creates a failed result with error details and typed return type
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="errorDetails">Detailed error information</param>
        /// <returns>Failed TimeTrackingActionResult with typed return and error details</returns>
        public static new TimeTrackingActionResult<T> CreateFailure(string message, string errorDetails)
        {
            return new TimeTrackingActionResult<T>
            {
                Success = false,
                Message = message,
                ErrorDetails = errorDetails
            };
        }

        /// <summary>
        /// Creates a failed result from an exception with typed return type
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="exception">Exception that occurred</param>
        /// <returns>Failed TimeTrackingActionResult with typed return and exception details</returns>
        public static new TimeTrackingActionResult<T> CreateFailure(string message, Exception exception)
        {
            return new TimeTrackingActionResult<T>
            {
                Success = false,
                Message = message,
                ErrorDetails = exception.Message
            };
        }
    }
}
