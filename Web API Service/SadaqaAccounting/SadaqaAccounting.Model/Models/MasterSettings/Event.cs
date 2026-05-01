using SadaqaAccounting.Model.Models.ExpenseManagement;
using SadaqaAccounting.Model.Models.IncomeManagement;

namespace SadaqaAccounting.Model.Models.MasterSettings
{
    [Table("Events", Schema = "MasterSettings")]
    public class Event: IAuditableEntity, IDelatableEntity
    {
        public Event()
        {
            Incomes = new HashSet<Income>();
            Expenses = new HashSet<Expense>();
        }
        public int Id { get; set; }
        public int AccountUnitId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public string CreatedById { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDateTime { get; set; }
        public AccountUnit AccountUnit { get; set; }
        public ICollection<Income> Incomes { get; set; }
        public ICollection<Expense> Expenses { get; set; }
    }
}
