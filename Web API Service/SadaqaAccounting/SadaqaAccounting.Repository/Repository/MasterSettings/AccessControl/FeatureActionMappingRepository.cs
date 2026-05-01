namespace SadaqaAccounting.Repository.Repository.MasterSettings.AccessControl
{
    public class FeatureActionMappingRepository : BaseRepository<FeatureActionMapping>, IFeatureActionMappingRepository
    {
        public FeatureActionMappingRepository(DatabaseContext databaseContext) : base(databaseContext) { }

        public override async Task<ICollection<FeatureActionMapping>> GetAllAsync()
        {
            var featureActions = await dbContext.FeatureActionMappings
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .Include(i => i.Feature)
                .Include(i => i.Action)
                .ToListAsync();

            return featureActions;
        }

        public override async Task<FeatureActionMapping?> GetByIdAsync(int id)
        {
            var featureAction = await dbContext.FeatureActionMappings
                .Where(x => !x.IsDeleted & x.Id == id)
                .FirstOrDefaultAsync();

            return featureAction;
        }

        public async Task<ICollection<FeatureActionMapping>> GetFeatureWiseActionsAsync(int id)
        {
            var featureWiseActions = await dbContext.FeatureActionMappings
                .Where(x => x.FeatureId == id)
                .Include(i => i.Action)
                    .ThenInclude(a => a.Status)
                .ToListAsync();

            return featureWiseActions;
        }

        public async Task<ICollection<FeatureActionMapping>> GetAllFeatureWiseActionsAsync()
        {
            var featureWiseActions = await dbContext.FeatureActionMappings
                .AsNoTracking()
                .Include(i => i.Feature)
                .Include(i => i.Action)
                    .ThenInclude(a => a.Status)
                .ToListAsync();

            return featureWiseActions;
        }

        public async Task<FeatureActionMapping?> GetFeatureActionMappingByFeatureAndActionId(int featureId, int actionId)
        {
            var featureActionMapping = await dbContext.FeatureActionMappings
                .Where(fam => fam.FeatureId == featureId && fam.ActionId == actionId && !fam.IsDeleted)
                .FirstOrDefaultAsync();

            return featureActionMapping;
        }
    }
}