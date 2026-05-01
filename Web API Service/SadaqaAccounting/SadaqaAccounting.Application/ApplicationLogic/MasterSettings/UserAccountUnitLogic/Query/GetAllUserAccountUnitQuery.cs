using SadaqaAccounting.Application.ApplicationLogic.MasterSettings.UserAccountUnitLogic.Model;

namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.UserAccountUnitLogic.Query
{
    public class GetAllUserAccountUnitQuery : IRequest<ICollection<UserAccountUnitGridModel>>
    {
        public class Handler : IRequestHandler<GetAllUserAccountUnitQuery, ICollection<UserAccountUnitGridModel>>
        {
            private readonly IUserAccountUnitRepository _userAccountUnitRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IUserAccountUnitRepository userAccountUnitRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _userAccountUnitRepository = userAccountUnitRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<ICollection<UserAccountUnitGridModel>> Handle(GetAllUserAccountUnitQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getUserAccountUnits = await _userAccountUnitRepository.GetAllAsync();
                var mapGetUserAccountUnits = _mapper.Map<ICollection<UserAccountUnitGridModel>>(getUserAccountUnits);

                return mapGetUserAccountUnits;
            }
        }
    }
}
