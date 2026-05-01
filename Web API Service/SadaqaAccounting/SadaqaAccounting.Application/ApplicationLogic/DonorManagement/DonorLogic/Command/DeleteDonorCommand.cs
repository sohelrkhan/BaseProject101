using SadaqaAccounting.Repository.Contracts.DonorManagement;

namespace SadaqaAccounting.Application.ApplicationLogic.DonorManagement.DonorLogic.Command
{
    public class DeleteDonorCommand: IRequest<bool>
    {
        public int Id { get; set; }
        public class Handler : IRequestHandler<DeleteDonorCommand, bool>
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
            public async Task<bool> Handle(DeleteDonorCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);
                var existingDonor = await _donorRepository.GetByIdAsync(request.Id);
                if (existingDonor == null)
                    throw new NotFoundException(ProvideErrorMessage.DonorNotFound);

                var isDeleteEvent = false;

                if (existingDonor is not null)
                {
                    existingDonor.IsDeleted = true;
                    existingDonor.DeletedDateTime = DateTime.UtcNow;
                    var updatedLeavePolicy = await _donorRepository.UpdateAsync(existingDonor);
                    isDeleteEvent = true;
                }
                return isDeleteEvent;
            }
        }
    }
}
