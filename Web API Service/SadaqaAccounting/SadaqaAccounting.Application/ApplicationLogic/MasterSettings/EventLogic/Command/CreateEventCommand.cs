namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.EventLogic.Command
{
    public class CreateEventCommand: EventCreateModel, IRequest<EventCreateModel>
    {
        public class Handler : IRequestHandler<CreateEventCommand, EventCreateModel>
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

            public async Task<EventCreateModel> Handle(CreateEventCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's accountUnitId from the current HTTP context
                var accountUnitId = _httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value;

                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var createdEvent = _mapper.Map<Event>(request);
                // Date conversion
                if (!string.IsNullOrEmpty(request.StartDateString))
                {
                    createdEvent.StartDate = DateTime.ParseExact(request.StartDateString!, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(request.EndDateString))
                {
                    createdEvent.EndDate = DateTime.ParseExact(request.EndDateString!, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }
                createdEvent.AccountUnitId = int.Parse(accountUnitId!);
                createdEvent.CreatedById = userId;
                createdEvent.CreatedDateTime = DateTime.UtcNow;
                createdEvent = await _eventRepository.CreateAsync(createdEvent);

                return request;
            }
        }
    }
}
