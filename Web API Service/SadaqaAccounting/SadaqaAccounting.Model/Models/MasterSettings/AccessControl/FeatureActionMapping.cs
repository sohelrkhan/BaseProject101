namespace SadaqaAccounting.Model.Models.MasterSettings.AccessControl
{
    [Table("FeatureActionMappings", Schema = "MasterSettings")]
    public class FeatureActionMapping : IDelatableEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please, select feature.")]
        public int FeatureId { get; set; }

        [Required(ErrorMessage = "Please, select action.")]
        public int ActionId { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedDateTime { get; set; }

        public Feature Feature { get; set; }
        public Action Action { get; set; }       
    }
}