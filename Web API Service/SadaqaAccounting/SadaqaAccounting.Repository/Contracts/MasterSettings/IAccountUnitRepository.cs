namespace SadaqaAccounting.Repository.Contracts.MasterSettings
{
    public interface IAccountUnitRepository : IBaseRepository<AccountUnit>
    {
        Task<List<AccountUnit>> GetByCodesAsync(ISet<string> codes);
        Task <ICollection<SelectModel>> GetAccountUnitSelectListAsync();
    }
}