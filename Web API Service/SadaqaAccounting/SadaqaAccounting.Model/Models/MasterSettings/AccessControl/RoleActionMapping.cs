namespace SadaqaAccounting.Model.Models.MasterSettings.AccessControl
{
    [Table("RoleActionMappings", Schema = "MasterSettings")]
    public class RoleActionMapping
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please, select user.")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Please, select feature.")]
        public int FeatureId { get; set; }

        [Required(ErrorMessage = "Please, select action.")]
        public int ActionId { get; set; }

        public Role Role { get; set; }
        public Feature Feature { get; set; }
        public Action Action { get; set; }
    }
}