using SadaqaAccounting.Application.ApplicationLogic.MasterSettings.EventLogic.Model;

namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.EventLogic.Queries
{
    public class GetEventDetailsQuery: IRequest<EventUpdateModel>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<GetEventDetailsQuery, EventUpdateModel>
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

            public async Task<EventUpdateModel> Handle(GetEventDetailsQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Check if module id is null
                if (string.IsNullOrWhiteSpace(request.Id) || string.IsNullOrEmpty(request.Id) || request.Id == "-1")
                    return new EventUpdateModel();

                // Decrypt event id
                var decryptEventId = EncryptionService.Decrypt(request.Id);

                // Check if module decrypt id is null
                if (string.IsNullOrWhiteSpace(decryptEventId) || string.IsNullOrEmpty(decryptEventId))
                    return new EventUpdateModel();

                // Convert decrypt event id
                var convertEventId = Convert.ToInt32(decryptEventId);
                var getExistEvent = await _eventRepository.GetByIdAsync(convertEventId);

                if (getExistEvent is null)
                    return new EventUpdateModel();

                var mapExitEvent = _mapper.Map<EventUpdateModel>(getExistEvent);
                // Date conversion
                if (mapExitEvent?.StartDate != null)
                    mapExitEvent.StartDateString = Convert.ToDateTime(mapExitEvent?.StartDate).ToString("dd-MM-yyyy");
                if (mapExitEvent?.EndDate != null)
                    mapExitEvent.EndDateString = Convert.ToDateTime(mapExitEvent?.EndDate).ToString("dd-MM-yyyy");

                return mapExitEvent!;
            }
        }
    }
}
