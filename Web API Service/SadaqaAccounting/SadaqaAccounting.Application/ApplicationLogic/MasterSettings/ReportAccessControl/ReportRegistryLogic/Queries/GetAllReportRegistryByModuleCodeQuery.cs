namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.ReportAccessControl.ReportRegistryLogic.Queries
{
    public class GetAllReportRegistryByModuleCodeQuery : IRequest<ICollection<GroupReportRegistryModel>>
    {
        public string ModuleCode { get; set; }

        public class Handler : IRequestHandler<GetAllReportRegistryByModuleCodeQuery, ICollection<GroupReportRegistryModel>>
        {
            private readonly IReportRegistryRepository _reportRegistryRepository;
            private readonly IMapper _mapper;
            private readonly IModuleRepository _moduleRepository;
            private readonly IReportUserAccessRepository _reportUserAccessRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IReportRegistryRepository reportRegistryRepository, IMapper mapper, IModuleRepository moduleRepository, IReportUserAccessRepository reportUserAccessRepository, 
                IHttpContextAccessor httpContextAccessor)
            {
                _reportRegistryRepository = reportRegistryRepository;
                _mapper = mapper;
                _moduleRepository = moduleRepository;
                _reportUserAccessRepository = reportUserAccessRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<ICollection<GroupReportRegistryModel>> Handle(GetAllReportRegistryByModuleCodeQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var returnList = new List<GroupReportRegistryModel>();

                var module = await _moduleRepository.GetModuleByCode(request.ModuleCode);
                var getReportRegistries = await _reportRegistryRepository.GetAllByModuleAsync(module.Id);
                var mapReportRegistries = _mapper.Map<ICollection<ReportRegistryGridModel>>(getReportRegistries);

                //Get report by User(Login user)
                var getReportUserAccesses = await _reportUserAccessRepository.GetReportUserAccessesByUserAsync(userId);

                // Filter Data by Report Id and Only permitted report will be in list
                 mapReportRegistries = mapReportRegistries
                    .Where(r => getReportUserAccesses.Any(u => u.ReportRegistryId == r.Id))
                    .ToList();

                foreach (var group in mapReportRegistries.GroupBy(g => new { g.ReportGroup }))
                {
                    var returnModel = new GroupReportRegistryModel();
                    var reportRegistryList = new List<ReportRegistryGridModel>();
                    foreach (var item in group)
                    {
                        item.EncryptedId = EncryptionService.Encrypt(item.Id.ToString());
                        reportRegistryList.Add(item);
                    }
                    returnModel.GroupName = group.FirstOrDefault()!.ReportGroup;
                    returnModel.ReportRegistryList = reportRegistryList;

                    returnList.Add(returnModel);
                }
                return returnList;
            }
        }
    }
}