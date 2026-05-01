namespace SadaqaAccounting.Model.Models.MasterSettings.AccessControl
{
    [Table("UserAccessMappings", Schema = "MasterSettings")]
    public class UserAccessMapping
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please, select user.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Please, select feature.")]
        public int FeatureId { get; set; }

        [Required(ErrorMessage = "Please, select action.")]
        public int ActionId { get; set; }

        public User User { get; set; }
        public Feature Feature { get; set; }
        public Action Action { get; set; }
    }
}