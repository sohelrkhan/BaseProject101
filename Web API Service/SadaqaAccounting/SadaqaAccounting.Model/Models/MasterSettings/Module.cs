namespace SadaqaAccounting.Model.Models.MasterSettings
{
    [Table("Modules", Schema = "MasterSettings")]
    public class Module : IDelatableEntity
    {
        public Module()
        {
            Features = new HashSet<Feature>();
            ReportRegistries = new HashSet<ReportRegistry>();
        }

        public int Id { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide unique code.")]
        [StringLength(50, MinimumLength = 2)]
        public string Code { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please, select status.")]
        public int StatusId { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedDateTime { get; set; }

        public EnumTypeCollection Status { get; set; }
        public ICollection<Feature> Features { get; set; }
        public ICollection<ReportRegistry> ReportRegistries { get; set; }
    }
}