namespace SadaqaAccounting.Repository.Repository.CashBankManagement
{
    public class BankRepository : BaseRepository<Bank>, IBankRepository
    {
        public BankRepository(DatabaseContext databaseContext) : base(databaseContext) { }

        public override async Task<ICollection<Bank>> GetAllAsync()
        {
            var banks = await dbContext.Banks
                .AsNoTracking()
                .Where(b => !b.IsDeleted)
                .Include(b => b.AccountUnit)
                .Include(b => b.Status)
                .ToListAsync();

            return banks;
        }

        public async Task<PaginatedResponse<Bank>> GetBanksFilterAsync(PaginationRequest paginationRequest, CancellationToken cancellationToken = default)
        {
            // Get banks
            var banks = dbContext.Banks
                .AsNoTracking()
                .Where(b => !b.IsDeleted && b.AccountUnitId == paginationRequest.AccountUnitId)
                .Include(b => b.AccountUnit)
                .Include(b => b.Status)
                .AsNoTracking();

            // Apply filtering
            var filterValue = paginationRequest.SearchTerm?.Trim();
            if (!string.IsNullOrWhiteSpace(filterValue) && !string.IsNullOrEmpty(filterValue))
            {
                var searchPattern = $"%{filterValue}%";
                banks = banks.Where(b => (b.Name != null && EF.Functions.Like(b.Name, searchPattern))
                    || (b.BranchName != null && EF.Functions.Like(b.BranchName, searchPattern))
                    || (b.AccountNumber != null && EF.Functions.Like(b.AccountNumber, searchPattern)));
            }

            // Apply sorting
            var sortableColumns = new Dictionary<string, Expression<Func<Bank, object>>>(StringComparer.OrdinalIgnoreCase)
            {
                ["name"] = u => u.Name,
                ["branchName"] = u => u.BranchName,
                ["accountNumber"] = u => u.AccountNumber,
                ["id"] = u => u.Id
            };

            var sortColumn = paginationRequest.SortField?.Trim();
            var sortOrder = paginationRequest.SortOrder?.Trim();

            if (!string.IsNullOrWhiteSpace(sortColumn) && sortableColumns.TryGetValue(sortColumn, out var sortExpression))
                banks = string.Equals(sortOrder, "descend", StringComparison.OrdinalIgnoreCase) ? banks.OrderByDescending(sortExpression) : banks.OrderBy(sortExpression);
            else
                banks = banks.OrderBy(b => b.Id);

            // Get total count before paging
            var totalCount = await banks.CountAsync(cancellationToken);

            // Apply paging
            var pageSize = paginationRequest.PageSize <= 0 ? 10 : paginationRequest.PageSize;
            var pageIndex = paginationRequest.Page < 0 ? 0 : paginationRequest.Page;

            // Retrieve paged result
            var result = await banks
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            // Prepare paginated response
            var paginatedResponse = new PaginatedResponse<Bank>
            {
                Data = result,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageSize = pageSize,
                CurrentPage = pageIndex,
                TotalRecords = totalCount,
            };

            return paginatedResponse;
        }

        public override async Task<Bank?> GetByIdAsync(int id)
        {
            var bank = await dbContext.Banks
                         .Where(ec => !ec.IsDeleted & ec.Id == id)
                         .FirstOrDefaultAsync();

            return bank;
        }

        public async Task<ICollection<SelectModel>> GetBankSelectListAsync()
        {
            var banks = await dbContext.Banks
                .AsNoTracking()
                .Where(ec => !ec.IsDeleted)
                .Select(s => new SelectModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Group = s.OpeningBalances.Count() > 0 ? "Have Opening Balance" : "No Opening Balance"
                }).ToListAsync();

            return banks;
        }

        public async Task<IEnumerable<SelectModel>> GetBankSelectListByAccountUnitAsync(int accountUnitId, CancellationToken cancellationToken)
        {
            var banks = await dbContext.Banks
                .AsNoTracking()
                .Where(b => b.AccountUnitId == accountUnitId && !b.IsDeleted)
                .Select(b => new SelectModel
                {
                    Id = b.Id,
                    Name = b.Name,
                }).ToListAsync();

            return banks;
        }

        public async Task<bool> HasBankAsync(string bankName)
        {
            var bank = await dbContext.Banks
                .Where(b => b.Name.ToLower() == bankName.ToLower() && !b.IsDeleted)
                .FirstOrDefaultAsync();
            
            if(bank == null)
                return false;   
            return true;
        }
    }
}