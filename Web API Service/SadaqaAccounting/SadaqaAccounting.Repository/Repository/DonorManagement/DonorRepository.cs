namespace SadaqaAccounting.Repository.Repository.DonorManagement
{
    public class DonorRepository : BaseRepository<Donor>, IDonorRepository
    {
        public DonorRepository(DatabaseContext context) : base(context) { }

        public async Task<ICollection<Donor>> GetAllAccountUnitWiseDonorAsync(int id)
        {
            var donors = await dbContext.Donors.Where(x => x.AccountUnitId == id && !x.IsDeleted).ToListAsync();
            return donors;
        }

        public async Task<ICollection<SelectModel>> GetSelectListDonorAsync(int id)
        {
            var donors = await dbContext.Donors.Where(x => x.AccountUnitId == id && !x.IsDeleted)
                .Select(s => new SelectModel
                {
                    Id = s.Id,
                    Name = s.Name
                }).ToListAsync();
            return donors;
        }
        public override async Task<Donor?> GetByIdAsync(int id)
        {
            var donor = await dbContext.Donors
                        .Include(i => i.Status)
                        .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            return donor;
        }

        public async Task<PaginatedResponse<Donor>> GetDonorsFilterAsync(PaginationRequest paginationRequest, CancellationToken cancellationToken = default)
        {
            // Get donors
            var donors = dbContext.Donors.AsNoTracking()
                .Include(i => i.Status)
                .Where(a => a.AccountUnitId == paginationRequest.AccountUnitId & !a.IsDeleted);

            // Apply filtering
            var filterValue = paginationRequest.SearchTerm?.Trim();
            if (!string.IsNullOrWhiteSpace(filterValue) && !string.IsNullOrEmpty(filterValue))
            {
                var searchPattern = $"%{filterValue}%";
                donors = donors.Where(a => (a.Name != null && EF.Functions.Like(a.Name, searchPattern)) || (a.MobileNo != null && EF.Functions.Like(a.MobileNo,searchPattern)));
            }

            // Apply sorting
            var sortableColumns = new Dictionary<string, Expression<Func<Donor, object>>>(StringComparer.OrdinalIgnoreCase)
            {
                ["name"] = u => u.Name,
                ["id"] = u => u.Id
            };

            var sortColumn = paginationRequest.SortField?.Trim();
            var sortOrder = paginationRequest.SortOrder?.Trim();

            if (!string.IsNullOrWhiteSpace(sortColumn) && sortableColumns.TryGetValue(sortColumn, out var sortExpression))
                donors = string.Equals(sortOrder, "descend", StringComparison.OrdinalIgnoreCase) ? donors.OrderByDescending(sortExpression) : donors.OrderBy(sortExpression);
            else
                donors = donors.OrderBy(u => u.Id);

            // Get total count before paging
            var totalCount = await donors.CountAsync(cancellationToken);

            // Apply paging
            var pageSize = paginationRequest.PageSize <= 0 ? 10 : paginationRequest.PageSize;
            var pageIndex = paginationRequest.Page < 0 ? 0 : paginationRequest.Page;

            // Retrieve paged result
            var result = await donors
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            // Prepare paginated response
            var paginatedResponse = new PaginatedResponse<Donor>
            {
                Data = result,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageSize = pageSize,
                CurrentPage = pageIndex,
                TotalRecords = totalCount,
            };

            return paginatedResponse;
        }

        public async Task<bool> CheckDuplicateDonorByNameAndMobileAsync(string name, string mobileNo)
        {
            bool response = false;
            var donor = await dbContext.Donors.FirstOrDefaultAsync(x => x.Name.Equals(name) && x.MobileNo.Equals(mobileNo) && !x.IsDeleted);
            if (donor != null)
            {
                response = true;
            }
            return response;
        }

        public async Task<string> GetUniqueDonorCodeAsync()
        {
            const string prefix = "DNR-";

            // Get the last donor code (highest)
            var lastDonorCode = await dbContext.Donors
                .Where(x => !x.IsDeleted && x.Code.StartsWith(prefix))
                .OrderByDescending(x => x.Code)
                .Select(x => x.Code)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(lastDonorCode))
            {
                return $"{prefix}000001";
            }

            // Extract numeric part
            var numberPart = lastDonorCode.Replace(prefix, "");
            var lastNumber = int.Parse(numberPart);

            // Increment
            var newNumber = lastNumber + 1;

            // Format with leading zeros
            return $"{prefix}{newNumber:D6}";
        }

        public async Task<IEnumerable<SelectModel>> GetDonerSelectListByAccountUnitAsync(int accountUnitId, CancellationToken cancellationToken)
        {
            var doners = await dbContext.Donors
                .AsNoTracking()
                .Where(d => d.AccountUnitId == accountUnitId && !d.IsDeleted)
                .Select(s => new SelectModel
                {
                    Id = s.Id,
                    Name = s.Name,
                }).ToListAsync(cancellationToken);

            return doners;
        }
    }
}