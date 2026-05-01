namespace SadaqaAccounting.Repository.Repository.IncomeManagement
{
    public class IncomeCategoryRepository : BaseRepository<IncomeCategory>, IIncomeCategoryRepository
    {
        public IncomeCategoryRepository(DatabaseContext context) : base(context) { }

        public async Task<PaginatedResponse<IncomeCategory>> GetIncomeCategoriesFilterAsync(PaginationRequest paginationRequest, CancellationToken cancellationToken = default)
        {
            // Get income categories
            var incomeCategories = dbContext.IncomeCategories
                .AsNoTracking()
                .Include(a => a.AccountUnit)
                .Where(a => a.AccountUnitId == paginationRequest.AccountUnitId && !a.IsDeleted);
                

            // Apply filtering
            var filterValue = paginationRequest.SearchTerm?.Trim();
            if (!string.IsNullOrWhiteSpace(filterValue) && !string.IsNullOrEmpty(filterValue))
            {
                var searchPattern = $"%{filterValue}%";
                incomeCategories = incomeCategories.Where(a => (a.Name != null && EF.Functions.Like(a.Name, searchPattern)));
            }

            // Apply sorting
            var sortableColumns = new Dictionary<string, Expression<Func<IncomeCategory, object>>>(StringComparer.OrdinalIgnoreCase)
            {
                ["name"] = u => u.Name,
                ["id"] = u => u.Id
            };

            var sortColumn = paginationRequest.SortField?.Trim();
            var sortOrder = paginationRequest.SortOrder?.Trim();

            if (!string.IsNullOrWhiteSpace(sortColumn) && sortableColumns.TryGetValue(sortColumn, out var sortExpression))
                incomeCategories = string.Equals(sortOrder, "descend", StringComparison.OrdinalIgnoreCase) ? incomeCategories.OrderByDescending(sortExpression) : incomeCategories.OrderBy(sortExpression);
            else
                incomeCategories = incomeCategories.OrderBy(u => u.Id);

            // Get total count before paging
            var totalCount = await incomeCategories.CountAsync(cancellationToken);

            // Apply paging
            var pageSize = paginationRequest.PageSize <= 0 ? 10 : paginationRequest.PageSize;
            var pageIndex = paginationRequest.Page < 0 ? 0 : paginationRequest.Page;

            // Retrieve paged result
            var result = await incomeCategories
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            // Prepare paginated response
            var paginatedResponse = new PaginatedResponse<IncomeCategory>
            {
                Data = result,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageSize = pageSize,
                CurrentPage = pageIndex,
                TotalRecords = totalCount,
            };

            return paginatedResponse;
        }
        public override async Task<IncomeCategory?> GetByIdAsync(int id)
        {
            var incomeCategory = await dbContext.IncomeCategories
                         .Where(ec => !ec.IsDeleted & ec.Id == id)
                         .FirstOrDefaultAsync();

            return incomeCategory;
        }
        public async Task<ICollection<SelectModel>> GetIncomeCategorySelectListAsync(int accountUnitId)
        {
            var incomeCategories = await dbContext.IncomeCategories
                .AsNoTracking()
                .Where(ec => ec.AccountUnitId == accountUnitId && !ec.IsDeleted)
                .Select(s => new SelectModel
                {
                    Id = s.Id,
                    Name = s.Name,
                }).ToListAsync();

            return incomeCategories;
        }

        public async Task<IEnumerable<SelectModel>> GetIncomeCategorySelectListByAccountUnitAsync(int accountUnitId, CancellationToken cancellationToken)
        {
            var incomeCategories = await dbContext.IncomeCategories
                .AsNoTracking()
                .Where(ic => ic.AccountUnitId == accountUnitId && !ic.IsDeleted)
                .Select(s => new SelectModel
                {
                    Id = s.Id,
                    Name = s.Name,
                }).ToListAsync();

            return incomeCategories;
        }
    }
}