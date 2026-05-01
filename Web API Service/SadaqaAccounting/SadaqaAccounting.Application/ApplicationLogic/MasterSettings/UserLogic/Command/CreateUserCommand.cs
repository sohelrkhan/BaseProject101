namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.UserLogic.Command
{
    public class CreateUserCommand : UserCreateModel, IRequest<bool>
    {
        public class Handler : IRequestHandler<CreateUserCommand, bool>
        {
            private readonly UserManager<User> _userManager;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(UserManager<User> userManager, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _userManager = userManager;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<bool> Handle(CreateUserCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's ID from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user ID is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Check, employee and buyer id is null
                if (request.EmployeeId is null && request.BuyerId is null)
                    return false;

                // Check, email is exist or not
                var existEmail = await _userManager.Users.Where(u => u.Email == request.Email).FirstOrDefaultAsync();

                if (existEmail is not null)
                    throw new Exception("Email already exist! Try new one.");

                var registerUser = _mapper.Map<User>(request);
                registerUser.UserName = request.Email;
                registerUser.Email = request.Email;
                registerUser.FullName = request.FullName;
                registerUser.LockoutEnabled = false;
                registerUser.ForcePasswordChanged = false;

                // Set employee or buyer id
                if (request.EmployeeId is not null)
                    request.EmployeeId = request.EmployeeId;

                if(request.BuyerId is not null)
                    request.BuyerId = request.BuyerId;

                var result = _userManager.CreateAsync(registerUser, request.Password);
                var registerCompleteUser = _mapper.Map<UserCreateModel>(registerUser);

                if (result.Result.Succeeded)
                    return true;
                else
                    throw new Exception(result.Result.Errors.Select(s => s.Description).FirstOrDefault());
            }
        }
    }
}