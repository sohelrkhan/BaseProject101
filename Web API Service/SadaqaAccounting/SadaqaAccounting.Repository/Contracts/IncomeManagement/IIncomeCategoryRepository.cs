namespace SadaqaAccounting.Repository.Contracts.IncomeManagement
{
    public interface IIncomeCategoryRepository: IBaseRepository<IncomeCategory>
    {
        Task<PaginatedResponse<IncomeCategory>> GetIncomeCategoriesFilterAsync(PaginationRequest paginationRequest, CancellationToken cancellationToken = default);
        Task<ICollection<SelectModel>> GetIncomeCategorySelectListAsync(int accountUnitId);
        Task<IEnumerable<SelectModel>> GetIncomeCategorySelectListByAccountUnitAsync(int accountUnitId, CancellationToken cancellationToken);
    }
}