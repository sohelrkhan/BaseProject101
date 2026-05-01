namespace SadaqaAccounting.Api.Controllers.DonorManagement
{
    public class DonorController : BaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(DonorCreateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("Donor", "Create")]
        public async Task<ActionResult<DonorCreateModel>> CreateAsync(CreateDonorCommand donorCreateModel)
        {
            if (ModelState.IsValid)
            {
                var createDonor = await Mediator.Send(donorCreateModel);
                return Ok(createDonor);
            }

            return BadRequest(donorCreateModel);
        }
        [HttpPut]
        [ProducesResponseType(typeof(DonorUpdateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("Donor", "Update")]
        public async Task<ActionResult<DonorUpdateModel>> UpdateAsync(UpdateDonorCommand donorUpdateModel)
        {
            if (ModelState.IsValid)
            {
                var updateDonor = await Mediator.Send(donorUpdateModel);
                return Ok(updateDonor);
            }

            return BadRequest(donorUpdateModel);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [CheckAuthorize("Donor", "Delete")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var deleteDonor = await Mediator.Send(new DeleteDonorCommand { Id = id });
            return Ok(deleteDonor);
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DonorViewModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<DonorViewModel>> GetByIdAsync(string id)
        {
            var donorViewModel = new DonorViewModel
            {
                UpdateModel = await Mediator.Send(new GetDonorDetailsQuery { Id = id })
            };
            donorViewModel.OptionsDataSources.StatusSelectList = Mediator.Send(new GetEnumTypeCollectionQuery { EnumTypeId = BaseEnumConfiguration.SadaqaAccounting.GlobalStatus }).Result;
            donorViewModel.GenerateDonorCode = await Mediator.Send(new GenerateDonorCodeQuery());
            return Ok(donorViewModel);
        }
        [HttpPost]
        [ProducesResponseType(typeof(PaginatedResponse<DonorGridModel>), StatusCodes.Status200OK)]
        [CheckAuthorize("Donor", "List")]
        public async Task<ActionResult<PaginatedResponse<DonorGridModel>>> GetAllDonorFilterAsync(GetAllDonorFilterQuery getAllDonorQuery)
        {
            var getDonors = await Mediator.Send(getAllDonorQuery);
            return Ok(getDonors);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SelectModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SelectModel>>> GetSelectListDonorAsync()
        {
            var donorList = await Mediator.Send(new SelectListDonorQuery());
            return Ok(donorList);
        }

        [HttpGet("{accountUnitId}")]
        [ProducesResponseType(typeof(IEnumerable<SelectModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SelectModel>>> GetSelectListDonorByAccountUnitAsync(int accountUnitId)
        {
            var doners = await Mediator.Send(new SelectListDonerByAccountUnitQuery { AccountUnitId = accountUnitId });
            return Ok(doners);
        }
    }
}