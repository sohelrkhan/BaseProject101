namespace SadaqaAccounting.Repository.Contracts.CashBankManagement
{
    public interface IBankLedgerRepository : IBaseRepository<BankLedger>
    {
        Task<BankLedger?> GetBankLadgetByExpenseAsync(int expenseId, CancellationToken cancellationToken);
        Task<BankLedger?> GetBankLadgetByIncomeAsync(int incomeId, CancellationToken cancellationToken);
    }
}