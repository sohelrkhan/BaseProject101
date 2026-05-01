namespace SadaqaAccounting.Repository.Contracts.MasterSettings
{
    public interface IFeaturesRepository : IBaseRepository<Feature>
    {
        Task<Feature?> GetFeatureByTableName(string linkedTableName);
        Task<Feature?> GetFeatureByControllerName(string controllerName);
        Task<Feature?> GetFeatureByCode(string code);
        Task<ICollection<SelectModel>> GetFeaturesSelectListByModuleIdAsync(int moduleId, CancellationToken cancellationToken);
        Task<ICollection<SelectModel>> GetFeatureSelectListAsync(CancellationToken cancellationToken);
    }
}