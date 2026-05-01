namespace SadaqaAccounting.Repository.Repository.CashBankManagement
{
    public class OpeningBalanceRepository : BaseRepository<OpeningBalance>, IOpeningBalanceRepository
    {
        public OpeningBalanceRepository(DatabaseContext databaseContext) : base(databaseContext) { }

        public async Task<PaginatedResponse<OpeningBalance>> GetOpeningBalancesFilterAsync(PaginationRequest paginationRequest,
            CancellationToken cancellationToken = default)
        {
            // Get opening balances 
            var openingBalances = dbContext.OpeningBalances
                .AsNoTracking()
                .Where(ob => !ob.IsDeleted && ob.AccountUnitId == paginationRequest.AccountUnitId)
                .Include(ob => ob.AccountUnit)
                .Include(ob => ob.PaymentMode)
                .Include(ob => ob.Bank)
                .Include(ob => ob.Cash)
                .AsNoTracking();

            // Apply filtering
            var filterValue = paginationRequest.SearchTerm?.Trim();
            if (!string.IsNullOrWhiteSpace(filterValue) && !string.IsNullOrEmpty(filterValue))
            {
                var searchPattern = $"%{filterValue}%";
                openingBalances = openingBalances
                    .Where(ob => (ob.PaymentMode != null && EF.Functions.Like(ob.PaymentMode.Name, searchPattern))
                    || (ob.Bank != null && EF.Functions.Like(ob.Bank.Name, searchPattern))
                    || (ob.Cash != null && EF.Functions.Like(ob.Cash.Name, searchPattern)));
            }

            // Apply sorting
            var sortableColumns = new Dictionary<string, Expression<Func<OpeningBalance, object>>>
                (StringComparer.OrdinalIgnoreCase)
            {
                ["paymentmethodname"] = u => u.PaymentMode.Name,
                ["bankname"] = u => u.Bank.Name,
                ["cashname"] = u => u.Cash.Name,
                ["id"] = u => u.Id
            };

            var sortColumn = paginationRequest.SortField?.Trim();
            var sortOrder = paginationRequest.SortOrder?.Trim();

            if (!string.IsNullOrWhiteSpace(sortColumn) && sortableColumns.TryGetValue(sortColumn, out var sortExpression))
                openingBalances = string.Equals(sortOrder, "descend", StringComparison.OrdinalIgnoreCase) ? openingBalances.OrderByDescending(sortExpression) : openingBalances.OrderBy(sortExpression);
            else
                openingBalances = openingBalances.OrderBy(b => b.Id);

            // Get total count before paging
            var totalCount = await openingBalances.CountAsync(cancellationToken);

            // Apply paging
            var pageSize = paginationRequest.PageSize <= 0 ? 10 : paginationRequest.PageSize;
            var pageIndex = paginationRequest.Page < 0 ? 0 : paginationRequest.Page;

            // Retrieve paged result
            var result = await openingBalances
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            // Prepare paginated response
            var paginatedResponse = new PaginatedResponse<OpeningBalance>
            {
                Data = result,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageSize = pageSize,
                CurrentPage = pageIndex,
                TotalRecords = totalCount,
            };

            return paginatedResponse;
        }

        public async Task<OpeningBalance?> GetOpeningBalanceByBankAsync(int bankId, CancellationToken cancellationToken)
        {
            var openingBalance = await dbContext.OpeningBalances
                .Where(ob => ob.BankId == bankId && !ob.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            return openingBalance;
        }
    }
}