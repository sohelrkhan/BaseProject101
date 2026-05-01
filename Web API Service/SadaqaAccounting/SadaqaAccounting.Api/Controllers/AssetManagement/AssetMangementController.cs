using Microsoft.AspNetCore.Mvc;
using SadaqaAccounting.Application.ApplicationLogic.AssetManagement.AssetLogic.Command;
using SadaqaAccounting.Application.ApplicationLogic.AssetManagement.AssetLogic.Model;
using SadaqaAccounting.Application.ApplicationLogic.AssetManagement.AssetLogic.Queries;
using SadaqaAccounting.Application.ApplicationLogic.DonorManagement.DonorLogic.Command;
using SadaqaAccounting.Application.ApplicationLogic.DonorManagement.DonorLogic.Model;
using SadaqaAccounting.Application.ApplicationLogic.DonorManagement.DonorLogic.Queries;

namespace SadaqaAccounting.Api.Controllers.AssetManagement
{
    public class AssetMangementController : BaseController
    {

        [HttpPost]
        [ProducesResponseType(typeof(AssetCreateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("Asset", "Create")]
        public async Task<ActionResult<AssetCreateModel>> CreateAsync(CreateAssetCommand assetCreateModel)
        {
            if (ModelState.IsValid)
            {
                var createAsset = await Mediator.Send(assetCreateModel);
                return Ok(createAsset);
            }

            return BadRequest(assetCreateModel);
        }


        [HttpPut]
        [ProducesResponseType(typeof(AssetUpdateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("Asset", "Update")]
        public async Task<ActionResult<AssetUpdateModel>> UpdateAsync(UpdateAssetCommand assetUpdateModel)
        {
            if (ModelState.IsValid)
            {
                var updateAsset = await Mediator.Send(assetUpdateModel);
                return Ok(updateAsset);
            }

            return BadRequest(assetUpdateModel);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [CheckAuthorize("Asset", "Delete")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var deleteAsset = await Mediator.Send(new DeleteAssetCommand { Id = id });
            return Ok(deleteAsset);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AssetViewModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<AssetViewModel>> GetByIdAsync(string id)
        {
            var assetViewModel = new AssetViewModel
            {
                UpdateModel = await Mediator.Send(new GetAssetByIdQuery { Id = id })
            };
            assetViewModel.OptionsDataSources.StatusSelectList = Mediator.Send(new GetEnumTypeCollectionQuery { EnumTypeId = BaseEnumConfiguration.SadaqaAccounting.GlobalStatus }).Result;
            return Ok(assetViewModel);
        }

        [HttpPost]
        [ProducesResponseType(typeof(PaginatedResponse<AssetGridModel>), StatusCodes.Status200OK)]
        [CheckAuthorize("Asset", "List")]
        public async Task<ActionResult<PaginatedResponse<AssetGridModel>>> GetAllAssetFilterAsync(GetAllAssetByQuery getAllAssetQuery)
        {
            var getAsset = await Mediator.Send(getAllAssetQuery);
            return Ok(getAsset);
        }
    }
}

