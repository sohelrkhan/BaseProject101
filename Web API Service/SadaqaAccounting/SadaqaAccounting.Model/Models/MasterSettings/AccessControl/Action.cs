namespace SadaqaAccounting.Model.Models.MasterSettings.AccessControl
{
    [Table("Actions", Schema = "MasterSettings")]
    public class Action : IDelatableEntity
    {
        public Action()
        {
            FeatureActionMappings = new HashSet<FeatureActionMapping>();
            RoleActionMappings = new HashSet<RoleActionMapping>();
            UserAccessMappings = new HashSet<UserAccessMapping>();
        }

        public int Id { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please, provide status.")]
        public int StatusId { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedDateTime { get; set; }
        
        public EnumTypeCollection Status { get; set; }
        public ICollection<FeatureActionMapping> FeatureActionMappings { get; set; }
        public ICollection<RoleActionMapping> RoleActionMappings { get; set; }
        public ICollection<UserAccessMapping> UserAccessMappings { get; set; }
    }
}