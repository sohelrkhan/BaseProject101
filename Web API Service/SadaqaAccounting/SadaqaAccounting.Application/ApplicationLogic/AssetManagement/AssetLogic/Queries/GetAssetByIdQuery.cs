using SadaqaAccounting.Application.ApplicationLogic.AssetManagement.AssetLogic.Model;
using SadaqaAccounting.Application.ApplicationLogic.DonorManagement.DonorLogic.Model;
using SadaqaAccounting.Application.ApplicationLogic.DonorManagement.DonorLogic.Queries;
using SadaqaAccounting.Repository.Contracts.AssetManagement;
using SadaqaAccounting.Repository.Contracts.DonorManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadaqaAccounting.Application.ApplicationLogic.AssetManagement.AssetLogic.Queries
{
    public class GetAssetByIdQuery : IRequest<AssetUpdateModel>
    {
        public string Id { get; set; }
        public class Handler : IRequestHandler<GetAssetByIdQuery, AssetUpdateModel>
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

            public async Task<AssetUpdateModel> Handle(GetAssetByIdQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // Check if donor id is null
                if (string.IsNullOrWhiteSpace(request.Id) || string.IsNullOrEmpty(request.Id) || request.Id == "-1")
                    return new AssetUpdateModel();

                // Decrypt donor id
                var decryptAssetId = EncryptionService.Decrypt(request.Id);

                // Check if donor decrypt id is null
                if (string.IsNullOrWhiteSpace(decryptAssetId) || string.IsNullOrEmpty(decryptAssetId))
                    return new AssetUpdateModel();

                // Convert decrypt donor id
                var convertAssetId = Convert.ToInt32(decryptAssetId);
                var getExistAsset = await _assettRepository.GetByIdAsync(convertAssetId);

                if (getExistAsset is null)
                    return new AssetUpdateModel();

                var mapExitAsset = _mapper.Map<AssetUpdateModel>(getExistAsset);
                return mapExitAsset;
            }
        }
    }
}
