namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.ModuleLogic.Command
{
    public class UpdateModuleCommand : ModuleUpdateModel, IRequest<ModuleUpdateModel>
    {
        public class Handler : IRequestHandler<UpdateModuleCommand, ModuleUpdateModel>
        {
            private readonly IModuleRepository _moduleRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IModuleRepository moduleManager, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _moduleRepository = moduleManager;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<ModuleUpdateModel> Handle(UpdateModuleCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Get exist module
                var getExistModule = await _moduleRepository.GetByIdAsync(request.Id);

                if (getExistModule is null)
                    throw new BadRequestException(ProvideErrorMessage.ModuleNotFound);

                getExistModule = _mapper.Map((ModuleUpdateModel)request, getExistModule);
                getExistModule = await _moduleRepository.UpdateAsync(getExistModule);

                return request;
            }
        }
    }
}