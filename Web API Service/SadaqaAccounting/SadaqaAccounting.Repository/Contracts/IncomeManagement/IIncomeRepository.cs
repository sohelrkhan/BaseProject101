namespace SadaqaAccounting.Repository.Contracts.IncomeManagement
{
    public interface IIncomeRepository: IBaseRepository<Income>
    {
        Task<PaginatedResponse<Income>> GetIncomesFilterAsync(PaginationRequest paginationRequest, CancellationToken cancellationToken = default);
        Task<ICollection<Income>> GetIncomeSummaryByFilterAsync(int accountUnitId,int? eventId ,DateTime fromDate, DateTime toDate);
    }
}