namespace SadaqaAccounting.Repository.Contracts.MasterSettings.Enum
{
    public interface IEnumTypeCollectionRepository : IBaseRepository<EnumTypeCollection>
    {
        Task<List<string>> GetEnumTypeCollectionByIds(List<int> ids);
    }
}