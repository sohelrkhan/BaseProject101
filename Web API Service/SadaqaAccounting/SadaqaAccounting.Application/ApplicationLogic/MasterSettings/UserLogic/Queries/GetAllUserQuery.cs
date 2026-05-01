namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.UserLogic.Queries
{
    public class GetAllUserQuery : PaginationRequest, IRequest<PaginatedResponse<UserGridModel>>
    {
        public class Handler : IRequestHandler<GetAllUserQuery, PaginatedResponse<UserGridModel>>
        {
            private readonly UserManager<User> _userManager;
            private readonly IEmployeeRepository _employeeRepository;
            private readonly IMapper _mapper;

            public Handler(UserManager<User> userManager, IMapper mapper, IEmployeeRepository employeeRepository)
            {
                _userManager = userManager;
                _employeeRepository = employeeRepository;
                _mapper = mapper;
            }

            public async Task<PaginatedResponse<UserGridModel>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
            {
                // Get all users
                var users = _userManager.Users.AsNoTracking();

                // Apply filtering
                var filter = request.SearchTerm?.Trim();
                if (!string.IsNullOrWhiteSpace(filter) && !string.IsNullOrEmpty(filter))
                {
                    // SQL LIKE: %filter%
                    var pattern = $"%{filter}%";
                    users = users.Where(u => (u.FullName != null && EF.Functions.Like(u.FullName, pattern)) || (u.Email != null && EF.Functions.Like(u.Email, pattern)));
                }

                // Apply sorting
                var sortableColumns = new Dictionary<string, Expression<Func<User, object>>>(StringComparer.OrdinalIgnoreCase)
                {
                    ["name"] = u => u.FullName!,
                    ["email"] = u => u.Email!,
                    ["id"] = u => u.Id
                };

                var sortColumn = request.SortField?.Trim();
                var sortOrder = request.SortOrder?.Trim();

                if (!string.IsNullOrWhiteSpace(sortColumn) && sortableColumns.TryGetValue(sortColumn, out var sortExpression))
                    users = string.Equals(sortOrder, "descend", StringComparison.OrdinalIgnoreCase)
                        ? users.OrderByDescending(sortExpression)
                        : users.OrderBy(sortExpression);
                else
                    users = users.OrderBy(u => u.Id);

                // Get total count before paging
                var totalCount = await users.CountAsync(cancellationToken);

                // Apply paging
                var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
                var pageIndex = request.Page < 0 ? 0 : request.Page;

                // Retrieve paged result
                var result = await users
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize)
                    .ProjectTo<UserGridModel>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                // Prepare paginated response
                var paginatedResponse = new PaginatedResponse<UserGridModel>
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
}