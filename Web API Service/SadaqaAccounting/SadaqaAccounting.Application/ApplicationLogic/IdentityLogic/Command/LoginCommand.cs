namespace SadaqaAccounting.Application.ApplicationLogic.IdentityLogic.Command
{
    public class LoginCommand : LoginModel, IRequest<UserModel>
    {
        public class Handler : IRequestHandler<LoginCommand, UserModel>
        {
            private readonly UserManager<User> _userManager;
            private readonly IOptions<AppSettings> _appSeeting;
            private readonly IMapper _mapper;
            private readonly IUserAccessMappingRepository _userAccessMappingRepository;
            private readonly IEmployeeRepository _employeeRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IUserLoginHistoryRepository _userLoginHistoryRepository;

            public Handler(UserManager<User> userManager, IOptions<AppSettings> appSeeting, IMapper mapper, IUserAccessMappingRepository userAccessMappingRepository,
                IEmployeeRepository employeeRepository, IHttpContextAccessor httpContextAccessor, IUserLoginHistoryRepository userLoginHistoryRepository)
            {
                _userManager = userManager;
                _userAccessMappingRepository = userAccessMappingRepository;
                _appSeeting = appSeeting;
                _mapper = mapper;
                _employeeRepository = employeeRepository;
                _httpContextAccessor = httpContextAccessor;
                _userLoginHistoryRepository = userLoginHistoryRepository;
            }

            public async Task<UserModel> Handle(LoginCommand request, CancellationToken cancellationToken)
            {
                var existUser = await _userManager.FindByEmailAsync(request.Email);

                // Check disable login access for this employee
                if (existUser is not null && existUser.LockoutEnd is not null)
                    throw new Exception("Login access is currently disabled. Please reach out to the administrator for support.");

                if (existUser is not null && await _userManager.CheckPasswordAsync(existUser, request.Password))
                {
                    var mapExistUser = _mapper.Map<UserModel>(existUser);

                    // Check employee id exist or not
                    if (mapExistUser.EmployeeId is not null && mapExistUser.EmployeeId > 0)
                    {
                        // Get selected employee info
                        var selectedEmployeeInfo = await _employeeRepository.GetByIdAsync((int)mapExistUser.EmployeeId);

                        // Get selected employee gander id
                        mapExistUser.EmployeeEncryptedId = EncryptionService.Encrypt(mapExistUser.EmployeeId.ToString());

                        // Check, employee have image
                        if(selectedEmployeeInfo.Image is not null)
                            mapExistUser.Image = selectedEmployeeInfo.Image;                       
                    }
                    else
                        mapExistUser.CompanyId = null;

                    var signInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSeeting.Value.JWTSecret));
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name, existUser.Id.ToString()),
                            new Claim("UserName", existUser.UserName!.ToString()),
                            new Claim("FullName", existUser.FullName!.ToString()),
                            new Claim("EmployeeId",(existUser.EmployeeId == null) ? 0.ToString() : existUser.EmployeeId.ToString()!),
                            new Claim("CompanyId",(mapExistUser.CompanyId == null) ? 0.ToString() : mapExistUser.CompanyId.ToString()!),
                            new Claim("ForcePasswordChanged",(existUser.ForcePasswordChanged == null) ? "false": "true")
                        }),

                        Expires = DateTime.UtcNow.AddHours(10),
                        SigningCredentials = new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256Signature)
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                    var token = tokenHandler.WriteToken(securityToken);

                    // Set login user token
                    mapExistUser.Token = token;

                    // Get login user permission
                    var userPermissions = await _userAccessMappingRepository.GetUserWiseAccessAsync(existUser.Id);
                    var mapUserPermissions = _mapper.Map<ICollection<UserAccessMappingDetails>>(userPermissions);

                    if (mapUserPermissions is not null)
                        mapExistUser.UserAccessMappingDetails = mapUserPermissions;

                    //Get User IP Address
                    var clientLoginIp = IPHelper.GetIpAddress(_httpContextAccessor.HttpContext!);

                    //Save User IP Address in DB
                    var createUserLoginHistoryModel = new UserLoginHistory
                    {
                        UserId = existUser.Id,
                        LoginIp = clientLoginIp,
                        LoginDateTime = DateTime.UtcNow
                    };

                    var createUserLoginHistory =
                        await _userLoginHistoryRepository.CreateAsync(createUserLoginHistoryModel);

                    return mapExistUser;
                }

                throw new Exception("Email and password cannot matched! Please, try again.");
            }
        }
    }
}