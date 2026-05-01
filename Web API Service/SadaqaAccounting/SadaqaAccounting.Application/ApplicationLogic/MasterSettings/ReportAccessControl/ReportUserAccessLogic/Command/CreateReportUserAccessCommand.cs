namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.ReportAccessControl.ReportUserAccessLogic.Command
{
    public class CreateReportUserAccessCommand : ReportUserAccessCreateModel, IRequest<ReportUserAccessCreateModel>
    {
        public class Handler : IRequestHandler<CreateReportUserAccessCommand, ReportUserAccessCreateModel>
        {
            private readonly IReportUserAccessRepository _reportUserAccessRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IReportUserAccessRepository reportUserAccessRepository, IHttpContextAccessor httpContextAccessor)
            {
                _reportUserAccessRepository = reportUserAccessRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<ReportUserAccessCreateModel> Handle(CreateReportUserAccessCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's ID from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user ID is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                //Check if Report User Access table has value
                var isExistAccess = await _reportUserAccessRepository.GetReportUserAccessesByUserAsync(request.UserId);

                foreach (var item in isExistAccess)
                {
                    // first delete all data
                    await _reportUserAccessRepository.DeleteAsync(item);
                }

                foreach (var item in request.ReportIds)
                {
                    // then insert 
                    var obj = new ReportUserAccess
                    {
                        ReportRegistryId = item,
                        UserId = request.UserId
                    };

                    var createdReportUserAccess = await _reportUserAccessRepository.CreateAsync(obj);
                }
                return request;
            }
        }
    }
}