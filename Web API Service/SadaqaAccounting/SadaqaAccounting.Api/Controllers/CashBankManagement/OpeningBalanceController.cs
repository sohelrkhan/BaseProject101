namespace SadaqaAccounting.Api.Controllers.CashBankManagement
{
    public class OpeningBalanceController : BaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(PaginatedResponse<OpeningBalanceGridModel>), StatusCodes.Status200OK)]
        [CheckAuthorize("OpeningBalance", "List")]
        public async Task<ActionResult<PaginatedResponse<OpeningBalanceGridModel>>> GetAllAsync(GetAllOpeningBalanceQuery getAllOpeningBalanceQuery)
        {
            var getOpeningBalance = await Mediator.Send(getAllOpeningBalanceQuery);
            return Ok(getOpeningBalance);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OpeningBalanceViewModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<OpeningBalanceViewModel>> GetByIdAsync(string id)
        {
            var openingBalanceVm = new OpeningBalanceViewModel
            {
                UpdateModel = await Mediator.Send(new GetOpeningBalanceDetailQuery { Id = id })
            };

            // Get select list
            openingBalanceVm.OptionsDataSources.PaymentModeSelectList = 
                await Mediator.Send(new GetEnumTypeCollectionQuery 
                { EnumTypeId = BaseEnumConfiguration.SadaqaAccounting.PaymentMode });

            return Ok(openingBalanceVm);
        }

        [HttpPost]
        [ProducesResponseType(typeof(OpeningBalanceCreateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("OpeningBalance", "Create")]
        public async Task<ActionResult<OpeningBalanceCreateModel>> CreateAsync(CreateOpeningBalanceCommand openingBalanceCreateModel)
        {
            if (ModelState.IsValid)
            {
                var createOpeningBalance = await Mediator.Send(openingBalanceCreateModel);
                return Ok(createOpeningBalance);
            }

            return BadRequest(openingBalanceCreateModel);
        }

        [HttpPut]
        [ProducesResponseType(typeof(OpeningBalanceUpdateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("OpeningBalance", "Update")]
        public async Task<ActionResult<OpeningBalanceUpdateModel>> UpdateAsync(UpdateOpeningBalanceCommand openingBalanceUpdateModel)
        {
            if (ModelState.IsValid)
            {
                var updateOpeningBalance = await Mediator.Send(openingBalanceUpdateModel);
                return Ok(updateOpeningBalance);
            }

            return BadRequest(openingBalanceUpdateModel);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [CheckAuthorize("OpeningBalance", "Delete")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var deleteOpeningBalance = await Mediator.Send(new DeleteOpeningBalanceCommand { Id = id });
            return Ok(deleteOpeningBalance);
        }
    }
}