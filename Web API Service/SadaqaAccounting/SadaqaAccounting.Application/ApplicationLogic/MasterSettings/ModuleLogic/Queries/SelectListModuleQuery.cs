namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.ModuleLogic.Queries
{
    public class SelectListModuleQuery : IRequest<IEnumerable<SelectModel>>
    {
        public class Handler : IRequestHandler<SelectListModuleQuery, IEnumerable<SelectModel>>
        {
            private readonly IModuleRepository _moduleRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IModuleRepository moduleRepository, IHttpContextAccessor httpContextAccessor)
            {
                _moduleRepository = moduleRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<IEnumerable<SelectModel>> Handle(SelectListModuleQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getModules = await _moduleRepository.GetModuleSelectListAsync();
                return getModules;
            }
        }
    }
}