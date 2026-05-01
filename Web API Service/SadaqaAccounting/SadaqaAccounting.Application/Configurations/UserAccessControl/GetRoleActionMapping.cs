namespace SadaqaAccounting.Application.Configurations.UserAccessControl
{
    public static class GetRoleActionMapping
    {
        public static RoleActionMappingGridModel Get(ICollection<RoleActionMapping> roleActions, int RoleId)
        {
            var featureList = new List<FeatureActionMappingGridModel>();
            foreach (var features in roleActions.GroupBy(g => g.FeatureId))
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

            var roleActionMappingGridModel = new RoleActionMappingGridModel
            {
                RoleId = RoleId,
                FeatureActionMappingGridModelList = featureList
            };
            return roleActionMappingGridModel;
        }
    }
}
