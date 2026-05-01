namespace SadaqaAccounting.Repository.Contracts.MasterSettings
{
    public interface IModuleRepository : IBaseRepository<Model.Models.MasterSettings.Module>
    {
        Task<Model.Models.MasterSettings.Module?> GetModuleByName(string name);
        Task<Model.Models.MasterSettings.Module?> GetModuleByCode(string code);
        Task<IEnumerable<SelectModel>> GetModuleSelectListAsync();
    }
}