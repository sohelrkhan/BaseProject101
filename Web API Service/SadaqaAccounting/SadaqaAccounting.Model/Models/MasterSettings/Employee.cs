namespace SadaqaAccounting.Model.Models.MasterSettings
{
    [Table("Employees", Schema = "MasterSettings")]
    public class Employee : IAuditableEntity, IDelatableEntity
    {
        public Employee()
        {
            Users = new List<User>();
        }

        public int Id { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide full name.")]
        [StringLength(50, MinimumLength = 2)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Please, provide company.")]
        public int CompanyId { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Image { get; set; }
        public string? Address { get; set; }
        public int StatusId { get; set; }
        public string CreatedById { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDateTime { get; set; }

        public Company Company { get; set; }
        public EnumTypeCollection Status { get; set; }
        public ICollection<User> Users { get; set; }
    }
}