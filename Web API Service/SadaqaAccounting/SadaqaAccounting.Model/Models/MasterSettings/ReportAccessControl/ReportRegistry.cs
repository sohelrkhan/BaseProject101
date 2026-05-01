namespace SadaqaAccounting.Model.Models.MasterSettings.ReportAccessControl
{
    [Table("ReportRegistries", Schema = "MasterSettings")]
    public class ReportRegistry : IDelatableEntity
    {
        public ReportRegistry()
        {
            ReportUserAccesses = new HashSet<ReportUserAccess>();
        }

        public int Id { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(250, MinimumLength = 2)]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        [Required(ErrorMessage = "Please, provide url.")]
        public string Url { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide report code.")]
        public string ReportCode { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        [Required(ErrorMessage = "Please, provide report group.")]
        public string ReportGroup { get; set; }

        [Required(ErrorMessage = "Please, select module.")]
        public int ModuleId { get; set; }

        [Required(ErrorMessage = "Please, select status.")]
        public int StatusId { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedDateTime { get; set; }

        public Module Module { get; set; }
        public EnumTypeCollection Status { get; set; }
        public ICollection<ReportUserAccess> ReportUserAccesses { get; set; }
    }
}