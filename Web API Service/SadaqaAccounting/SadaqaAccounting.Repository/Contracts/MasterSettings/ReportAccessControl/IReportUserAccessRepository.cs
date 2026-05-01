namespace SadaqaAccounting.Repository.Contracts.MasterSettings.ReportAccessControl
{
    public interface IReportUserAccessRepository : IBaseRepository<ReportUserAccess>
    {
        Task<ICollection<ReportUserAccess>> GetReportUserAccessesByUserAsync(string userId);
    }
}