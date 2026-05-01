namespace SadaqaAccounting.Repository.Contracts.CashBankManagement
{
    public interface ICashLedgerRepository : IBaseRepository<CashLedger>
    {
        Task<CashLedger?> GetCashLedgerByExpenseAsync(int expenseId, CancellationToken cancellationToken);
        Task<CashLedger?> GetCashLedgerByIncomeAsync(int incomeId, CancellationToken cancellationToken);
    }
}