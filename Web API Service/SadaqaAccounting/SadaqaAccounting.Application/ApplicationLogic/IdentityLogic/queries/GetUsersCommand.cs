namespace SadaqaAccounting.Application.ApplicationLogic.IdentityLogic.queries
{
    public class GetUsersCommand : IRequest<ICollection<UserModel>>
    {
        public int CompanyId { get;set; }

        public class Handler : IRequestHandler<GetUsersCommand, ICollection<UserModel>>
        {
            private readonly UserManager<User> _userManager;
            private readonly IOptions<AppSettings> _appSeeting;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IEmployeeRepository _employeeRepository;

            public Handler(UserManager<User> userManager, IOptions<AppSettings> appSeeting, IMapper mapper, IHttpContextAccessor httpContextAccessor, IEmployeeRepository employeeRepository)
            {
                _userManager = userManager;
                _appSeeting = appSeeting;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
                _employeeRepository = employeeRepository;
        }

            public async Task<ICollection<UserModel>> Handle(GetUsersCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Step 1: Get all users who need password reset and are linked with employees
                var existUsers = await _userManager.Users
                    .AsNoTracking()
                    .Where(x => x.ForcePasswordChanged && x.EmployeeId != null)
                    .ToListAsync(cancellationToken);

                // Step 2: Apply filter only if necessary
                if (request.CompanyId != -1)
                {
                    // Collect employee IDs
                    var employeeIds = existUsers
                        .Select(u => (int)u.EmployeeId!)
                        .Distinct()
                        .ToList();

                    // Bulk fetch employees instead of calling one-by-one
                    var employees = await _employeeRepository.GetAllAsync();

                    // Build dictionary for quick lookup
                    var employeeDict = employees.ToDictionary(e => e.Id);

                    // Filter users in-memory
                    existUsers = existUsers.Where(user =>
                    {
                        if (user.EmployeeId == null) return false;

                        var employee = employeeDict[user.EmployeeId.Value];

                        // Company filter
                        if (request.CompanyId != null && request.CompanyId != -1 && employee.CompanyId != request.CompanyId)
                        {
                            return false;
                        }

                        return true;
                    }).ToList();
                }

                var mapUser = _mapper.Map<ICollection<UserModel>>(existUsers);
                return mapUser;
            }
        }
    }
}