namespace SadaqaAccounting.Application.ApplicationLogic.IdentityLogic.Command
{
    public class RegistrationCommand : RegisterModel, IRequest<UserModel>
    {
        public class Handler : IRequestHandler<RegistrationCommand, UserModel>
        {
            private readonly UserManager<User> _userManager;
            private readonly IMapper _mapper;

            public Handler(UserManager<User> userManager, IMapper mapper)
            {
                _userManager = userManager;
                _mapper = mapper;
            }

            public async Task<UserModel> Handle(RegistrationCommand request, CancellationToken cancellationToken)
            {
                var existUser = await _userManager.Users.Where(u => u.Email == request.Email).FirstOrDefaultAsync();

                if (existUser is not null)
                    throw new Exception("Email already exist! Try new one.");

                var registerUser = _mapper.Map<User>(request);
                registerUser.UserName = request.Email;
                registerUser.Email = request.Email;
                registerUser.FullName = request.FullName;

                var result = _userManager.CreateAsync(registerUser, request.Password);
                var registerCompleteUser = _mapper.Map<UserModel>(registerUser);

                if (result.Result.Succeeded)
                    return registerCompleteUser;
                else
                    throw new Exception(result.Result.Errors.Select(s => s.Description).FirstOrDefault());
            }
        }
    }
}