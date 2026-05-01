namespace SadaqaAccounting.Repository.Repository.MasterSettings
{
    public class UserLoginHistoryRepository : BaseRepository<UserLoginHistory>, IUserLoginHistoryRepository
    {
        public UserLoginHistoryRepository(DatabaseContext databaseContext) : base(databaseContext) { }

        public override async Task<ICollection<UserLoginHistory>> GetAllAsync()
        {
            var getUserLoginHistories = await dbContext.UserLoginHistories
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return getUserLoginHistories;
        }

        public override async Task<UserLoginHistory?> GetByIdAsync(int id)
        {
            var getUserLoginHistory = await dbContext.UserLoginHistories
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            return getUserLoginHistory!;
        }
    }
}