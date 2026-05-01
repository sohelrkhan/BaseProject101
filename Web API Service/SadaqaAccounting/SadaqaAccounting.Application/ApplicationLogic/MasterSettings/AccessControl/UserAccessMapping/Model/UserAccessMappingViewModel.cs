namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccessControl.UserAccessMapping.Model
{
    public class UserAccessMappingViewModel
    {
        public UserAccessMappingDetails UserAccessMapping { get; set; }
    }

    public class UserAccessMappingCreateModel
    {
        public string UserId { get; set; }
        public FeatureActionMappingCreateModel[] FeatureActionMappingCreateModel { get; set; }
    }
  
    public class UserAccessMappingGridModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public ICollection<FeatureActionMappingGridModel> FeatureActionMappingGridModelList { get; set; }
    }

    public class UserAccessMappingDetails : IMapFrom<SadaqaAccounting.Model.Models.MasterSettings.AccessControl.UserAccessMapping>
    {
        public int Id { get; set; }
        public int FeatureId { get; set; }
        public string FeatureName { get; set; }
        public int ActionId { get; set; }
        public string ActionName { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SadaqaAccounting.Model.Models.MasterSettings.AccessControl.UserAccessMapping, UserAccessMappingDetails>()
                .ForMember(d => d.FeatureName, s => s.MapFrom(m => m.Feature.Name))
                .ForMember(d => d.ActionName, s => s.MapFrom(m => m.Action.Name));
        }
    }
}