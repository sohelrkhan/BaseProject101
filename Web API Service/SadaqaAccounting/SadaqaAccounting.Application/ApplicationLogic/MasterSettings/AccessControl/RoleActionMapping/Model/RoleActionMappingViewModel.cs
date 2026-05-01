namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccessControl.RoleActionMapping.Model
{
    public class RoleActionMappingViewModel
    {
    }

    public class RoleActionMappingCreateModel
    {
        public string RoleName { get; set; }
        public FeatureActionMappingCreateModel[] FeatureActionMappingCreateModel { get; set; }
    }

    public class RoleActionMappingGridModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public ICollection<FeatureActionMappingGridModel> FeatureActionMappingGridModelList { get; set; }
    }
}