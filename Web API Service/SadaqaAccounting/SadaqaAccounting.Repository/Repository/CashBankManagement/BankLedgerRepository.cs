namespace SadaqaAccounting.Repository.Repository.CashBankManagement
{
    public class BankLedgerRepository : BaseRepository<BankLedger>, IBankLedgerRepository
    {
        public BankLedgerRepository(DatabaseContext databaseContext) : base(databaseContext) { }

        public async Task<BankLedger?> GetBankLadgetByExpenseAsync(int expenseId, CancellationToken cancellationToken)
        {
            var bankLedger = await dbContext.BankLedgers
                .Where(bl => bl.SourceId == expenseId && !bl.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            return bankLedger;
        }

        public async Task<BankLedger?> GetBankLadgetByIncomeAsync(int incomeId, CancellationToken cancellationToken)
        {
            var bankLedger = await dbContext.BankLedgers
                .Where(bl => bl.SourceId == incomeId && !bl.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            return bankLedger;
        }
    }
}