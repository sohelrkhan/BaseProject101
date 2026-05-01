namespace SadaqaAccounting.Repository.Repository.CashBankManagement
{
    public class CashRepository : BaseRepository<Cash>, ICashRepository
    {
        public CashRepository(DatabaseContext databaseContext) : base(databaseContext) { }

        public async Task<List<Cash>> GetByAccountUnitIdsAsync(ISet<int> accountUnitIds)
        {
            if (accountUnitIds == null || accountUnitIds.Count == 0)
                return new List<Cash>();

            return await dbContext.Cashes
                .AsNoTracking()
                .Where(c =>
                    !c.IsDeleted &&
                    accountUnitIds.Contains(c.AccountUnitId))
                .ToListAsync();
        }

        public async Task<IEnumerable<SelectModel>> GetCashSelectListByAccountUnitAsync(int accountUnitId, CancellationToken cancellationToken)
        {
            var cashes = await dbContext.Cashes
                .AsNoTracking()
                .Where(c => c.AccountUnitId == accountUnitId && !c.IsDeleted)
                .Select(s => new SelectModel 
                {
                    Id = s.Id,
                    Name = s.Name,
                    Group = s.OpeningBalances.Count() > 0 ? "Have Opening Balance" : "No Opening Balance"
                }).ToListAsync();

            return cashes;
        }
    }
}