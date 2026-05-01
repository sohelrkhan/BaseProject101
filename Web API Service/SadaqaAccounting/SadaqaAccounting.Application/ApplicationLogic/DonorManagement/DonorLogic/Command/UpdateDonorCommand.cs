using SadaqaAccounting.Application.ApplicationLogic.DonorManagement.DonorLogic.Model;
using SadaqaAccounting.Repository.Contracts.DonorManagement;

namespace SadaqaAccounting.Application.ApplicationLogic.DonorManagement.DonorLogic.Command
{
    public class UpdateDonorCommand: DonorUpdateModel, IRequest<DonorUpdateModel>
    {
        public class Handler : IRequestHandler<UpdateDonorCommand, DonorUpdateModel>
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
            public async Task<DonorUpdateModel> Handle(UpdateDonorCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
                // Retrieve the user's accountUnitId from the current HTTP context
                var AccountUnitId = int.Parse(_httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value!);
                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);
                var existingDonor = await _donorRepository.GetByIdAsync(request.Id);
                if (existingDonor == null)
                    throw new NotFoundException(ProvideErrorMessage.DonorNotFound);

                existingDonor = _mapper.Map((DonorUpdateModel)request, existingDonor);
                existingDonor.AccountUnitId = AccountUnitId;
                existingDonor.UpdatedById = userId;
                existingDonor.UpdatedDateTime = DateTime.UtcNow;
                await _donorRepository.UpdateAsync(existingDonor);
                return request;
            }
        }
    }
}
