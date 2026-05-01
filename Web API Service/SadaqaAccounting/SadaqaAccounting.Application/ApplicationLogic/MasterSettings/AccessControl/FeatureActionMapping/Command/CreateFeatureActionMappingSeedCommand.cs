namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccessControl.FeatureActionMapping.Command
{
    public class CreateFeatureActionMappingSeedCommand
    {
        private IActionRepository _actionRepository;
        private IFeaturesRepository _featureRepository;
        private IFeatureActionMappingRepository _featureActionMappingRepository;

        public CreateFeatureActionMappingSeedCommand(IFeaturesRepository featureRepository, IFeatureActionMappingRepository featureActionMappingRepository, IActionRepository actionRepository)
        {
            _featureRepository = featureRepository;
            _featureActionMappingRepository = featureActionMappingRepository;
            _actionRepository = actionRepository;
        }

        public async Task SeedAsync()
        {
            // Get features
            var features = await _featureRepository.GetAllAsync();

            // get action list
            var actions = await _actionRepository.GetAllAsync();

            var finalFeatureActionMappings = new List<SadaqaAccounting.Model.Models.MasterSettings.AccessControl.FeatureActionMapping>();
            var featureActionMappings = new List<SadaqaAccounting.Model.Models.MasterSettings.AccessControl.FeatureActionMapping>();

            foreach (var feature in features)
            {
                foreach (var action in actions)
                {
                    // Newly created feature action mapping
                    featureActionMappings.Add(new SadaqaAccounting.Model.Models.MasterSettings.AccessControl.FeatureActionMapping
                    {
                        FeatureId = feature.Id,
                        ActionId = action.Id,
                    });
                }
            }

            // Create feature action mapping
            foreach (var featureAction in featureActionMappings)
            {
                // Check exist feature action mapping
                var existFearureAction = await _featureActionMappingRepository.GetFeatureActionMappingByFeatureAndActionId(featureAction.FeatureId, featureAction.ActionId);

                if (existFearureAction is null)
                    finalFeatureActionMappings.Add(featureAction);
            }

            await _featureActionMappingRepository.CreateBulkInsertAsync(finalFeatureActionMappings);
        }
    }
}