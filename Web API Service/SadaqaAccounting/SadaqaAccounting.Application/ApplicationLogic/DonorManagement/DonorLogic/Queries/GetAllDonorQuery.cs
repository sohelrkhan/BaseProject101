using SadaqaAccounting.Application.ApplicationLogic.DonorManagement.DonorLogic.Model;
using SadaqaAccounting.Repository.Contracts.DonorManagement;

namespace SadaqaAccounting.Application.ApplicationLogic.DonorManagement.DonorLogic.Queries
{
    public class GetAllDonorQuery: IRequest<ICollection<DonorGridModel>>
    {
        public class Handler : IRequestHandler<GetAllDonorQuery, ICollection<DonorGridModel>>
        {
            private readonly IDonorRepository _donorRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IDonorRepository donorRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _donorRepository = donorRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<ICollection<DonorGridModel>> Handle(GetAllDonorQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
                // Retrieve the user's accountUnitId from the current HTTP context
                var AccountUnitId = int.Parse(_httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value!);
                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getDonors = await _donorRepository.GetAllAccountUnitWiseDonorAsync(AccountUnitId);
                var mapGetDonors = _mapper.Map<ICollection<DonorGridModel>>(getDonors);

                return mapGetDonors;
            }
        }
    }
}
