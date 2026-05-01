namespace SadaqaAccounting.Repository.Contracts.CashBankManagement
{
    public interface ICashRepository : IBaseRepository<Cash>
    {
        Task<List<Cash>> GetByAccountUnitIdsAsync(ISet<int> accountUnitIds);
        Task<IEnumerable<SelectModel>> GetCashSelectListByAccountUnitAsync(int accountUnitId, CancellationToken cancellationToken);
    }
}