namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.EventLogic.Queries
{
    public class GetAllEventFilterQuery: PaginationRequest, IRequest<PaginatedResponse<EventGridModel>>
    {
        public class Handler : IRequestHandler<GetAllEventFilterQuery, PaginatedResponse<EventGridModel>>
        {
            private readonly IEventRepository _eventRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IEventRepository _eventRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                this._eventRepository = _eventRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<PaginatedResponse<EventGridModel>> Handle(GetAllEventFilterQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
                // Retrieve the user's accountUnitId from the current HTTP context
                request.AccountUnitId = int.Parse(_httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value!);

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getEvents = await _eventRepository.GetEventsFilterAsync(request, cancellationToken);
                var mapEvents = _mapper.Map<ICollection<EventGridModel>>(getEvents.Data);

                var result = new PaginatedResponse<EventGridModel>
                {
                    Data = mapEvents,
                    CurrentPage = getEvents.CurrentPage,
                    TotalPages = getEvents.TotalPages,
                    TotalRecords = getEvents.TotalRecords,
                    PageSize = getEvents.PageSize
                };

                return result;
            }
        }
    }
}
