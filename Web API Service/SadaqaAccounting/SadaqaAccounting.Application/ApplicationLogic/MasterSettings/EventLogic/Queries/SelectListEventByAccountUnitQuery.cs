namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.EventLogic.Queries
{
    public class SelectListEventByAccountUnitQuery : IRequest<IEnumerable<SelectModel>>
    {
        public int AccountUnitId { get; set; }

        public class Handler : IRequestHandler<SelectListEventByAccountUnitQuery, IEnumerable<SelectModel>>
        {
            private readonly IEventRepository _eventRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IEventRepository eventRepository, IHttpContextAccessor httpContextAccessor)
            {
                _eventRepository = eventRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<IEnumerable<SelectModel>> Handle(SelectListEventByAccountUnitQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var events = await _eventRepository.GetEventSelectListByAccountUnitAsync(request.AccountUnitId, cancellationToken);
                return events;
            }
        }
    }
}