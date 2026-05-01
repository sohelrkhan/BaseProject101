namespace SadaqaAccounting.Repository.Repository.MasterSettings
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(DatabaseContext databaseContext) : base(databaseContext) { }

        public override async Task<ICollection<Employee>> GetAllAsync()
        {
            var getEmployess = await dbContext.Employees
                .AsNoTracking()
                .Where(e => !e.IsDeleted)
                .Include(e => e.Status)
                .ToListAsync();

            return getEmployess;
        }

        public override async Task<Employee?> GetByIdAsync(int id)
        {
            var getEmployee = await dbContext.Employees
                .Where(e => e.Id == id && !e.IsDeleted)
                .FirstOrDefaultAsync();

            return getEmployee!;
        }

        public async Task<IQueryable<Employee>> GetAllEmployeeFilterByIdQuery(int? companyId, int? statusId)
        {
            // Get employees
            var employees = dbContext.Employees
                .Where(e => !e.IsDeleted)
                .Include(e => e.Status)
                .AsNoTracking()
                .AsQueryable();

            // Apply company id filter 
            if (companyId.HasValue && companyId.Value != -1)
                employees = employees.Where(e => e.CompanyId == companyId);           

            // Apply status id filter
            if (statusId.HasValue && statusId.Value != -1)
                employees = employees.Where(e => e.StatusId == statusId);

            return employees;
        }

        public async Task<string> GetEmployeeNameByEmployeeIdsAsync(int[] employeeIds)
        {
            var employeeNames = await dbContext.Employees
                .Where(e => employeeIds.Contains(e.Id) && !e.IsDeleted)
                .Select(e => e.FullName)
                .ToListAsync();

            return string.Join(", ", employeeNames);
        }

        public async Task<ICollection<SelectModel>> EmployeeSelectListAsync(CancellationToken cancellationToken)
        {
            var employees = await dbContext.Employees
                .AsNoTracking()
                .Where(e => !e.IsDeleted)
                .Select(e => new SelectModel
                {
                    Id = e.Id,
                    Name = e.FullName
                })
                .ToListAsync(cancellationToken);

            return employees;
        }

        public async Task<ICollection<SelectModel>> EmployeeSelectListByCompanyIdAsync(int companyId, CancellationToken cancellationToken)
        {
            var employees = await dbContext.Employees
                .AsNoTracking()
                .Where(e => !e.IsDeleted && e.CompanyId == companyId)
                .Select(e => new SelectModel
                {
                    Id = e.Id,
                    Name = e.FullName
                })
                .ToListAsync(cancellationToken);

            return employees;
        }

        public async Task<bool> IsEmailExistAsync(string email)
        {
            var isExist = await dbContext.Employees
                .AsNoTracking()
                .AnyAsync(e => e.Email.ToLower() == email.ToLower() && !e.IsDeleted);

            return isExist;
        }

        public async Task<PaginatedResponse<Employee>> GetEmployeesFilterAsync(PaginationRequest paginationRequest, CancellationToken cancellationToken = default)
        {
            // Get employee
            var getEmployee = dbContext.Employees
                .AsNoTracking()
                .Include(e => e.Company)
                .Include(e => e.Status)
                .Where(e => !e.IsDeleted)
                .AsNoTracking();

            // Apply filtering
            var filterValue = paginationRequest.SearchTerm?.Trim();
            if (!string.IsNullOrWhiteSpace(filterValue) && !string.IsNullOrEmpty(filterValue))
            {
                var searchPattern = $"%{filterValue}%";
                getEmployee = getEmployee.Where(e => 
                (e.FullName != null && EF.Functions.Like(e.FullName, searchPattern))
                || (e.Email != null && EF.Functions.Like(e.Email, searchPattern))
                || (e.PhoneNumber != null && EF.Functions.Like(e.PhoneNumber, searchPattern)));
            }

            // Apply sorting
            var sortableColumns = new Dictionary<string, Expression<Func<Employee, object>>>(StringComparer.OrdinalIgnoreCase)
            {
                ["fullName"] = u => u.FullName,
                ["email"] = u => u.Email,
                ["companyName"] = u => u.Company.Name,
                ["id"] = u => u.Id
            };

            var sortColumn = paginationRequest.SortField?.Trim();
            var sortOrder = paginationRequest.SortOrder?.Trim();

            if (!string.IsNullOrWhiteSpace(sortColumn) && sortableColumns.TryGetValue(sortColumn, out var sortExpression))
                getEmployee = string.Equals(sortOrder, "descend", StringComparison.OrdinalIgnoreCase) ? getEmployee.OrderByDescending(sortExpression) : getEmployee.OrderBy(sortExpression);
            else
                getEmployee = getEmployee.OrderBy(u => u.Id);

            // Get total count before paging
            var totalCount = await getEmployee.CountAsync(cancellationToken);

            // Apply paging
            var pageSize = paginationRequest.PageSize <= 0 ? 10 : paginationRequest.PageSize;
            var pageIndex = paginationRequest.Page < 0 ? 0 : paginationRequest.Page;

            // Retrieve paged result
            var result = await getEmployee
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            // Prepare paginated response
            var paginatedResponse = new PaginatedResponse<Employee>
            {
                Data = result,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageSize = pageSize,
                CurrentPage = pageIndex,
                TotalRecords = totalCount,
            };

            return paginatedResponse;
        }
    }
}