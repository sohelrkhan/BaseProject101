using SadaqaAccounting.Application.ApplicationLogic.DonorManagement.DonorLogic.Model;
using SadaqaAccounting.Repository.Contracts.DonorManagement;

namespace SadaqaAccounting.Application.ApplicationLogic.DonorManagement.DonorLogic.Queries
{
    public class GetDonorDetailsQuery: IRequest<DonorUpdateModel>
    {
        public string Id { get; set; }
        public class Handler : IRequestHandler<GetDonorDetailsQuery, DonorUpdateModel>
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

            public async Task<DonorUpdateModel> Handle(GetDonorDetailsQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Check if donor id is null
                if (string.IsNullOrWhiteSpace(request.Id) || string.IsNullOrEmpty(request.Id) || request.Id == "-1")
                    return new DonorUpdateModel();

                // Decrypt donor id
                var decryptDonorId = EncryptionService.Decrypt(request.Id);

                // Check if donor decrypt id is null
                if (string.IsNullOrWhiteSpace(decryptDonorId) || string.IsNullOrEmpty(decryptDonorId))
                    return new DonorUpdateModel();

                // Convert decrypt donor id
                var convertDonorId = Convert.ToInt32(decryptDonorId);
                var getExistDonor = await _donorRepository.GetByIdAsync(convertDonorId);

                if (getExistDonor is null)
                    return new DonorUpdateModel();

                var mapExitDonor = _mapper.Map<DonorUpdateModel>(getExistDonor);
                return mapExitDonor;
            }
        }
    }
}
