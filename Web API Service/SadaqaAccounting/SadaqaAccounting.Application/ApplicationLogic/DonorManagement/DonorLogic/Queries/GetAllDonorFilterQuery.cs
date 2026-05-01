using SadaqaAccounting.Application.ApplicationLogic.DonorManagement.DonorLogic.Model;
using SadaqaAccounting.Repository.Contracts.DonorManagement;

namespace SadaqaAccounting.Application.ApplicationLogic.DonorManagement.DonorLogic.Queries
{
    public class GetAllDonorFilterQuery: PaginationRequest, IRequest<PaginatedResponse<DonorGridModel>>
    {
        public class Handler : IRequestHandler<GetAllDonorFilterQuery, PaginatedResponse<DonorGridModel>>
        {
            private readonly IDonorRepository _donorRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IDonorRepository _donorRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                this._donorRepository = _donorRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<PaginatedResponse<DonorGridModel>> Handle(GetAllDonorFilterQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Retrieve the user's accountUnitId from the current HTTP context
                request.AccountUnitId = int.Parse(_httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value!);

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var getDonors = await _donorRepository.GetDonorsFilterAsync(request, cancellationToken);
                var mapDonors = _mapper.Map<ICollection<DonorGridModel>>(getDonors.Data);

                var result = new PaginatedResponse<DonorGridModel>
                {
                    Data = mapDonors,
                    CurrentPage = getDonors.CurrentPage,
                    TotalPages = getDonors.TotalPages,
                    TotalRecords = getDonors.TotalRecords,
                    PageSize = getDonors.PageSize
                };

                return result;
            }
        }
    }
}
