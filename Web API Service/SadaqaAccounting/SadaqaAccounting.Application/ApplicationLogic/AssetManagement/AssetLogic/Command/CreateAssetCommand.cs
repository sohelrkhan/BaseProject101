using SadaqaAccounting.Application.ApplicationLogic.AssetManagement.AssetLogic.Model;
using SadaqaAccounting.Application.ApplicationLogic.DonorManagement.DonorLogic.Command;
using SadaqaAccounting.Application.ApplicationLogic.DonorManagement.DonorLogic.Model;
using SadaqaAccounting.Model.Models.AssetManagement;
using SadaqaAccounting.Model.Models.DonorManagement;
using SadaqaAccounting.Repository.Contracts.AssetManagement;
using SadaqaAccounting.Repository.Contracts.DonorManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadaqaAccounting.Application.ApplicationLogic.AssetManagement.AssetLogic.Command
{
    public class CreateAssetCommand : AssetCreateModel, IRequest<AssetCreateModel>
    {
        public class Handler : IRequestHandler<CreateAssetCommand, AssetCreateModel>
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

            public async Task<AssetCreateModel> Handle(CreateAssetCommand request, CancellationToken cancellationToken)
            {
                // Retrieve the user's accountUnitId from the current HTTP context
                var accountUnitId = _httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value;

                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var createdAsset = _mapper.Map<Asset>(request);
                createdAsset.StatusId = GlobalStatus.Active;
                createdAsset.CreatedById = userId;
                createdAsset.AccountUnitId = int.Parse(accountUnitId!);
                createdAsset.CreatedDateTime = DateTime.UtcNow;
                createdAsset = await _assettRepository.CreateAsync(createdAsset);

                return request;
            }
        }
    }
}
