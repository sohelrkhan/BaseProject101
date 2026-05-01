namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.ReportAccessControl.ReportUserAccessLogic.Queries
{
    public class GetAllReportUserAccessGroupByUserIdQuery : IRequest<ICollection<ReportUserAccessGridModel>>
    {
        public class Handler : IRequestHandler<GetAllReportUserAccessGroupByUserIdQuery, ICollection<ReportUserAccessGridModel>>
        {
            private readonly IReportUserAccessRepository _reportUserAccessRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IReportUserAccessRepository reportUserAccessRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _reportUserAccessRepository = reportUserAccessRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<ICollection<ReportUserAccessGridModel>> Handle(GetAllReportUserAccessGroupByUserIdQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's ID from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user ID is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getReportUserAccesses = await _reportUserAccessRepository.GetAllAsync();
                var mapReportUserAccesses = _mapper.Map<ICollection<ReportUserAccessGridModel>>(getReportUserAccesses);

                var groupedByUserId = mapReportUserAccesses
                    .GroupBy(x => new { x.UserId, x.UserName })
                    .Select(g => new ReportUserAccessGridModel
                    {
                        UserId = g.Key.UserId,
                        UserName = g.Key.UserName
                    })
                    .ToList();

                return groupedByUserId;
            }
        }
    }
}