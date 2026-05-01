namespace SadaqaAccounting.Api.Controllers.IncomeManagement
{
    public class IncomeCategoryController : BaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(PaginatedResponse<IncomeCategoryGridModel>), StatusCodes.Status200OK)]
        [CheckAuthorize("IncomeCategory", "List")]
        public async Task<ActionResult<PaginatedResponse<IncomeCategoryGridModel>>> GetAllAsync(GetAllIncomeCategoryQuery getAllIncomeCategoryQuery)
        {
            var getIncomeCategories = await Mediator.Send(getAllIncomeCategoryQuery);
            return Ok(getIncomeCategories);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IncomeCategoryViewModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<IncomeCategoryViewModel>> GetByIdAsync(string id)
        {
            var IncomeCategoryVm = new IncomeCategoryViewModel
            {
                UpdateModel = await Mediator.Send(new GetIncomeCategoryDetailsQuery { Id = id })
            };
            
            return Ok(IncomeCategoryVm);
        }

        [HttpPost]
        [ProducesResponseType(typeof(IncomeCategoryCreateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("IncomeCategory", "Create")]
        public async Task<ActionResult<IncomeCategoryCreateModel>> CreateAsync(CreateIncomeCategoryCommand IncomeCategoryCreateModel)
        {
            if (ModelState.IsValid)
            {
                var createIncomeCategory = await Mediator.Send(IncomeCategoryCreateModel);
                return Ok(createIncomeCategory);
            }

            return BadRequest(IncomeCategoryCreateModel);
        }

        [HttpPut]
        [ProducesResponseType(typeof(IncomeCategoryUpdateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("IncomeCategory", "Update")]
        public async Task<ActionResult<IncomeCategoryUpdateModel>> UpdateAsync(UpdateIncomeCategoryCommand IncomeCategoryUpdateModel)
        {
            if (ModelState.IsValid)
            {
                var updateIncomeCategory = await Mediator.Send(IncomeCategoryUpdateModel);
                return Ok(updateIncomeCategory);
            }

            return BadRequest(IncomeCategoryUpdateModel);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [CheckAuthorize("IncomeCategory", "Delete")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var deleteIncomeCategory = await Mediator.Send(new DeleteIncomeCategoryCommand { Id = id });
            return Ok(deleteIncomeCategory);
        }

        [HttpGet("{accountUnitId}")]
        [ProducesResponseType(typeof(IEnumerable<SelectModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SelectModel>>> GetSelectListIncomeCategoryByAccountUnitAsync(int accountUnitId)
        {
            var incomeCategories = await Mediator.Send(new SelectListIncomeCategoryByAccountUnitQuery { AccountUnitId = accountUnitId });
            return Ok(incomeCategories);
        }
    }
}