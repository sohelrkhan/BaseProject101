namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.EventLogic.Queries
{
    public class SelectListEventQuery: IRequest<IEnumerable<SelectModel>>
    {
        public class Handler : IRequestHandler<SelectListEventQuery, IEnumerable<SelectModel>>
        {
            private readonly IEventRepository _eventRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IEventRepository eventRepository, IHttpContextAccessor httpContextAccessor)
            {
                _eventRepository = eventRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<IEnumerable<SelectModel>> Handle(SelectListEventQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Retrieve the user's accountUnitId from the current HTTP context
                var AccountUnitId = int.Parse(_httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value!);
                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var events = await _eventRepository.GetEventsByAccountUnitIdAsync(AccountUnitId);

                var getEvents = events
                .OrderBy(m => m.Name)
                .Select(m => new SelectModel
                {
                    Id = m.Id,
                    Name = m.Name,
                });
                return getEvents;
            }
        }
    }
}
