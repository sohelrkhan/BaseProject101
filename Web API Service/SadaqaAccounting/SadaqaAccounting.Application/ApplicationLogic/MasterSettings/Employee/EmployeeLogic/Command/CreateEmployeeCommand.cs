namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.Employee.EmployeeLogic.Command
{
    public class CreateEmployeeCommand : EmployeeCreateModel, IRequest<int>
    {
        public class Handler : IRequestHandler<CreateEmployeeCommand, int>
        {
            private readonly IEmployeeRepository _employeeRepository;
            private readonly UserManager<User> _userManager;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IEmployeeRepository employeeRepository, UserManager<User> userManager, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _employeeRepository = employeeRepository;
                _userManager = userManager; 
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<int> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);
              
                var createdEmployee = _mapper.Map<SadaqaAccounting.Model.Models.MasterSettings.Employee>(request);
                createdEmployee.CreatedById = userId;
                createdEmployee.CreatedDateTime = DateTime.UtcNow;
                createdEmployee.StatusId = GlobalStatus.Active;

                createdEmployee = await _employeeRepository.CreateAsync(createdEmployee);

                // Created employee assign in the user table
                var registerUser = new User();
                registerUser.UserName = request.Email;
                registerUser.Email = request.Email;
                registerUser.FullName = request.FullName;
                registerUser.EmployeeId = createdEmployee.Id;
                registerUser.ForcePasswordChanged = true;
                registerUser.ApplicationUserTypeId = ApplicationUserType.Employee;

                // Register employee as user
                await _userManager.CreateAsync(registerUser, "Employee@123");
                return createdEmployee.Id;
            }
        }
    }
}