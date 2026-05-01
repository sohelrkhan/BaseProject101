namespace SadaqaAccounting.Model.Models.MasterSettings.Enum
{
    public class EnumTypeCollection
    {
        public EnumTypeCollection()
        {
            #region Master Settings
            Employees = new HashSet<Employee>();
            Companies = new HashSet<Company>();
            Modules = new HashSet<Module>();
            Features = new HashSet<Feature>();
            Actions = new HashSet<AccessControl.Action>();
            Roles = new HashSet<Role>();
            ReportRegistries = new HashSet<ReportRegistry>();
            Banks = new HashSet<Bank>();
            IncomeMonths = new HashSet<Income>();
            #endregion
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please, selected enum type.")]
        public int EnumTypeId { get; set; }

        [Required(ErrorMessage = "Please, provide deleted or not.")]
        public bool IsDeleted { get; set; }

        #region Master Settings
        public EnumType EnumType { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public ICollection<Company> Companies { get; set; }
        public ICollection<Module> Modules { get; set; }
        public ICollection<Feature> Features { get; set; }
        public ICollection<AccessControl.Action> Actions { get; set; }
        public ICollection<Role> Roles { get; set; }
        public ICollection<ReportRegistry> ReportRegistries { get; set; }
        public ICollection<Bank> Banks { get; set; }
        public ICollection<Income> IncomeMonths { get; set; }
        #endregion
    }
}