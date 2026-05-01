using SadaqaAccounting.Repository.Contracts.DonorManagement;

namespace SadaqaAccounting.Application.ApplicationLogic.DonorManagement.DonorLogic.Queries
{
    public class SelectListDonorQuery : IRequest<IEnumerable<SelectModel>>
    {
        public class Handler : IRequestHandler<SelectListDonorQuery, IEnumerable<SelectModel>>
        {
            private readonly IDonorRepository _donorRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IDonorRepository donorRepository, IHttpContextAccessor httpContextAccessor)
            {
                _donorRepository = donorRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<IEnumerable<SelectModel>> Handle(SelectListDonorQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Retrieve the user's accountUnitId from the current HTTP context
                var AccountUnitId = int.Parse(_httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value!);

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var donors = await _donorRepository.GetAllAccountUnitWiseDonorAsync(AccountUnitId);

                var getDonors = donors
                .OrderBy(m => m.Name)
                .Select(m => new SelectModel
                {
                    Id = m.Id,
                    Name = m.Name,
                });
                return getDonors;
            }
        }
    }
}
