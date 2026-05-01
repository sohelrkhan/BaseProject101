namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.ModuleLogic.Command
{
    public class CreateModuleCommand : ModuleCreateModel, IRequest<ModuleCreateModel>
    {
        public class Handler : IRequestHandler<CreateModuleCommand, ModuleCreateModel>
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

            public async Task<ModuleCreateModel> Handle(CreateModuleCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var createdModule = _mapper.Map<SadaqaAccounting.Model.Models.MasterSettings.Module>(request);
                createdModule.StatusId = GlobalStatus.Active;
                createdModule = await _moduleRepository.CreateAsync(createdModule);

                return request;
            }
        }
    }
}