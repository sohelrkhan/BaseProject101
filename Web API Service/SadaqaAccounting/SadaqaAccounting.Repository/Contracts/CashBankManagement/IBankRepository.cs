namespace SadaqaAccounting.Repository.Contracts.CashBankManagement
{
    public interface IBankRepository : IBaseRepository<Bank>
    {
        Task<PaginatedResponse<Bank>> GetBanksFilterAsync(PaginationRequest paginationRequest, CancellationToken cancellationToken);
        Task<ICollection<SelectModel>> GetBankSelectListAsync();
        Task<IEnumerable<SelectModel>> GetBankSelectListByAccountUnitAsync(int accountUnitId, CancellationToken cancellationToken);
        Task<bool> HasBankAsync(string bankName); 
    }
}