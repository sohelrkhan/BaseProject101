namespace SadaqaAccounting.Application.ApplicationLogic.IdentityLogic.Command
{
    public class SelectAccountUnitCommand : IRequest<UserModel>
    {
        public int AccountUnitId { get; set; }

        public class Handler : IRequestHandler<SelectAccountUnitCommand, UserModel>
        {
            private readonly UserManager<User> _userManager;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IMapper _mapper;
            private readonly IUserAccessMappingRepository _userAccessMappingRepository;
            private readonly IUserAccountUnitRepository _userAccountUnitRepository;

            private readonly IOptions<AppSettings> _appSeeting;

            public Handler(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, IMapper mapper, IUserAccessMappingRepository userAccessMappingRepository,
                IUserAccountUnitRepository userAccountUnitRepository, IOptions<AppSettings> appSeeting)
            {
                _userManager = userManager;
                _httpContextAccessor = httpContextAccessor;
                _mapper = mapper;
                _userAccessMappingRepository = userAccessMappingRepository;
                _userAccountUnitRepository = userAccountUnitRepository;
                _appSeeting = appSeeting;
            }

            public async Task<UserModel> Handle(SelectAccountUnitCommand request, CancellationToken cancellationToken)
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
                if (string.IsNullOrWhiteSpace(userId))
                    throw new Exception("Unauthorized.");

                var user = await _userManager.FindByIdAsync(userId);
                if (user is null)
                    throw new Exception("User not found.");

                var hasAccess = await _userAccountUnitRepository.UserHasAccountUnitAsync(userId, request.AccountUnitId, cancellationToken);
                if (!hasAccess)
                    throw new Exception("You don't have access to this account unit.");

                var model = _mapper.Map<UserModel>(user);
                model.AccountUnitId = request.AccountUnitId;

                var userPermissions = await _userAccessMappingRepository.GetUserWiseAccessAsync(user.Id);
                model.UserAccessMappingDetails = _mapper.Map<ICollection<UserAccessMappingDetails>>(userPermissions);

                // Issue NEW token with AccountUnitId claim
                model.Token = GenerateJwt(user, model.CompanyId, model.AccountUnitId);

                return model;
            }

            private string GenerateJwt(User user, int? companyId, int? accountUnitId)
            {
                var signInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSeeting.Value.JWTSecret));
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Id),
                    new Claim("UserName", user.UserName ?? ""),
                    new Claim("FullName", user.FullName ?? ""),
                    new Claim("EmployeeId", (user.EmployeeId ?? 0).ToString()),
                    new Claim("CompanyId", (companyId ?? 0).ToString()),
                    new Claim("AccountUnitId", (accountUnitId ?? 0).ToString()),
                    new Claim("ForcePasswordChanged", user.ForcePasswordChanged == false ? "false" : "true"),
                };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(10),
                    SigningCredentials = new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
            }
        }
    }
}