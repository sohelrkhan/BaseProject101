namespace SadaqaAccounting.Repository.Repository.CashBankManagement
{
    public class CashLedgerRepository : BaseRepository<CashLedger>, ICashLedgerRepository
    {
        public CashLedgerRepository(DatabaseContext databaseContext) : base(databaseContext) { }

        public async Task<CashLedger?> GetCashLedgerByExpenseAsync(int expenseId, CancellationToken cancellationToken)
        {
            var cashLadger = await dbContext.CashLedgers
                .Where(cl => cl.SourceId == expenseId && !cl.IsDeleted)
                .FirstOrDefaultAsync();

            return cashLadger;
        }

        public async Task<CashLedger?> GetCashLedgerByIncomeAsync(int incomeId, CancellationToken cancellationToken)
        {
            var cashLadger = await dbContext.CashLedgers
                .Where(cl => cl.SourceId == incomeId && !cl.IsDeleted)
                .FirstOrDefaultAsync();

            return cashLadger;
        }
    }
}