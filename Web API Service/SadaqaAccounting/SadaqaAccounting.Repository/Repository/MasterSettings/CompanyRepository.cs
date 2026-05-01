namespace SadaqaAccounting.Repository.Repository.MasterSettings
{
    public class CompanyRepository : BaseRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(DatabaseContext databaseContext) : base(databaseContext) { }

        public override async Task<ICollection<Company>> GetAllAsync()
        {
            var getCompanies = await dbContext.Companies
                .AsNoTracking()
                .Where(c => !c.IsDeleted)
                .Include(c => c.Status)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return getCompanies;
        }

        public async Task<ICollection<SelectModel>> GetCompanySelectListAsync()
        {
            var companies = await dbContext.Companies
                .AsNoTracking()
                .Where(c => c.StatusId != GlobalStatus.Inactive && !c.IsDeleted)
                .OrderBy(c => c.Name)
                .Select(c => new SelectModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            return companies;
        }

        public override async Task<Company?> GetByIdAsync(int id)
        {
            var getCompany = await dbContext.Companies
                .Where(c => c.Id == id && !c.IsDeleted)
                .FirstOrDefaultAsync();

            return getCompany;
        }

        public async Task<Company?> GetFirstOrDefaultCompanyAsync()
        {
            var getFirstOrDefaultCompany = await dbContext.Companies.FirstOrDefaultAsync(c => !c.IsDeleted);
            return getFirstOrDefaultCompany;
        }
    }
}