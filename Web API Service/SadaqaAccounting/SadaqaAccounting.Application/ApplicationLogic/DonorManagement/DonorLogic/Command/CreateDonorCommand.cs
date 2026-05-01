using SadaqaAccounting.Application.ApplicationLogic.DonorManagement.DonorLogic.Model;
using SadaqaAccounting.Model.Models.DonorManagement;
using SadaqaAccounting.Repository.Contracts.DonorManagement;

namespace SadaqaAccounting.Application.ApplicationLogic.DonorManagement.DonorLogic.Command
{
    public class CreateDonorCommand: DonorCreateModel, IRequest<DonorCreateModel>
    {
        public class Handler : IRequestHandler<CreateDonorCommand, DonorCreateModel>
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

            public async Task<DonorCreateModel> Handle(CreateDonorCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's accountUnitId from the current HTTP context
                var accountUnitId = _httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value;

                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var isDuplicate = await _donorRepository.CheckDuplicateDonorByNameAndMobileAsync(request.Name,request.MobileNo);
                if (isDuplicate)
                {
                    throw new BadRequestException(ProvideErrorMessage.DuplicateDonor);
                }
                var createdDonor = _mapper.Map<Donor>(request);
                createdDonor.Code = await _donorRepository.GetUniqueDonorCodeAsync();
                createdDonor.StatusId = GlobalStatus.Active;
                createdDonor.CreatedById = userId;
                createdDonor.AccountUnitId = int.Parse(accountUnitId!);
                createdDonor.CreatedDateTime = DateTime.UtcNow;
                createdDonor = await _donorRepository.CreateAsync(createdDonor);

                return request;
            }
        }
    }
}
