namespace SadaqaAccounting.Api.Controllers.CashBankManagement
{
    public class BankController : BaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(PaginatedResponse<BankGridModel>), StatusCodes.Status200OK)]
        [CheckAuthorize("Bank", "List")]
        public async Task<ActionResult<PaginatedResponse<BankGridModel>>> GetAllAsync(GetAllBankQuery getAllBankQuery)
        {
            var getBank = await Mediator.Send(getAllBankQuery);
            return Ok(getBank);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BankViewModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<BankViewModel>> GetByIdAsync(string id)
        {
            var bankVm = new BankViewModel
            {
                UpdateModel = await Mediator.Send(new GetBankDetailQuery { Id = id })
            };

            bankVm.OptionsDataSources.StatusSelectList = await Mediator.Send(new GetEnumTypeCollectionQuery { EnumTypeId = BaseEnumConfiguration.SadaqaAccounting.GlobalStatus });
            return Ok(bankVm);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BankCreateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("Bank", "Create")]
        public async Task<ActionResult<BankCreateModel>> CreateAsync(CreateBankCommand bankCreateModel)
        {
            if (ModelState.IsValid)
            {
                var createBank = await Mediator.Send(bankCreateModel);
                return Ok(createBank);
            }

            return BadRequest(bankCreateModel);
        }

        [HttpPut]
        [ProducesResponseType(typeof(BankUpdateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("Bank", "Update")]
        public async Task<ActionResult<BankUpdateModel>> UpdateAsync(UpdateBankCommand bankUpdateModel)
        {
            if (ModelState.IsValid)
            {
                var updateBank = await Mediator.Send(bankUpdateModel);
                return Ok(updateBank);
            }

            return BadRequest(bankUpdateModel);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [CheckAuthorize("Bank", "Delete")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var deleteBank = await Mediator.Send(new DeleteBankCommand { Id = id });
            return Ok(deleteBank);
        }

        [HttpGet("{accountUnitId}")]
        [ProducesResponseType(typeof(IEnumerable<SelectModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SelectModel>>> GetSelectListBankByAccountUnitAsync(int accountUnitId)
        {
            var banks = await Mediator.Send(new SelectListBankByAccountUnitQuery { AccountUnitId = accountUnitId });
            return Ok(banks);
        }
    }
}