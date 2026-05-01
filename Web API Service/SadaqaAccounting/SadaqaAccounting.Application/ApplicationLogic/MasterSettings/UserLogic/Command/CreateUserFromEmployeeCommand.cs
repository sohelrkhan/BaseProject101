namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.UserLogic.Command
{
    public class CreateUserFromEmployeeCommand : UserCreateFromEmployeeModel,IRequest<UserCreateFromEmployeeModel>
    {
        public class Handler : IRequestHandler<CreateUserFromEmployeeCommand, UserCreateFromEmployeeModel>
        {
            private readonly UserManager<User> _userManager;
            private readonly IEmployeeRepository _employeeRepository;
            private readonly IMapper _mapper;

            public Handler(UserManager<User> userManager, IEmployeeRepository employeeRepository, IMapper mapper)
            {
                _userManager = userManager;
                _employeeRepository = employeeRepository;
                _mapper = mapper;
            }

            public async Task<UserCreateFromEmployeeModel> Handle(CreateUserFromEmployeeCommand request, CancellationToken cancellationToken)
            {
                // Get employee details by id
                var employeeDetails = await _employeeRepository.GetByIdAsync(request.EmployeeId);

                var requestUserModel = new UserCreateModel
                {
                    Email = employeeDetails.Email!,
                    FullName = employeeDetails.FullName,
                    Password = "Admin@123456" // password should be auto generated and send to employee office email
                };

                var existUser = await _userManager.Users.Where(u => u.Email == requestUserModel.Email).FirstOrDefaultAsync();

                if (existUser is not null)
                    throw new Exception("Email already exist! Try new one.");

                var registerUser = _mapper.Map<User>(requestUserModel);
                registerUser.UserName = requestUserModel.Email;
                registerUser.Email = requestUserModel.Email;
                registerUser.FullName = requestUserModel.FullName;
                registerUser.EmployeeId = request.EmployeeId;
                registerUser.LockoutEnabled = false;
                registerUser.ForcePasswordChanged = false;
                registerUser.ApplicationUserTypeId = ApplicationUserType.Employee;

                var result = _userManager.CreateAsync(registerUser, requestUserModel.Password);
                var registerCompleteUser = _mapper.Map<UserCreateModel>(registerUser);

                if (result.Result.Succeeded)
                    return request;
                else
                    throw new Exception(result.Result.Errors.Select(s => s.Description).FirstOrDefault());
            }
        }
    }
}