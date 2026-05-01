namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.EventLogic.Command
{
    public class DeleteEventCommand: IRequest<bool>
    {
        public int Id { get; set; }

        public class Handler : IRequestHandler<DeleteEventCommand, bool>
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

            public async Task<bool> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
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

                var isDeleteEvent = false;
                if (getExistEvent is not null)
                {
                    getExistEvent.IsDeleted = true;
                    getExistEvent.DeletedDateTime = DateTime.UtcNow;

                    var updatedLeavePolicy = await _eventRepository.UpdateAsync(getExistEvent);
                    isDeleteEvent = true;
                }

                return isDeleteEvent;
            }
        }
    }
}