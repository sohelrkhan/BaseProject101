namespace SadaqaAccounting.Repository.Contracts.MasterSettings.AccressControl
{
    public interface IUserAccessMappingRepository : IBaseRepository<UserAccessMapping>
    {
        Task<ICollection<UserAccessMapping>> GetUserWiseAccessAsync(string userId);
        Task<UserAccessMapping?> GetUserAccessMappingByUserFeatureActionId(string userId, int featureId, int actionId);
    }
}