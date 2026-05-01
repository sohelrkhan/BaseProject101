using SadaqaAccounting.Application.ApplicationLogic.AssetManagement.AssetLogic.Model;
using SadaqaAccounting.Application.ApplicationLogic.DonorManagement.DonorLogic.Command;
using SadaqaAccounting.Application.ApplicationLogic.DonorManagement.DonorLogic.Model;
using SadaqaAccounting.Repository.Contracts.AssetManagement;
using SadaqaAccounting.Repository.Contracts.DonorManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadaqaAccounting.Application.ApplicationLogic.AssetManagement.AssetLogic.Command
{
    public class UpdateAssetCommand : AssetUpdateModel, IRequest<AssetUpdateModel>
    {
        public class Handler : IRequestHandler<UpdateAssetCommand, AssetUpdateModel>
        {
            private readonly IAssettRepository _assettRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;
            public Handler(IAssettRepository assettRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                 _assettRepository = assettRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }
            public async Task<AssetUpdateModel> Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
                // Retrieve the user's accountUnitId from the current HTTP context
                var AccountUnitId = int.Parse(_httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value!);
                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);
                var existingAsset = await _assettRepository.GetByIdAsync(request.Id);
                if (existingAsset == null)
                    throw new NotFoundException(ProvideErrorMessage.AssetIdNotFound);

                existingAsset = _mapper.Map((AssetUpdateModel)request, existingAsset);
                existingAsset.AccountUnitId = AccountUnitId;
                existingAsset.UpdatedById = userId;
                existingAsset.UpdatedDateTime = DateTime.UtcNow;
                await _assettRepository.UpdateAsync(existingAsset);
                return request;
            }
        }
    }
}
