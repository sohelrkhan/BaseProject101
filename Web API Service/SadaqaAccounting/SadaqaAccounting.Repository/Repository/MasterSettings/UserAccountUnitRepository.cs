namespace SadaqaAccounting.Repository.Repository.MasterSettings
{
    public class UserAccountUnitRepository : BaseRepository<UserAccountUnit>, IUserAccountUnitRepository
    {
        public UserAccountUnitRepository(DatabaseContext context) : base(context) { }

        public override async Task<ICollection<UserAccountUnit>> GetAllAsync()
        {
            var userAccountUnits = await dbContext.UserAccountUnits
                .AsNoTracking()
                .Where(uau => !uau.IsDeleted)
                .ToListAsync();

            return userAccountUnits;
        }

        public async Task<ICollection<UserAccountUnit>> GetUserAccountUnitSelectListAsync(string userId)
        {
            var accountUnits = await dbContext.UserAccountUnits
                .Where(uau => uau.UserId == userId && !uau.IsDeleted)
                .Include(uau => uau.AccountUnit)
                .Include(i => i.AccountUnit)
                .ToListAsync();

            return accountUnits;
        }

        public async Task<bool> UserHasAccountUnitAsync(string userId, int accountUnitId, CancellationToken cancellationToken)
        {
            var hasUserAccountUnit = await dbContext.UserAccountUnits
                .Where(uau => !uau.IsDeleted && uau.UserId == userId && uau.AccountUnitId == accountUnitId)
                .FirstOrDefaultAsync(cancellationToken);

            if (hasUserAccountUnit == null) 
                return false;

            return true;
        }
    }
}