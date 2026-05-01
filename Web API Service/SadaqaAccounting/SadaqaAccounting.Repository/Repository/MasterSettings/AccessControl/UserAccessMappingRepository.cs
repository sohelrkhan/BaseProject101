namespace SadaqaAccounting.Repository.Repository.MasterSettings.AccessControl
{
    public class UserAccessMappingRepository : BaseRepository<UserAccessMapping>, IUserAccessMappingRepository
    {
        public UserAccessMappingRepository(DatabaseContext databaseContext) : base(databaseContext) { }

        public override async Task<ICollection<UserAccessMapping>> GetAllAsync()
        {
            return await dbContext.UserAccessMappings
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.Feature)
                .Include(x => x.Action)
                    .ThenInclude(x => x.Status)
                .ToListAsync();
        }

        public async Task<ICollection<UserAccessMapping>> GetUserWiseAccessAsync(string userId)
        {
            var userPermissions = await dbContext.UserAccessMappings
                .Where(uam => uam.UserId == userId)
                .Include(uam => uam.Feature)
                .Include(uam => uam.Action)
                    .ThenInclude (uam => uam.Status)
                .Include (uam => uam.User)
                .ToListAsync();

            return userPermissions;
        }

        public async Task<UserAccessMapping?> GetUserAccessMappingByUserFeatureActionId(string userId, int featureId, int actionId)
        {
            var userAccessMapping = await dbContext.UserAccessMappings
                .Where(uam => uam.UserId == userId && uam.FeatureId == featureId && uam.ActionId == actionId)
                .FirstOrDefaultAsync();

            return userAccessMapping;
        }
    }
}