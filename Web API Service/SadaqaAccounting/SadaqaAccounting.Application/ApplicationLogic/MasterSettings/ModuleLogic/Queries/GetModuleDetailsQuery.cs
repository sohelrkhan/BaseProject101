namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.ModuleLogic.Queries
{
    public class GetModuleDetailsQuery : IRequest<ModuleUpdateModel>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<GetModuleDetailsQuery, ModuleUpdateModel>
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

            public async Task<ModuleUpdateModel> Handle(GetModuleDetailsQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Check if module id is null
                if (string.IsNullOrWhiteSpace(request.Id) || string.IsNullOrEmpty(request.Id) || request.Id == "-1")
                    return new ModuleUpdateModel();

                // Decrypt module id
                var decryptModuleId = EncryptionService.Decrypt(request.Id);

                // Check if module decrypt id is null
                if (string.IsNullOrWhiteSpace(decryptModuleId) || string.IsNullOrEmpty(decryptModuleId))
                    return new ModuleUpdateModel();

                // Convert decrypt module id
                var convertModuleId = Convert.ToInt32(decryptModuleId);
                var getExistModule = await _moduleRepository.GetByIdAsync(convertModuleId);

                if (getExistModule is null)
                    return new ModuleUpdateModel();

                var mapExitModule = _mapper.Map<ModuleUpdateModel>(getExistModule);
                return mapExitModule;
            }
        }
    }
}