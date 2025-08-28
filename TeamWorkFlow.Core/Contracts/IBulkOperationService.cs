using TeamWorkFlow.Core.Models.BulkOperations;

namespace TeamWorkFlow.Core.Contracts
{
    public interface IBulkOperationService<T>
    {
        Task<BulkOperationResult> BulkDeleteAsync(List<int> itemIds);
        Task<bool> CanDeleteItemAsync(int itemId);
        Task<List<string>> ValidateItemsForDeletionAsync(List<int> itemIds);
    }

    public interface IBulkTaskOperationService : IBulkOperationService<Infrastructure.Data.Models.Task>
    {
        Task<BulkOperationResult> BulkArchiveAsync(List<int> taskIds);
        Task<bool> CanArchiveTaskAsync(int taskId);
        Task<List<string>> ValidateTasksForArchivingAsync(List<int> taskIds);
    }
}
