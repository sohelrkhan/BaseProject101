using SadaqaAccounting.Model.Models.IncomeManagement;

namespace SadaqaAccounting.Repository.Contracts.ExpenseManagement
{
    public interface IExpenseRepository : IBaseRepository<Expense>
    {
        Task<PaginatedResponse<Expense>> GetExpensesFilterAsync(PaginationRequest paginationRequest, CancellationToken cancellationToken = default);
        Task<ICollection<Expense>> GetExpenseSummaryByFilterAsync(int accountUnitId, int? eventId, DateTime fromDate, DateTime toDate);
    }
}