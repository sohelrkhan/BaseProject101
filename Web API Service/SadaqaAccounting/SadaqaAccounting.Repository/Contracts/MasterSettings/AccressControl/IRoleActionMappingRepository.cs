namespace SadaqaAccounting.Repository.Contracts.MasterSettings.AccressControl
{
    public interface IRoleActionMappingRepository : IBaseRepository<RoleActionMapping>
    {
        Task<ICollection<RoleActionMapping>> GetRoleMappingById(int id);
    }
}