namespace SadaqaAccounting.Repository.Contracts.MasterSettings
{
    public interface IUserAccountUnitRepository: IBaseRepository<UserAccountUnit>
    {
        Task<ICollection<UserAccountUnit>> GetUserAccountUnitSelectListAsync(string userId);
        Task<bool> UserHasAccountUnitAsync(string userId, int accountUnitId, CancellationToken cancellationToken);
    }
}