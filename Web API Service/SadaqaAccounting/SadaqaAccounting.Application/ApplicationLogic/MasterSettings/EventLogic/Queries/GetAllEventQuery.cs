namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.EventLogic.Queries
{
    public class GetAllEventQuery: IRequest<List<EventGridModel>>
    {
        public class Handler : IRequestHandler<GetAllEventQuery, ICollection<EventGridModel>>
        {
            private readonly IEventRepository _eventRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IEventRepository eventRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _eventRepository = eventRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<ICollection<EventGridModel>> Handle(GetAllEventQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
                // Retrieve the user's accountUnitId from the current HTTP context
                var AccountUnitId = int.Parse(_httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value!);
                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getEvents = await _eventRepository.GetEventsByAccountUnitIdAsync(AccountUnitId);
                var mapGetEvents = _mapper.Map<ICollection<EventGridModel>>(getEvents);

                return mapGetEvents;
            }
        }
    }
}
