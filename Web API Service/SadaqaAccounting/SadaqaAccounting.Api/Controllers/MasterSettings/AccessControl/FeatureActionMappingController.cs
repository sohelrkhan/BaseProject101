namespace SadaqaAccounting.Api.Controllers.MasterSettings.AccessControl
{
    public class FeatureActionMappingController : BaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(FeatureActionMappingCreateModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<FeatureActionMappingCreateModel>> CreateAsync(CreateFeatureActionMappingCommand featureActionCreateModel)
        {
            if (ModelState.IsValid)
            {
                var createAction = await Mediator.Send(featureActionCreateModel);
                return Ok(createAction);
            }

            return BadRequest(featureActionCreateModel);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FeatureActionMappingGridModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<FeatureActionMappingGridModel>> GetFeatureWiseActionByIdAsync(int id)
        {
            var featureAction = await Mediator.Send(new GetFeatureWiseActionsQuery { Id = id });
            return Ok(featureAction);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ICollection<FeatureActionMappingGridModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ICollection<FeatureActionMappingGridModel>>> GetAllFeatureWiseActionsync()
        {
            var allFeatureAction = await Mediator.Send(new GetAllFeatureWiseActionQuery());
            return Ok(allFeatureAction);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FeatureActionMappingViewModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<FeatureActionMappingViewModel>> GetByIdAsync(string id)
        {
            var featureActionMappingVm = new FeatureActionMappingViewModel
            {
                UpdateModel = await Mediator.Send(new GetFeatureActionDetailQuery { Id = id })
            };

            // Select list
            featureActionMappingVm.OptionsDataSources.ModuleSelectList = await Mediator.Send(new SelectListModuleQuery());
            return Ok(featureActionMappingVm);
        }
    }
}