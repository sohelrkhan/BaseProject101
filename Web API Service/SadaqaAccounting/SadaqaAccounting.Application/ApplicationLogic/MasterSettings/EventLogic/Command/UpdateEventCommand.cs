namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.EventLogic.Command
{
    public class UpdateEventCommand: EventUpdateModel, IRequest<EventUpdateModel>
    {
        public class Handler : IRequestHandler<UpdateEventCommand, EventUpdateModel>
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

            public async Task<EventUpdateModel> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Get exist event
                var getExistEvent = await _eventRepository.GetByIdAsync(request.Id);

                if (getExistEvent is null)
                    throw new BadRequestException(ProvideErrorMessage.EventNotFound);

                getExistEvent = _mapper.Map((EventUpdateModel)request, getExistEvent);

                getExistEvent.AccountUnitId = int.Parse(_httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value!);

                // Date conversion
                if (!string.IsNullOrEmpty(request.StartDateString))
                {
                    getExistEvent.StartDate = DateTime.ParseExact(request.StartDateString!, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(request.EndDateString))
                {
                    getExistEvent.EndDate = DateTime.ParseExact(request.EndDateString!, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }
                getExistEvent = await _eventRepository.UpdateAsync(getExistEvent);

                return request;
            }
        }
    }
}