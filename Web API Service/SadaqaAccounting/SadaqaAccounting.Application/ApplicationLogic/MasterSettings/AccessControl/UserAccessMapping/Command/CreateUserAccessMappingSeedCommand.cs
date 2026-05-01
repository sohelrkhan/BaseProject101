namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccessControl.UserAccessMapping.Command
{
    public class CreateUserAccessMappingSeedCommand
    {
        private IFeaturesRepository _featureRepository;
        private IActionRepository _actionRepository;
        private IUserAccessMappingRepository _userAccessMappingRepositry;

        public CreateUserAccessMappingSeedCommand(IUserAccessMappingRepository userAccessMappingRepository, IFeaturesRepository featureRepository, IActionRepository actionRepositry)
        {
            _userAccessMappingRepositry = userAccessMappingRepository;
            _featureRepository = featureRepository;
            _actionRepository = actionRepositry;
        }

        public async Task SeedAsync()
        {
            var userAccessMappingList = new List<SadaqaAccounting.Model.Models.MasterSettings.AccessControl.UserAccessMapping>();
            var finalUserAccessMappingList = new List<SadaqaAccounting.Model.Models.MasterSettings.AccessControl.UserAccessMapping>();

            // get actions
            var actions = await _actionRepository.GetAllAsync();

            // get features
            var features = await _featureRepository.GetAllAsync();

            foreach (var feature in features)
            {
                foreach (var action in actions)
                {
                    userAccessMappingList.Add(
                    new SadaqaAccounting.Model.Models.MasterSettings.AccessControl.UserAccessMapping
                    {
                        UserId = "a6923057-af80-4dd0-b3b2-3ff979f69b6d", 
                        FeatureId = feature.Id,
                        ActionId = action.Id
                    });
                }
            }

            foreach (var userActionMapping in userAccessMappingList)
            {
                // Check, exist user action mapping
                var existUserACtionMapping = await _userAccessMappingRepositry
                    .GetUserAccessMappingByUserFeatureActionId(userActionMapping.UserId, userActionMapping.FeatureId, userActionMapping.ActionId);

                if (existUserACtionMapping is null)
                    finalUserAccessMappingList.Add(userActionMapping);
            }

            await _userAccessMappingRepositry.CreateBulkInsertAsync(finalUserAccessMappingList);
        }
    }
}