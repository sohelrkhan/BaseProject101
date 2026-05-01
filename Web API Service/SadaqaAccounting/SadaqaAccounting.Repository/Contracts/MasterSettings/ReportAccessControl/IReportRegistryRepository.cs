namespace SadaqaAccounting.Repository.Contracts.MasterSettings.ReportAccessControl
{
    public interface IReportRegistryRepository : IBaseRepository<ReportRegistry>
    {
        Task<ICollection<ReportRegistry>> GetAllByModuleAsync(int moduleId);
        Task<ReportRegistry> GetByReportCodeAsync(string reportCode);
        Task<IEnumerable<ReportRegistry>> GetReportRegistersByModuleIdAndReportGroupName(int? moduleId, string? reportGroupName);
        Task<IEnumerable<SelectModel>> GetReportGroupSelectList();
    }
}