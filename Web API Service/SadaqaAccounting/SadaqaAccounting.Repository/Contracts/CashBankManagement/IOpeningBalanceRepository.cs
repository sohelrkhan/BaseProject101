namespace SadaqaAccounting.Repository.Contracts.CashBankManagement
{
    public interface IOpeningBalanceRepository : IBaseRepository<OpeningBalance>
    {
        Task<PaginatedResponse<OpeningBalance>> GetOpeningBalancesFilterAsync(PaginationRequest paginationRequest,
            CancellationToken cancellationToken);
        Task<OpeningBalance?> GetOpeningBalanceByBankAsync(int bankId, CancellationToken cancellationToken);
    }
}