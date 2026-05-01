namespace SadaqaAccounting.Model.Models.MasterSettings
{
    [Table("Features", Schema = "MasterSettings")]
    public class Feature : IDelatableEntity
    {
        public Feature()
        {
            FeatureActionMappings = new HashSet<FeatureActionMapping>();
            RoleActionMappings = new HashSet<RoleActionMapping>();
            UserAccessMappings = new HashSet<UserAccessMapping>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Please, select module.")]
        public int ModuleId { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide code.")]
        [StringLength(500, MinimumLength = 2)]
        public string Code { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(500, MinimumLength = 2)]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide linked table name.")]
        [StringLength(50, MinimumLength = 2)]
        public string LinkedTableName { get; set; }

        public string? LinkedControllerName { get; set; }

        [Required(ErrorMessage = "Please, select status.")]
        public int StatusId { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedDateTime { get; set; }

        public Module Module { get; set; }
        public EnumTypeCollection Status { get; set; }
        public ICollection<FeatureActionMapping> FeatureActionMappings { get; set; }
        public ICollection<RoleActionMapping> RoleActionMappings { get; set; }
        public ICollection<UserAccessMapping> UserAccessMappings { get; set; }
    }
}