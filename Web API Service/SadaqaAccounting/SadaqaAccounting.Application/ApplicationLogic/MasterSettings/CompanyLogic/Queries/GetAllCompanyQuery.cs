namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.CompanyLogic.Queries
{
    public class GetAllCompanyQuery : IRequest<ICollection<CompanyGridModel>>
    {
        public class Handler : IRequestHandler<GetAllCompanyQuery, ICollection<CompanyGridModel>>
        {
            private readonly ICompanyRepository _companyRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(ICompanyRepository companyRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _companyRepository = companyRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<ICollection<CompanyGridModel>> Handle(GetAllCompanyQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getCompanies = await _companyRepository.GetAllAsync();
                var mapGetCompanies = _mapper.Map<ICollection<CompanyGridModel>>(getCompanies);

                return mapGetCompanies;
            }
        }
    }
}