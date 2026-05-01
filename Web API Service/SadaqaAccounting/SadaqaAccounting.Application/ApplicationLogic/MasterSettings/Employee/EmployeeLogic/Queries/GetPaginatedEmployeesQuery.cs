namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.Employee.EmployeeLogic.Queries
{
    public class GetPaginatedEmployeesQuery : PaginationRequest, IRequest<PaginatedResponse<EmployeeGridModel>>
    {
        public class Handler : IRequestHandler<GetPaginatedEmployeesQuery, PaginatedResponse<EmployeeGridModel>>
        {
            private readonly IEmployeeRepository _employeeRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly UserManager<User> _userManager;

            public Handler(IEmployeeRepository employeeRepository, IMapper mapper,IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
            {
                _employeeRepository = employeeRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
                _userManager = userManager;
            }

            public async Task<PaginatedResponse<EmployeeGridModel>> Handle(GetPaginatedEmployeesQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);
                
                // Get employees and map 
                var getEmployees = await _employeeRepository.GetEmployeesFilterAsync(request, cancellationToken);
                var mapGetEmployee = _mapper.Map<ICollection<EmployeeGridModel>>(getEmployees.Data);

                // Get users and employee ids
                var employeeIdSet = (await _userManager.Users
                        .AsNoTracking()
                        .Where(u => u.EmployeeId != null)
                        .Select(u => u.EmployeeId!.Value)
                        .Distinct()
                        .ToListAsync(cancellationToken))
                    .ToHashSet();

                var result = new PaginatedResponse<EmployeeGridModel>
                {
                    Data = mapGetEmployee,
                    CurrentPage = getEmployees.CurrentPage,
                    TotalPages = getEmployees.TotalPages,
                    TotalRecords = getEmployees.TotalRecords,
                    PageSize = getEmployees.PageSize
                };

                foreach (var employee in result.Data)
                {
                    if (employeeIdSet.Contains(employee.Id))
                    {
                        employee.HasUserId = true;
                    }
                }

                return result;
            }
        }
    }
}