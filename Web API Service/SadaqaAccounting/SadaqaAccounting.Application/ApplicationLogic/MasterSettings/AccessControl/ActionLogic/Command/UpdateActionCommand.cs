namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccessControl.ActionLogic.Command
{
    public class UpdateActionCommand : ActionUpdateModel, IRequest<ActionUpdateModel>
    {
        public class Handler : IRequestHandler<UpdateActionCommand, ActionUpdateModel>
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

            public async Task<ActionUpdateModel> Handle(UpdateActionCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Get exist action
                var getExistAction = await _actionRepository.GetByIdAsync(request.Id);

                if (getExistAction is null)
                    throw new BadRequestException(ProvideErrorMessage.ActionIdNotFound);

                getExistAction = _mapper.Map((ActionUpdateModel)request, getExistAction);
                getExistAction = await _actionRepository.UpdateAsync(getExistAction);

                return request;
            }
        }
    }
}