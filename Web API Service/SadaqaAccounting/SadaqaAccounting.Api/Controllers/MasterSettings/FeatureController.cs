namespace SadaqaAccounting.Api.Controllers.MasterSettings
{
    public class FeatureController : BaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(IQueryable<FeaturesGridModel>), StatusCodes.Status200OK)]
        [CheckAuthorize("Feature", "List")]
        public async Task<ActionResult<IQueryable<FeaturesGridModel>>> GetAllAsync()
        {
            var getFeatures = await Mediator.Send(new GetAllFeatureQuery());
            return Ok(getFeatures);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FeaturesViewModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<FeaturesViewModel>> GetByIdAsync(string id)
        {
            var featureViewModel = new FeaturesViewModel
            {
                UpdateModel = await Mediator.Send(new GetFeatureDetailsQuery { Id = id })
            };

            // Select list
            featureViewModel.OptionsDataSources.ModuleSelectList = Mediator.Send(new SelectListModuleQuery()).Result;
            featureViewModel.OptionsDataSources.FeatureSelectList = Mediator.Send(new SelectListFeatureQuery()).Result;
            featureViewModel.OptionsDataSources.StatusSelectList = Mediator.Send(new GetEnumTypeCollectionQuery { EnumTypeId = BaseEnumConfiguration.SadaqaAccounting.GlobalStatus }).Result;

            return Ok(featureViewModel);
        }

        [HttpPost]
        [ProducesResponseType(typeof(FeaturesCreateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("Feature", "Create")]
        public async Task<ActionResult<FeaturesCreateModel>> CreateAsync(CreateFeaturesCommand featureCreateModel)
        {
            if (ModelState.IsValid)
            {
                var createFeature = await Mediator.Send(featureCreateModel);
                return Ok(createFeature);
            }

            return BadRequest(featureCreateModel);
        }

        [HttpPut]
        [ProducesResponseType(typeof(FeaturesUpdateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("Feature", "Update")]
        public async Task<ActionResult<FeaturesUpdateModel>> UpdateAsync(UpdateFeaturesCommand featureUpdateModel)
        {
            if (ModelState.IsValid)
            {
                var updateFeature = await Mediator.Send(featureUpdateModel);
                return Ok(updateFeature);
            }

            return BadRequest(featureUpdateModel);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [CheckAuthorize("Feature", "Delete")]
        public async Task<ActionResult<bool>> Delete(string id)
        {
            var deleteFeature = await Mediator.Send(new DeleteFeatureCommand { Id = id });
            return Ok(deleteFeature);
        }

        [HttpGet("{moduleId}")]
        [ProducesResponseType(typeof(ICollection<SelectModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ICollection<SelectModel>>> GetFeaturesByModuleIdAsync(int moduleId)
        {
            var getFeatures = await Mediator.Send(new SelectListFeaturesByModuleQuery { ModuleId = moduleId });
            return Ok(getFeatures);
        }

        [HttpGet]
        [ProducesResponseType(typeof(FeaturesViewModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<FeaturesViewModel>> GetSelectListFeatureAsync()
        {
            var featureViewModel = new FeaturesViewModel();
            featureViewModel.OptionsDataSources.FeatureSelectList = await Mediator.Send(new SelectListFeatureQuery());
            return Ok(featureViewModel);
        }
    }
}