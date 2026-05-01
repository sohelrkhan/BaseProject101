namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccessControl.ActionLogic.Command
{
    public class CreateActionCommand : ActionCreateModel, IRequest<ActionCreateModel>
    {
        public class Handler : IRequestHandler<CreateActionCommand, ActionCreateModel>
        {
            private readonly IActionRepository _actionRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IActionRepository actionRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _actionRepository = actionRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<ActionCreateModel> Handle(CreateActionCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var createdAction = _mapper.Map<SadaqaAccounting.Model.Models.MasterSettings.AccessControl.Action>(request);
                createdAction.StatusId = GlobalStatus.Active;

                createdAction = await _actionRepository.CreateAsync(createdAction);
                return request;
            }
        }
    }
}