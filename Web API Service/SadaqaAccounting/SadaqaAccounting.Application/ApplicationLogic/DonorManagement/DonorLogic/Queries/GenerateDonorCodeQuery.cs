using SadaqaAccounting.Repository.Contracts.DonorManagement;

namespace SadaqaAccounting.Application.ApplicationLogic.DonorManagement.DonorLogic.Queries
{
    public class GenerateDonorCodeQuery: IRequest<string>
    {
        public class Handler : IRequestHandler<GenerateDonorCodeQuery, string>
        {
            private readonly IDonorRepository _donorRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IDonorRepository donorRepository, IHttpContextAccessor httpContextAccessor)
            {
                _donorRepository = donorRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<string> Handle(GenerateDonorCodeQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);
                
                var getUniqueCode = await _donorRepository.GetUniqueDonorCodeAsync();
                return getUniqueCode;
            }
        }
    }
}
