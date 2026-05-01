namespace SadaqaAccounting.Api.Controllers.MasterSettings
{
    public class ModuleController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(ICollection<ModuleGridModel>), StatusCodes.Status200OK)]
        [CheckAuthorize("Module", "List")]
        public async Task<ActionResult<ICollection<ModuleGridModel>>> GetAllAsync()
        {
            var getModules = await Mediator.Send(new GetAllModuleQuery());
            return Ok(getModules);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ModuleViewModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<ModuleViewModel>> GetByIdAsync(string id)
        {
            var moduleViewModel = new ModuleViewModel
            {
                UpdateModel = await Mediator.Send(new GetModuleDetailsQuery { Id = id })
            };

            // Select list
            moduleViewModel.OptionsDataSources.StatusSelectList = Mediator.Send(new GetEnumTypeCollectionQuery { EnumTypeId = BaseEnumConfiguration.SadaqaAccounting.GlobalStatus }).Result;
            return Ok(moduleViewModel);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ModuleCreateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("Module", "Create")]
        public async Task<ActionResult<ModuleCreateModel>> CreateAsync(CreateModuleCommand moduleCreateModel)
        {
            if (ModelState.IsValid)
            {
                var createModule = await Mediator.Send(moduleCreateModel);
                return Ok(createModule);
            }

            return BadRequest(moduleCreateModel);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ModuleUpdateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("Module", "Update")]
        public async Task<ActionResult<ModuleUpdateModel>> UpdateAsync(UpdateModuleCommand moduleUpdateModel)
        {
            if (ModelState.IsValid)
            {
                var updateModule = await Mediator.Send(moduleUpdateModel);
                return Ok(updateModule);
            }

            return BadRequest(moduleUpdateModel);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [CheckAuthorize("Module", "Delete")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var deleteModule = await Mediator.Send(new DeleteModuleCommand { Id = id });
            return Ok(deleteModule);
        }
    }
}