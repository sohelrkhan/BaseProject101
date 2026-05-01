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
    public class GetAllAssetByQuery : PaginationRequest, IRequest<PaginatedResponse<AssetGridModel>>
    {
        public class Handler : IRequestHandler<GetAllAssetByQuery, PaginatedResponse<AssetGridModel>>
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

            public async Task<PaginatedResponse<AssetGridModel>> Handle(GetAllAssetByQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Retrieve the user's accountUnitId from the current HTTP context
                request.AccountUnitId = int.Parse(_httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value!);

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var (getAsset, totalRecords) = await _assettRepository.GetPaginatedAsync(request.Page,request.PageSize,
                                                                             request.SearchTerm,request.SortField,request.SortOrder,
                                                                             null,request.AdditionalFilters);

                // Calculate total pages
                var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

                var mapAsset = _mapper.Map<ICollection<AssetGridModel>>(getAsset);

                var result = new PaginatedResponse<AssetGridModel>
                {
                    Data = mapAsset,
                    TotalRecords = totalRecords,
                    CurrentPage = request.Page,
                    PageSize = request.PageSize,
                    TotalPages = totalPages
                };

                return result;
            }
        }
    }
}
