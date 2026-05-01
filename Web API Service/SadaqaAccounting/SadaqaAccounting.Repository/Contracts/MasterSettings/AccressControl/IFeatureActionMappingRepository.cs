namespace SadaqaAccounting.Repository.Contracts.MasterSettings.AccressControl
{
    public interface IFeatureActionMappingRepository : IBaseRepository<FeatureActionMapping>
    {
        Task<ICollection<FeatureActionMapping>> GetFeatureWiseActionsAsync(int id);
        Task<ICollection<FeatureActionMapping>> GetAllFeatureWiseActionsAsync();
        Task<FeatureActionMapping?> GetFeatureActionMappingByFeatureAndActionId(int featureId, int actionId);
    }
}