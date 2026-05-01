namespace SadaqaAccounting.Application.Configurations.UserAccessControl
{
    public static class GetUserWiseFeatureActionMapping
    {
        public static UserAccessMappingGridModel GetUserWiseFeatureAccessAsync(ICollection<UserAccessMapping> userAccesses,string userId)
        {
            var featureList = new List<FeatureActionMappingGridModel>();
            foreach (var features in userAccesses.GroupBy(g => g.FeatureId))
            {
                var actionList = new List<ActionGridModel>();
                foreach (var action in features)
                {
                    var model = new ActionGridModel
                    {
                        Id = action.ActionId,
                        Name = action.Action.Name,
                        StatusName = action.Action.Status.Name,
                        IsExist = true
                    };
                    actionList.Add(model);
                }

                var featureActionGrid = new FeatureActionMappingGridModel
                {
                    FeatureId = features.FirstOrDefault()!.FeatureId,
                    FeatureName = features.FirstOrDefault()!.Feature.Name,
                    ActionList = actionList
                };
                featureList.Add(featureActionGrid);
            }

            var userAccessMappingGridModel = new UserAccessMappingGridModel
            {
                UserId = userId,
                FeatureActionMappingGridModelList = featureList
            };
            return userAccessMappingGridModel;
        }
    }
}