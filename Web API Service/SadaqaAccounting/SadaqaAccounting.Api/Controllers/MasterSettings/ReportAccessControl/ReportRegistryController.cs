namespace SadaqaAccounting.Api.Controllers.MasterSettings.ReportAccessControl
{
    public class ReportRegistryController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(ICollection<ReportRegistryGridModel>), StatusCodes.Status200OK)]
        [CheckAuthorize("ReportRegistry", "List")]
        public async Task<ActionResult<ICollection<ReportRegistryGridModel>>> GetAllAsync()
        {
            var getReportRegistries = await Mediator.Send(new GetAllReportRegistryQuery());
            return Ok(getReportRegistries);
        }

        [HttpGet("{moduleId},{reportGroupName}")]
        [ProducesResponseType(typeof(IEnumerable<ReportRegistryGridModel>), StatusCodes.Status200OK)]
        [CheckAuthorize("ReportRegistry", "List")]
        public async Task<ActionResult<IEnumerable<ReportRegistryGridModel>>> GetReportRegisterByModuleAndReportGroupAsync(int? moduleId, string? reportGroupName)
        {
            var reportRegistries = await Mediator.Send(new GetReportRegisterByModuleAndReportGroupQuery { ModuleId = moduleId, ReportGroupName = reportGroupName });
            return Ok(reportRegistries);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ReportRegistryViewModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<ReportRegistryViewModel>> GetByIdAsync(int id)
        {
            var reportRegistryViewModel = new ReportRegistryViewModel
            {
                UpdateModel = await Mediator.Send(new GetReportRegistryDetailsQuery { Id = id })
            };

            reportRegistryViewModel.OptionsDataSources.ModuleSelectList = await Mediator.Send(new SelectListModuleQuery());
            reportRegistryViewModel.OptionsDataSources.ReportGroupSelectList = await Mediator.Send(new GetSelectListReportGroupQuery());
            reportRegistryViewModel.OptionsDataSources.StatusSelectList = 
                await Mediator.Send(new GetEnumTypeCollectionQuery { EnumTypeId = BaseEnumConfiguration.SadaqaAccounting.GlobalStatus });

            return Ok(reportRegistryViewModel);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ReportRegistryCreateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("ReportRegistry", "Create")]
        public async Task<ActionResult<ReportRegistryCreateModel>> CreateAsync(CreateReportRegistryCommand reportRegistryCreateModel)
        {
            if (ModelState.IsValid)
            {
                var createReportRegistry = await Mediator.Send(reportRegistryCreateModel);
                return Ok(createReportRegistry);
            }

            return BadRequest(reportRegistryCreateModel);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ReportRegistryUpdateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("ReportRegistry", "Update")]
        public async Task<ActionResult<ReportRegistryUpdateModel>> UpdateAsync(UpdateReportRegistryCommand reportRegistryUpdateModel)
        {
            if (ModelState.IsValid)
            {
                var updateReportRegistry = await Mediator.Send(reportRegistryUpdateModel);
                return Ok(updateReportRegistry);
            }

            return BadRequest(reportRegistryUpdateModel);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [CheckAuthorize("ReportRegistry", "Delete")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var deleteReportRegistry = await Mediator.Send(new DeleteReportRegistryCommand { Id = id });
            return Ok(deleteReportRegistry);
        }

        [HttpGet("{moduleCode}")]
        [ProducesResponseType(typeof(ICollection<GroupReportRegistryModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ICollection<GroupReportRegistryModel>>> GetAllReportRegistryByModuleCodeAsync(string moduleCode)
        {
            var getReportRegistries = await Mediator.Send(new GetAllReportRegistryByModuleCodeQuery{ModuleCode = moduleCode});
            return Ok(getReportRegistries);
        }
    }
}