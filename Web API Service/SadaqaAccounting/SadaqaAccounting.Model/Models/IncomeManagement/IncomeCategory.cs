namespace SadaqaAccounting.Model.Models.IncomeManagement
{
    [Table("IncomeCategories", Schema = "IncomeManagement")]
    public class IncomeCategory : IAuditableEntity, IDelatableEntity
    {
        public IncomeCategory()
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
        public bool IsDonorBased { get; set; }
        public string CreatedById { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDateTime { get; set; }

        public AccountUnit AccountUnit { get; set; }
        public ICollection<Income> Incomes { get; set; }
    }
}
