namespace SadaqaAccounting.Model.Models.DonorManagement
{
    [Table("Donors", Schema = "DonorManagement")]
    public class Donor : IAuditableEntity, IDelatableEntity
    {
        public Donor()
        {
            Incomes = new HashSet<Income>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Please, select a unit.")]
        public int AccountUnitId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Code { get; set; }

        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string? Address { get; set; }
        public int StatusId { get; set; }

        public string CreatedById { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDateTime { get; set; }

        public AccountUnit AccountUnit { get; set; }
        public EnumTypeCollection Status { get; set; }
        public ICollection<Income> Incomes { get; set; }
    }
}