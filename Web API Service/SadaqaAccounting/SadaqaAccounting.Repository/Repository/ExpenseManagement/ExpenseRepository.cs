namespace SadaqaAccounting.Repository.Repository.ExpenseManagement
{
    public class ExpenseRepository : BaseRepository<Expense>, IExpenseRepository
    {
        public ExpenseRepository(DatabaseContext databaseContext) : base(databaseContext) { }

        public override async Task<ICollection<Expense>> GetAllAsync()
        {
            var expenses = await dbContext.Expenses
                .AsNoTracking()
                .Where(ec => !ec.IsDeleted)
                .ToListAsync();

            return expenses;
        }

        public async Task<PaginatedResponse<Expense>> GetExpensesFilterAsync(PaginationRequest paginationRequest, CancellationToken cancellationToken = default)
        {
            // Get expenses
            var expenses = dbContext.Expenses
                .Where(e => !e.IsDeleted && e.AccountUnitId == paginationRequest.AccountUnitId)
                .Include(e => e.AccountUnit)
                .Include(e => e.ExpenseCategory)
                .Include(e => e.Event)
                .Include(e => e.Bank)
                .Include (e => e.Cash)
                .Include(e => e.PaymentMode)
                .AsNoTracking();

            // Apply filtering
            var filterValue = paginationRequest.SearchTerm?.Trim();
            if (!string.IsNullOrWhiteSpace(filterValue) && !string.IsNullOrEmpty(filterValue))
            {
                var searchPattern = $"%{filterValue}%";
                expenses = expenses
                    .Where(e => (e.AccountUnit.Name != null && EF.Functions.Like(e.AccountUnit.Name, searchPattern)
                        || (e.ExpenseCategory.Name != null && EF.Functions.Like(e.ExpenseCategory.Name, searchPattern))
                        || (e.Event.Name != null && EF.Functions.Like(e.Event.Name, searchPattern))
                        || (e.PaymentMode.Name != null && EF.Functions.Like(e.PaymentMode.Name, searchPattern))
                        || (e.Bank.Name != null && EF.Functions.Like(e.Bank.Name, searchPattern))
                        || (e.Cash.Name != null && EF.Functions.Like(e.Cash.Name, searchPattern))));
            }

            // Apply sorting
            var sortableColumns = new Dictionary<string, Expression<Func<Expense, object>>>(StringComparer.OrdinalIgnoreCase)
            {
                ["expenseCategoryName"] = e => e.ExpenseCategory.Name,
                ["eventName"] = e => e.Event.Name,
                ["paymentModeName"] = e => e.PaymentMode.Name,
                ["bankName"] = e => e.Bank.Name,
                ["bankName"] = e => e.Cash.Name,
                ["id"] = e => e.Id
            };

            var sortColumn = paginationRequest.SortField?.Trim();
            var sortOrder = paginationRequest.SortOrder?.Trim();

            if (!string.IsNullOrWhiteSpace(sortColumn) && sortableColumns.TryGetValue(sortColumn, out var sortExpression))
                expenses = string.Equals(sortOrder, "descend", StringComparison.OrdinalIgnoreCase) ? expenses.OrderByDescending(sortExpression) : expenses.OrderBy(sortExpression);
            else
                expenses = expenses.OrderBy(e => e.Id);

            // Get total count before paging
            var totalCount = await expenses.CountAsync(cancellationToken);

            // Apply paging
            var pageSize = paginationRequest.PageSize <= 0 ? 10 : paginationRequest.PageSize;
            var pageIndex = paginationRequest.Page < 0 ? 0 : paginationRequest.Page;

            // Retrieve paged result
            var result = await expenses
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            // Prepare paginated response
            var paginatedResponse = new PaginatedResponse<Expense>
            {
                Data = result,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageSize = pageSize,
                CurrentPage = pageIndex,
                TotalRecords = totalCount,
            };

            return paginatedResponse;
        }

        public override async Task<Expense?> GetByIdAsync(int id)
        {
            var expense = await dbContext.Expenses
                         .Where(e => !e.IsDeleted & e.Id == id)
                         .FirstOrDefaultAsync();

            return expense;
        }

        public async Task<ICollection<Expense>> GetExpenseSummaryByFilterAsync(int accountUnitId, int? eventId, DateTime fromDate, DateTime toDate)
        {
            var expenses = await dbContext.Expenses
                          .Include(i => i.ExpenseCategory)
                          .Where(x => x.AccountUnitId == accountUnitId && (eventId == null || x.EventId == eventId) &&
                          x.ExpenseDate.Date >= fromDate && x.ExpenseDate.Date <= toDate).ToListAsync();
            return expenses;
        }
    }
}