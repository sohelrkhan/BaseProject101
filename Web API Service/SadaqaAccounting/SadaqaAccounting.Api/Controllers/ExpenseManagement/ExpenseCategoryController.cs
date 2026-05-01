namespace SadaqaAccounting.Api.Controllers.ExpenseManagement
{
    public class ExpenseCategoryController : BaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(PaginatedResponse<ExpenseCategoryGridModel>), StatusCodes.Status200OK)]
        [CheckAuthorize("ExpenseCategory", "List")]
        public async Task<ActionResult<PaginatedResponse<ExpenseCategoryGridModel>>> GetAllAsync(GetAllExpenseCategoryQuery getAllExpenseCategoryQuery)
        {
            var getExpenseCategories = await Mediator.Send(getAllExpenseCategoryQuery);
            return Ok(getExpenseCategories);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ExpenseCategoryViewModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<ExpenseCategoryViewModel>> GetByIdAsync(string id)
        {
            var expenseCategoryVm = new ExpenseCategoryViewModel
            {
                UpdateModel = await Mediator.Send(new GetExpenseCategoryDetailsQuery { Id = id })
            };

            // Select list
            expenseCategoryVm.OptionsDataSources.AccountUnitSelectList = await Mediator.Send(new SelectListAccountUnitQuery());
            return Ok(expenseCategoryVm);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ExpenseCategoryCreateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("ExpenseCategory", "Create")]
        public async Task<ActionResult<ExpenseCategoryCreateModel>> CreateAsync(CreateExpenseCategoryCommand expenseCategoryCreateModel)
        {
            if (ModelState.IsValid)
            {
                var createExpenseCategory = await Mediator.Send(expenseCategoryCreateModel);
                return Ok(createExpenseCategory);
            }

            return BadRequest(expenseCategoryCreateModel);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ExpenseCategoryUpdateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("ExpenseCategory", "Update")]
        public async Task<ActionResult<ExpenseCategoryUpdateModel>> UpdateAsync(UpdateExpenseCategoryCommand expenseCategoryUpdateModel)
        {
            if (ModelState.IsValid)
            {
                var updateExpenseCategory = await Mediator.Send(expenseCategoryUpdateModel);
                return Ok(updateExpenseCategory);
            }

            return BadRequest(expenseCategoryUpdateModel);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [CheckAuthorize("ExpenseCategory", "Delete")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var deleteExpenseCategory = await Mediator.Send(new DeleteExpenseCategoryCommand { Id = id });
            return Ok(deleteExpenseCategory);
        }

        [HttpGet("{accountUnitId}")]
        [ProducesResponseType(typeof(IEnumerable<SelectModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SelectModel>>> GetSelectListExpenseCategoryByAccountUnitAsync(int accountUnitId)
        {
            var expensecategories = await Mediator.Send(new SelectListExpenseCategoryByAccountUnitQuery { AccountUnitId = accountUnitId });
            return Ok(expensecategories);
        }
    }
}