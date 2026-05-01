namespace SadaqaAccounting.Repository.Repository.MasterSettings
{
    public class FeaturesRepository : BaseRepository<Feature>, IFeaturesRepository
    {
        public FeaturesRepository(DatabaseContext databaseContext) : base(databaseContext) { }

        public override async Task<ICollection<Feature>> GetAllAsync()
        {
            var getFeatures = await dbContext.Features
                .AsNoTracking()
                .Where(f => !f.IsDeleted)
                .Include(f => f.Module)
                .Include(f => f.Status)
                .OrderBy(f => f.Name)
                .ToListAsync();

            return getFeatures;
        }

        public override async Task<Feature?> GetByIdAsync(int id)
        {
            var getFeature = await dbContext.Features
                .Where(f => f.Id == id && !f.IsDeleted)
                .FirstOrDefaultAsync();

            return getFeature!;
        }

        public async Task<Feature?> GetFeatureByTableName(string linkedTableName)
        {
            var getExistFeature = await dbContext.Features
                .Where(f => f.LinkedTableName.Trim().ToLower() == linkedTableName.Trim().ToLower() && !f.IsDeleted)
                .FirstOrDefaultAsync();

            // Get default feature
            if (getExistFeature is null)
                getExistFeature = await dbContext.Features.Where(f => f.Code.Equals("Default")).FirstOrDefaultAsync();

            return getExistFeature!;
        }

        public async Task<Feature?> GetFeatureByControllerName(string controllerName)
        {
            var feature = await dbContext.Features
                .Where(f => f.LinkedControllerName != null && f.LinkedControllerName.ToLower() == controllerName.ToLower() && !f.IsDeleted)
                .FirstOrDefaultAsync();

            return feature;
        }

        public async Task<Feature?> GetFeatureByCode(string code)
        {
            var getFeature = await dbContext.Features
                .Where(f => f.Code.ToLower() == code.ToLower() && !f.IsDeleted)
                .FirstOrDefaultAsync();

            return getFeature;
        }

        public async Task<ICollection<SelectModel>> GetFeatureSelectListAsync(CancellationToken cancellationToken)
        {
            var getFeatures = await dbContext.Features
                .AsNoTracking()
                .Where(f => !f.IsDeleted)
                .Select(s => new SelectModel()
                {
                    Id = s.Id,
                    Name = s.Name
                }).ToListAsync(cancellationToken);

            return getFeatures;
        }

        public async Task<ICollection<SelectModel>> GetFeaturesSelectListByModuleIdAsync(int moduleId, CancellationToken cancellationToken)
        {
            var getFeatures = await dbContext.Features
                .AsNoTracking()
                .Where(f => f.ModuleId == moduleId && !f.IsDeleted)
                .Select(s => new SelectModel()
                {
                    Id = s.Id,
                    Name = s.Name
                }).ToListAsync(cancellationToken);

            return getFeatures;
        }
    }
}