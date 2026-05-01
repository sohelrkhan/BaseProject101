namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccessControl.ActionLogic.Queries
{
    public class SelectListActionQuery : IRequest<List<SelectModel>>
    {
        public class Handler : IRequestHandler<SelectListActionQuery, List<SelectModel>>
        {
            private readonly IActionRepository _actionRepository;

            public Handler(IActionRepository actionRepository)
            {
                _actionRepository = actionRepository;
            }

            public async Task<List<SelectModel>> Handle(SelectListActionQuery request, CancellationToken cancellationToken)
            {
                var getActions = await _actionRepository.GetAllAsync();

                var actionSelectList = getActions
                    .Select(s => new SelectModel()
                    {
                        Id = s.Id,
                        Name = s.Name
                    }).ToList();

                return actionSelectList;
            }
        }
    }
}