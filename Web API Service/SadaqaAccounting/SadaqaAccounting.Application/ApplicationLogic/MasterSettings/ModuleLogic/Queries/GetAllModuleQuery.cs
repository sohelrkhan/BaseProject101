namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.ModuleLogic.Queries
{
    public class GetAllModuleQuery : IRequest<ICollection<ModuleGridModel>>
    {
        public class Handler : IRequestHandler<GetAllModuleQuery, ICollection<ModuleGridModel>>
        {
            private readonly IModuleRepository _moduleRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IModuleRepository moduleRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _moduleRepository = moduleRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<ICollection<ModuleGridModel>> Handle(GetAllModuleQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getModules = await _moduleRepository.GetAllAsync();
                var mapGetModules = _mapper.Map<ICollection<ModuleGridModel>>(getModules);

                return mapGetModules;
            }
        }
    }
}