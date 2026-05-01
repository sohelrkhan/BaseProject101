namespace SadaqaAccounting.Application.ApplicationShared.Queries
{
    public class GetEnumTypeCollectionQuery : IRequest<ICollection<SelectModel>>
    {
        public int EnumTypeId { get; set; } 

        public class Handler : IRequestHandler<GetEnumTypeCollectionQuery, ICollection<SelectModel>>
        {
            private readonly IEnumTypeCollectionRepository _enumTypeCollectionRepository;

            public Handler(IEnumTypeCollectionRepository enumTypeCollectionRepository)
            {
                _enumTypeCollectionRepository = enumTypeCollectionRepository;
            }

            public async Task<ICollection<SelectModel>> Handle(GetEnumTypeCollectionQuery request, CancellationToken cancellationToken)
            {
                var enumTypeCollections = await _enumTypeCollectionRepository.GetAllAsync();

                var mapSelectModels = enumTypeCollections.Where(etc => etc.EnumTypeId == request.EnumTypeId && !etc.IsDeleted)
                    .Select(s => new SelectModel
                    {
                        Id = s.Id,
                        Name = s.Name
                    }).ToList();

                return mapSelectModels;   
            }
        }
    }
}