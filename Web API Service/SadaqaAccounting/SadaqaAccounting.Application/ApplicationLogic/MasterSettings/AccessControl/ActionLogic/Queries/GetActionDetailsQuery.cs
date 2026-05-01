namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccessControl.ActionLogic.Queries
{
    public class GetActionDetailsQuery : IRequest<ActionUpdateModel>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<GetActionDetailsQuery, ActionUpdateModel>
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

            public async Task<ActionUpdateModel> Handle(GetActionDetailsQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Check if feature id is null
                if (string.IsNullOrWhiteSpace(request.Id) || string.IsNullOrEmpty(request.Id) || request.Id == "-1")
                    return new ActionUpdateModel();

                // Decrypt action id
                var decryptActionId = EncryptionService.Decrypt(request.Id);

                // Check if action decrypt id is null
                if (string.IsNullOrWhiteSpace(decryptActionId) || string.IsNullOrEmpty(decryptActionId))
                    return new ActionUpdateModel(); 

                // Convert decrypt action id
                var convertFeatureId = Convert.ToInt32(decryptActionId);

                var getExistActon = await _actionRepository.GetByIdAsync(convertFeatureId);

                if (getExistActon is null)
                    return new ActionUpdateModel();

                var mapExitAction = _mapper.Map<ActionUpdateModel>(getExistActon);
                return mapExitAction;
            }
        }
    }
}