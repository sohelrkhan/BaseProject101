namespace SadaqaAccounting.Repository.Repository.MasterSettings.ReportAccessControl
{
    public class ReportUserAccessRepository : BaseRepository<ReportUserAccess>, IReportUserAccessRepository
    {
        public ReportUserAccessRepository(DatabaseContext databaseContext) : base(databaseContext) { }

        public override async Task<ICollection<ReportUserAccess>> GetAllAsync()
        {
            var getReportUserAccesses = await dbContext.ReportUserAccesses
                .AsNoTracking()
                .Where(c => !c.IsDeleted)
                .Include(c => c.ReportRegistry)
                .Include(c => c.User)
                .ToListAsync();

            return getReportUserAccesses;
        }

        public override async Task<ReportUserAccess?> GetByIdAsync(int id)
        {
            var getReportUserAccess = await dbContext.ReportUserAccesses
                .Where(c => c.Id == id && !c.IsDeleted)
                .FirstOrDefaultAsync();

            return getReportUserAccess!;
        }

        public async Task<ICollection<ReportUserAccess>> GetReportUserAccessesByUserAsync(string userId)
        {
            var userPermissions = await dbContext.ReportUserAccesses
                .Where(rua => !rua.IsDeleted && rua.UserId == userId)
                .ToListAsync();

            return userPermissions.OrderByDescending(o => o.Id).ToList();
        }
    }
}