namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.CompanyLogic.Queries
{
    public class SelectListCompanyQuery : IRequest<ICollection<SelectModel>>
    {
        public class Handler : IRequestHandler<SelectListCompanyQuery, ICollection<SelectModel>>
        {
            private readonly ICompanyRepository _companyRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(ICompanyRepository companyRepository, IHttpContextAccessor httpContextAccessor)
            {
                _companyRepository = companyRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<ICollection<SelectModel>> Handle(SelectListCompanyQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getCompanies = await _companyRepository.GetCompanySelectListAsync();
                return getCompanies;
            }
        }
    }
}