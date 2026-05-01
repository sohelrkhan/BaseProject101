namespace SadaqaAccounting.Model.Models.ExpenseManagement
{
    [Table("Expenses", Schema = "ExpenseManagement")]
    public class Expense: IAuditableEntity, IDelatableEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please, select a unit.")]
        public int AccountUnitId { get; set; }

        [Required(ErrorMessage = "Please, select a category.")]
        public int ExpenseCategoryId { get; set; }

        public int? EventId { get; set; }
        public DateTime ExpenseDate { get; set; }
        public decimal Amount { get; set; }
        public int PaymentModeId { get; set; }
        public int? BankId { get; set; }
        public int? CashId { get; set; }
        public string? Description { get; set; }

        public string CreatedById { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDateTime { get; set; }

        public AccountUnit AccountUnit { get; set; }
        public ExpenseCategory ExpenseCategory { get; set; }
        public Event Event { get; set; }
        public Bank Bank { get; set; }
        public Cash Cash { get; set; }
        public EnumTypeCollection PaymentMode { get; set; }
    }
}