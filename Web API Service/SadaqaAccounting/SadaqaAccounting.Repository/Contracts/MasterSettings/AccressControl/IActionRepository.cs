namespace SadaqaAccounting.Repository.Contracts.MasterSettings.AccressControl
{
    public interface IActionRepository : IBaseRepository<Action>
    {
        Task<Action?> GetActionByName(string name);
        Task<PaginatedResponse<Action>> GetActionsFilterAsync(PaginationRequest paginationRequest, CancellationToken cancellationToken = default);
    }
}