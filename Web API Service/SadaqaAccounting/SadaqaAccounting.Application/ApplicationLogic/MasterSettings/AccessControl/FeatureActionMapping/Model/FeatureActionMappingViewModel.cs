namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccessControl.FeatureActionMapping.Model
{
    public class FeatureActionMappingViewModel
    {
        public FeatureActionMappingCreateModel CreateModel { get; set; }
        public FeatureActionMappingUpdateModel UpdateModel { get; set; }
        public FeatureActionMappingGridModel GridModel { get; set; }
        public dynamic OptionsDataSources { get; set; } = new ExpandoObject();
    }

    public class FeatureActionMappingCreateModel
    {
        [NotMapped] public int ModuleId { get; set; }
        public int FeatureId { get; set; }
        public int[] ActionIdList { get; set; }
    }

    public class FeatureActionMappingUpdateModel
    {
        public int FeatureId { get; set; }
        public ICollection<int> ActionIdList { get; set; }
    }

    public class FeatureActionMappingGridModel
    {
        public int Id { get; set; }
        public int FeatureId { get; set; }
        public string FeatureName { get; set; }
        public ICollection<ActionGridModel> ActionList { get; set; }
    }
}