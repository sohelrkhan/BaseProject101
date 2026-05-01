namespace SadaqaAccounting.Repository.Repository.ExpenseManagement
{
    public class ExpenseCategoryRepository : BaseRepository<ExpenseCategory>, IExpenseCategoryRepository
    {
        public ExpenseCategoryRepository(DatabaseContext databaseContext) : base(databaseContext) { }

        public override async Task<ICollection<ExpenseCategory>> GetAllAsync()
        {
            var expenseCategories = await dbContext.ExpenseCategories
                .AsNoTracking()
                .Where(ec => !ec.IsDeleted)
                .Include(ec => ec.AccountUnit)
                .ToListAsync();

            return expenseCategories;
        }

        public async Task<PaginatedResponse<ExpenseCategory>> GetExpenseCategoriesFilterAsync(PaginationRequest paginationRequest, CancellationToken cancellationToken = default)
        {
            // Get actions
            var expenseCategories = dbContext.ExpenseCategories
                .Where(ec => !ec.IsDeleted && ec.AccountUnitId == paginationRequest.AccountUnitId)
                .Include(a => a.AccountUnit)
                .AsNoTracking();

            // Apply filtering
            var filterValue = paginationRequest.SearchTerm?.Trim();
            if (!string.IsNullOrWhiteSpace(filterValue) && !string.IsNullOrEmpty(filterValue))
            {
                var searchPattern = $"%{filterValue}%";
                expenseCategories = expenseCategories
                    .Where(a => (a.Name != null && EF.Functions.Like(a.Name, searchPattern)
                        || (a.AccountUnit.Name != null && EF.Functions.Like(a.AccountUnit.Name, searchPattern))));
            }

            // Apply sorting
            var sortableColumns = new Dictionary<string, Expression<Func<ExpenseCategory, object>>>(StringComparer.OrdinalIgnoreCase)
            {
                ["name"] = u => u.Name,
                ["accountUnitName"] = u => u.AccountUnit.Name,
                ["id"] = u => u.Id
            };

            var sortColumn = paginationRequest.SortField?.Trim();
            var sortOrder = paginationRequest.SortOrder?.Trim();

            if (!string.IsNullOrWhiteSpace(sortColumn) && sortableColumns.TryGetValue(sortColumn, out var sortExpression))
                expenseCategories = string.Equals(sortOrder, "descend", StringComparison.OrdinalIgnoreCase) ? expenseCategories.OrderByDescending(sortExpression) : expenseCategories.OrderBy(sortExpression);
            else
                expenseCategories = expenseCategories.OrderBy(u => u.Id);

            // Get total count before paging
            var totalCount = await expenseCategories.CountAsync(cancellationToken);

            // Apply paging
            var pageSize = paginationRequest.PageSize <= 0 ? 10 : paginationRequest.PageSize;
            var pageIndex = paginationRequest.Page < 0 ? 0 : paginationRequest.Page;

            // Retrieve paged result
            var result = await expenseCategories
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            // Prepare paginated response
            var paginatedResponse = new PaginatedResponse<ExpenseCategory>
            {
                Data = result,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageSize = pageSize,
                CurrentPage = pageIndex,
                TotalRecords = totalCount,
            };

            return paginatedResponse;
        }

        public override async Task<ExpenseCategory?> GetByIdAsync(int id)
        {
            var expenseCategor = await dbContext.ExpenseCategories
                         .Where(ec => !ec.IsDeleted & ec.Id == id)
                         .FirstOrDefaultAsync();

            return expenseCategor;
        }

        public async Task<ICollection<SelectModel>> GetExpenseCategorySelectListAsync()
        {
            var expenseCategories = await dbContext.ExpenseCategories
                .AsNoTracking()
                .Where(ec => !ec.IsDeleted)
                .Select(s => new SelectModel
                {
                    Id = s.Id,
                    Name = s.Name,
                }).ToListAsync();

            return expenseCategories;
        }

        public async Task<IEnumerable<SelectModel>> GetExpenseCategorySelectListByAccountUnitAsync(int accountUnitId, CancellationToken cancellationToken)
        {
            var expenseCategories = await dbContext.ExpenseCategories
                .AsNoTracking()
                .Where(ec => ec.AccountUnitId == accountUnitId && !ec.IsDeleted)
                .Select(s => new SelectModel
                {
                    Id = s.Id,
                    Name = s.Name,
                }).ToListAsync();

            return expenseCategories;
        }
    }
}