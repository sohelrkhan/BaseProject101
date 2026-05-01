using SadaqaAccounting.Application.ApplicationLogic.DonorManagement.DonorLogic.Command;
using SadaqaAccounting.Repository.Contracts.AssetManagement;
using SadaqaAccounting.Repository.Contracts.DonorManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadaqaAccounting.Application.ApplicationLogic.AssetManagement.AssetLogic.Command
{
    public class DeleteAssetCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public class Handler : IRequestHandler<DeleteAssetCommand, bool>
        {
            private readonly  IAssettRepository _assettRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;
            public Handler(IAssettRepository assettRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _assettRepository = assettRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }
            public async Task<bool> Handle(DeleteAssetCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);
                var existingAsset = await _assettRepository.GetByIdAsync(request.Id);
                if (existingAsset == null)
                    throw new NotFoundException(ProvideErrorMessage.AssetnotFound);

                var isDeleteEvent = false;

                if (existingAsset is not null)
                {
                    existingAsset.IsDeleted = true;
                    existingAsset.DeletedDateTime = DateTime.UtcNow;
                    var updatedLeavePolicy = await _assettRepository.UpdateAsync(existingAsset);
                    isDeleteEvent = true;
                }
                return isDeleteEvent;
            }
        }
    }
}
