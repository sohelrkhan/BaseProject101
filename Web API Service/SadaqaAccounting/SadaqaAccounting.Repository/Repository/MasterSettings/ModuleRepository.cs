namespace SadaqaAccounting.Repository.Repository.MasterSettings
{
    public class ModuleRepository : BaseRepository<Model.Models.MasterSettings.Module>, IModuleRepository
    {
        public ModuleRepository(DatabaseContext databaseContext) : base(databaseContext) { }

        public override async Task<ICollection<Model.Models.MasterSettings.Module>> GetAllAsync()
        {
            var modules = await dbContext.Modules
                .AsNoTracking()
                .Where(m => !m.IsDeleted)
                .Include(m => m.Status)
                .OrderBy(m => m.Name)
                .ToListAsync();

            return modules;
        }

        public override async Task<Model.Models.MasterSettings.Module?> GetByIdAsync(int id)
        {
            var getModule = await dbContext.Modules
                .Where(m => m.Id == id && !m.IsDeleted)
                .FirstOrDefaultAsync();

            return getModule;
        }

        public async Task<Model.Models.MasterSettings.Module?> GetModuleByName(string name)
        {
            return await dbContext.Modules.FirstOrDefaultAsync(m => m.Name.ToLower() == name.ToLower());
        }

        public async Task<Model.Models.MasterSettings.Module?> GetModuleByCode(string code)
        {
            return await dbContext.Modules.FirstOrDefaultAsync(m => m.Code.ToLower() == code.ToLower());
        }

        // Get module select list
        public async Task<IEnumerable<SelectModel>> GetModuleSelectListAsync()
        {
            var modules = dbContext.Modules
                .AsNoTracking()
                .Where(m => !m.IsDeleted && m.StatusId != GlobalStatus.Inactive)
                .OrderBy(m => m.Name)
                .Select(m => new SelectModel
                {
                    Id = m.Id,
                    Name = m.Name,
                });

            return await modules.ToListAsync();
        }
    }
}