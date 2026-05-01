namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccessControl.ActionLogic.Queries
{
    public class GetAllActionQuery : PaginationRequest, IRequest<PaginatedResponse<ActionGridModel>>
    {
        public class Handler : IRequestHandler<GetAllActionQuery, PaginatedResponse<ActionGridModel>>
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

            public async Task<PaginatedResponse<ActionGridModel>> Handle(GetAllActionQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getActions = await _actionRepository.GetActionsFilterAsync(request, cancellationToken);
                var mapGetActions = _mapper.Map<ICollection<ActionGridModel>>(getActions.Data);

                var result = new PaginatedResponse<ActionGridModel>
                {
                    Data = mapGetActions,
                    CurrentPage = getActions.CurrentPage,
                    TotalPages = getActions.TotalPages,
                    TotalRecords = getActions.TotalRecords,
                    PageSize = getActions.PageSize
                };

                return result;
            }
        }
    }
}