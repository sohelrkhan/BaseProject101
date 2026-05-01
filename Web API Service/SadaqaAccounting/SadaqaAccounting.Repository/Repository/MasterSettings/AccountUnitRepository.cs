namespace SadaqaAccounting.Repository.Repository.MasterSettings
{
    public class AccountUnitRepository : BaseRepository<AccountUnit>, IAccountUnitRepository
    {
        public AccountUnitRepository(DatabaseContext databaseContext) : base(databaseContext) { }

        public override async Task<ICollection<AccountUnit>> GetAllAsync()
        {
            var accountUnits = await dbContext.AccountUnits
                .AsNoTracking()
                .Where(au => !au.IsDeleted)
                .ToListAsync();

            return accountUnits;
        }

        public override async Task<AccountUnit?> GetByIdAsync(int id)
        {
            var accountUnit = await dbContext.AccountUnits
                .Where(au => !au.IsDeleted && au.Id == id)
                .FirstOrDefaultAsync();

            return accountUnit;
        }

        public async Task<ICollection<SelectModel>> GetAccountUnitSelectListAsync()
        {
            var accountUnits = await dbContext.AccountUnits
                .AsNoTracking()
                .Where(au => !au.IsDeleted)
                .OrderBy(au => au.Name)
                .Select(au => new SelectModel
                {
                    Id = au.Id,
                    Name = au.Name
                })
                .ToListAsync();

            return accountUnits;
        }

        public async Task<List<AccountUnit>> GetByCodesAsync(ISet<string> codes)
        {
            if (codes == null || codes.Count == 0)
                return new List<AccountUnit>();

            var normalizedCodes = codes
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim().ToUpper())
                .ToHashSet();

            return await dbContext.AccountUnits
                .AsNoTracking()
                .Where(au =>
                    !au.IsDeleted &&
                    normalizedCodes.Contains(au.Code.ToUpper()))
                .ToListAsync();
        }
    }
}