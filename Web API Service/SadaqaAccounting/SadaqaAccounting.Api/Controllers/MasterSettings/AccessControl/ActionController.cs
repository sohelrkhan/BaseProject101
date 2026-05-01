namespace SadaqaAccounting.Api.Controllers.MasterSettings.AccessControl
{
    public class ActionController : BaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(PaginatedResponse<ActionGridModel>), StatusCodes.Status200OK)]
        [CheckAuthorize("Action", "List")]
        public async Task<ActionResult<PaginatedResponse<ActionGridModel>>> GetAllAsync(GetAllActionQuery getAllActionQuery)
        {
            var getActions = await Mediator.Send(getAllActionQuery);
            return Ok(getActions);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ActionViewModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<ActionViewModel>> GetByIdAsync(string id)
        {
            var actionViewModel = new ActionViewModel
            {
                UpdateModel = await Mediator.Send(new GetActionDetailsQuery { Id = id })
            };

            // Select list
            actionViewModel.OptionsDataSources.StatusSelectList = Mediator.Send(new GetEnumTypeCollectionQuery { EnumTypeId = BaseEnumConfiguration.SadaqaAccounting.GlobalStatus }).Result;
            return Ok(actionViewModel);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ActionCreateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("Action", "Create")]
        public async Task<ActionResult<ActionCreateModel>> CreateAsync(CreateActionCommand actionCreateModel)
        {
            if (ModelState.IsValid)
            {
                var createAction = await Mediator.Send(actionCreateModel);
                return Ok(createAction);
            }

            return BadRequest(actionCreateModel);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ActionUpdateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("Action", "Update")]
        public async Task<ActionResult<ActionUpdateModel>> UpdateAsync(UpdateActionCommand actionUpdateModel)
        {
            if (ModelState.IsValid)
            {
                var updateAction = await Mediator.Send(actionUpdateModel);
                return Ok(updateAction);
            }

            return BadRequest(actionUpdateModel);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [CheckAuthorize("Action", "Delete")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var deleteAction = await Mediator.Send(new DeleteActionCommand { Id = id });
            return Ok(deleteAction);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ActionViewModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<ActionViewModel>> GetSelectListActionAsync()
        {
            var actionViewModel = new ActionViewModel();
            actionViewModel.OptionsDataSources.ActionSelectList = await Mediator.Send(new SelectListActionQuery());
            return Ok(actionViewModel);
        }
    }
}