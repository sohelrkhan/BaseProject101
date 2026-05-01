namespace SadaqaAccounting.Repository.Contracts.MasterSettings
{
    public interface ICompanyRepository : IBaseRepository<Company>
    {
        Task<ICollection<SelectModel>> GetCompanySelectListAsync();
        Task<Company?> GetFirstOrDefaultCompanyAsync();
    }
}