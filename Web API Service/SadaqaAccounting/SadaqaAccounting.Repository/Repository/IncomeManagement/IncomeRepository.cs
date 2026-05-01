namespace SadaqaAccounting.Repository.Repository.IncomeManagement
{
    public class IncomeRepository : BaseRepository<Income>, IIncomeRepository
    {
        public IncomeRepository(DatabaseContext databaseContext) : base(databaseContext) { }

        public async Task<PaginatedResponse<Income>> GetIncomesFilterAsync(PaginationRequest paginationRequest, CancellationToken cancellationToken = default)
        {
            // Get incomes
            var incomes = dbContext.Incomes
                .Where(i => !i.IsDeleted && i.AccountUnitId == paginationRequest.AccountUnitId)
                .Include(i => i.AccountUnit)
                .Include(i => i.IncomeCategory)
                .Include(i => i.Event)
                .Include(i => i.Bank)
                .Include(i => i.Cash)
                .Include(i => i.Donor)
                .Include(i => i.PaymentMode)
                .AsNoTracking();

            // Apply filtering
            var filterValue = paginationRequest.SearchTerm?.Trim();
            if (!string.IsNullOrWhiteSpace(filterValue) && !string.IsNullOrEmpty(filterValue))
            {
                var searchPattern = $"%{filterValue}%";
                incomes = incomes
                    .Where(i => (i.AccountUnit.Name != null && EF.Functions.Like(i.AccountUnit.Name, searchPattern)
                        || (i.IncomeCategory.Name != null && EF.Functions.Like(i.IncomeCategory.Name, searchPattern))
                        || (i.Event.Name != null && EF.Functions.Like(i.Event.Name, searchPattern))
                        || (i.PaymentMode.Name != null && EF.Functions.Like(i.PaymentMode.Name, searchPattern))
                        || (i.Bank.Name != null && EF.Functions.Like(i.Bank.Name, searchPattern))
                        || (i.Cash.Name != null && EF.Functions.Like(i.Cash.Name, searchPattern))
                        || (i.Donor.Name != null && EF.Functions.Like(i.Donor.Name, searchPattern))));
            }

            // Apply sorting
            var sortableColumns = new Dictionary<string, Expression<Func<Income, object>>>(StringComparer.OrdinalIgnoreCase)
            {
                ["incomeCategoryName"] = e => e.IncomeCategory.Name,
                ["eventName"] = e => e.Event.Name,
                ["paymentModeName"] = e => e.PaymentMode.Name,
                ["bankName"] = e => e.Bank.Name,
                ["bankName"] = e => e.Cash.Name,
                ["donorName"] = e => e.Donor.Name,
                ["id"] = e => e.Id
            };

            var sortColumn = paginationRequest.SortField?.Trim();
            var sortOrder = paginationRequest.SortOrder?.Trim();

            if (!string.IsNullOrWhiteSpace(sortColumn) && sortableColumns.TryGetValue(sortColumn, out var sortExpression))
                incomes = string.Equals(sortOrder, "descend", StringComparison.OrdinalIgnoreCase) ? incomes.OrderByDescending(sortExpression) : incomes.OrderBy(sortExpression);
            else
                incomes = incomes.OrderBy(e => e.Id);

            // Get total count before paging
            var totalCount = await incomes.CountAsync(cancellationToken);

            // Apply paging
            var pageSize = paginationRequest.PageSize <= 0 ? 10 : paginationRequest.PageSize;
            var pageIndex = paginationRequest.Page < 0 ? 0 : paginationRequest.Page;

            // Retrieve paged result
            var result = await incomes
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            // Prepare paginated response
            var paginatedResponse = new PaginatedResponse<Income>
            {
                Data = result,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageSize = pageSize,
                CurrentPage = pageIndex,
                TotalRecords = totalCount,
            };

            return paginatedResponse;
        }

        public override async Task<Income?> GetByIdAsync(int id)
        {
            var income = await dbContext.Incomes
                .Where(e => !e.IsDeleted & e.Id == id)
                .FirstOrDefaultAsync();

            return income;
        }

        public async Task<ICollection<Income>> GetIncomeSummaryByFilterAsync(int accountUnitId, int? eventId, DateTime fromDate, DateTime toDate)
        {
            var incomes = await dbContext.Incomes
                          .Include(i => i.IncomeCategory)
                          .Where(x => x.AccountUnitId == accountUnitId && (eventId == null || x.EventId == eventId) &&
                          x.ReceiptDate.Date >= fromDate && x.ReceiptDate.Date <= toDate).ToListAsync();
            return incomes;
        }
    }
}