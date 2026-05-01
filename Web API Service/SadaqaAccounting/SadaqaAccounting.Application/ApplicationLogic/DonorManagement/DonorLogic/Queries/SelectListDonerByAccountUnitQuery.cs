namespace SadaqaAccounting.Application.ApplicationLogic.DonorManagement.DonorLogic.Queries
{
    public class SelectListDonerByAccountUnitQuery : IRequest<IEnumerable<SelectModel>>
    {
        public int AccountUnitId { get; set; }

        public class Handler : IRequestHandler<SelectListDonerByAccountUnitQuery, IEnumerable<SelectModel>>
        {
            private readonly IDonorRepository _donoeRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IDonorRepository donorRepository, IHttpContextAccessor httpContextAccessor)
            {
                _donoeRepository = donorRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<IEnumerable<SelectModel>> Handle(SelectListDonerByAccountUnitQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getIdonoes = await _donoeRepository.GetDonerSelectListByAccountUnitAsync(request.AccountUnitId, cancellationToken);
                return getIdonoes;
            }
        }
    }
}