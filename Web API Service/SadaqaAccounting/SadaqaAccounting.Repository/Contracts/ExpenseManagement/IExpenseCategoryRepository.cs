namespace SadaqaAccounting.Repository.Contracts.ExpenseManagement
{
    public interface IExpenseCategoryRepository : IBaseRepository<ExpenseCategory>
    {
        Task<PaginatedResponse<ExpenseCategory>> GetExpenseCategoriesFilterAsync(PaginationRequest paginationRequest, CancellationToken cancellationToken = default);
        Task<ICollection<SelectModel>> GetExpenseCategorySelectListAsync();
        Task<IEnumerable<SelectModel>> GetExpenseCategorySelectListByAccountUnitAsync(int accountUnitId, CancellationToken cancellationToken);
    }
}