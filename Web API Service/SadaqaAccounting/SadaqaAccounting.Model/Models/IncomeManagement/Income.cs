namespace SadaqaAccounting.Model.Models.IncomeManagement
{
    [Table("Incomes", Schema = "IncomeManagement")]
    public class Income: IAuditableEntity, IDelatableEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please, select a unit.")]
        public int AccountUnitId { get; set; }

        [Required(ErrorMessage = "Please, select a category.")]
        public int IncomeCategoryId { get; set; }

        public int? DonorId { get; set; }
        public int? EventId { get; set; }
        public int? CashId { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string? ReceiptBookNo { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string? ReceiptNo { get; set; }

        [Required(ErrorMessage = "Please, provide receipt date.")]
        public DateTime ReceiptDate { get; set; }

        [Required(ErrorMessage = "Please, provide amount.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Please, provide payment mode.")]
        public int PaymentModeId { get; set; }

        public int? BankId { get; set; }
        public int? MonthId { get; set; }
        public int? Year { get; set; }
        public string? Description { get; set; }

        public string CreatedById { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDateTime { get; set; }

        public AccountUnit AccountUnit { get; set; }
        public IncomeCategory IncomeCategory { get; set; }
        public Donor Donor { get; set; }
        public Event Event { get; set; }
        public Bank Bank { get; set; }
        public EnumTypeCollection PaymentMode { get; set; }
        public Cash Cash { get; set; }
        public EnumTypeCollection Month { get; set; }
    }
}