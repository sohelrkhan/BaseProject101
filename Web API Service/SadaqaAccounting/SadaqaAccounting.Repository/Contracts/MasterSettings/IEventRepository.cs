namespace SadaqaAccounting.Repository.Contracts.MasterSettings
{
    public interface IEventRepository : IBaseRepository<Event>
    {
        Task<ICollection<Event>> GetEventsByAccountUnitIdAsync(int accountUnitId);
        Task<PaginatedResponse<Event>> GetEventsFilterAsync(PaginationRequest paginationRequest, CancellationToken cancellationToken = default);
        Task<IEnumerable<SelectModel>> GetEventSelectListByAccountUnitAsync(int accountUnitId, CancellationToken cancellationToken);
    }
}