namespace SadaqaAccounting.Repository.Repository.MasterSettings.ReportAccessControl
{
    public class ReportRegistryRepository : BaseRepository<ReportRegistry>, IReportRegistryRepository
    {
        public ReportRegistryRepository(DatabaseContext databaseContext) : base(databaseContext) { }

        public override async Task<ICollection<ReportRegistry>> GetAllAsync()
        {
            var getReportRegistries = await dbContext.ReportRegistries
                .AsNoTracking()
                .Where(c => !c.IsDeleted)
                .Include(c => c.Module)
                .Include(c => c.Status)
                .ToListAsync();

            return getReportRegistries;
        }

        public override async Task<ReportRegistry?> GetByIdAsync(int id)
        {
            var getReportRegistry = await dbContext.ReportRegistries
                .Where(c => c.Id == id && !c.IsDeleted)
                .FirstOrDefaultAsync();

            return getReportRegistry!;
        }

        public async Task<ICollection<ReportRegistry>> GetAllByModuleAsync(int moduleId)
        {
            var getReportRegistries = await dbContext.ReportRegistries
                .Where(c => !c.IsDeleted && c.ModuleId == moduleId)
                .ToListAsync();

            return getReportRegistries.OrderBy(o => o.Id).ToList();
        }

        public async Task<ReportRegistry> GetByReportCodeAsync(string reportCode)
        {
            var getReportRegistry = await dbContext.ReportRegistries
                .Where(c => c.ReportCode.ToLower() == reportCode.ToLower() && !c.IsDeleted)
                .FirstOrDefaultAsync();

            return getReportRegistry!;
        }

        // Get report registers by module id and report group name
        public async Task<IEnumerable<ReportRegistry>> GetReportRegistersByModuleIdAndReportGroupName(int? moduleId, string? reportGroupName)
        {
            // Get report registers
            var reportRegisters = dbContext.ReportRegistries
                .AsNoTracking()
                .Include(i => i.Module)
                .Where(rr => !rr.IsDeleted);

            // Check if moduleId is provided
            if(moduleId.HasValue && moduleId.Value is not -1)
                reportRegisters = reportRegisters.Where(rr => rr.ModuleId == moduleId.Value);

            // Check if report group name is provided
            if (!string.IsNullOrEmpty(reportGroupName) && !string.IsNullOrWhiteSpace(reportGroupName) && reportGroupName is not "-1")
                reportRegisters = reportRegisters.Where(rr => rr.ReportGroup.ToLower() == reportGroupName.ToLower());

            return await reportRegisters.ToListAsync();
        }

        // Get distinct report group select list
        public async Task<IEnumerable<SelectModel>> GetReportGroupSelectList()
        {
            var reportGroups = dbContext.ReportRegistries
                .AsNoTracking()
                .Where(rr => !rr.IsDeleted && rr.StatusId != GlobalStatus.Inactive)
                .Select(rr => new SelectModel
                {
                    Id = rr.ReportGroup,
                    Name = rr.ReportGroup
                })
                .Distinct();

            return await reportGroups.ToListAsync();
        }
    }
}