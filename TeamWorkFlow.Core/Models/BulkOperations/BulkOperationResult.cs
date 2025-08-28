using System.ComponentModel.DataAnnotations;

namespace TeamWorkFlow.Core.Models.BulkOperations
{
    public class BulkOperationResult
    {
        public bool Success { get; set; }
        public int TotalItems { get; set; }
        public int ProcessedItems { get; set; }
        public int FailedItems { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
        public string? Message { get; set; }
    }

    public class BulkDeleteRequest
    {
        [Required]
        public List<int> ItemIds { get; set; } = new List<int>();
    }

    public class BulkArchiveRequest
    {
        [Required]
        public List<int> TaskIds { get; set; } = new List<int>();
    }
}
