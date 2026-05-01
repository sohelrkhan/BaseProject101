namespace SadaqaAccounting.Model.Models.MasterSettings
{
    public class User : IdentityUser
    {
        public User()
        {
            ReportUserAccesses = new HashSet<ReportUserAccess>();
            UserAccessMappings = new HashSet<UserAccessMapping>();
            UserAccountUnits = new HashSet<UserAccountUnit>();
        }

        [PersonalData]
        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide full name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string FullName { get; set; }
        
        public int? EmployeeId { get; set; }
        public bool ForcePasswordChanged { get; set; }
        public int ApplicationUserTypeId { get; set; }

        public Employee? Employee { get; set; }
        public EnumTypeCollection ApplicationUserType { get; set; }        
        public ICollection<ReportUserAccess> ReportUserAccesses { get; set; }
        public ICollection<UserAccessMapping> UserAccessMappings { get; set; }
        public ICollection<UserAccountUnit> UserAccountUnits { get; set; }
    }
}