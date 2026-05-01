namespace SadaqaAccounting.Model.Models.CashBankManagement
{
    [Table("Cashes", Schema = "Cash&BankManagement")]
    public class Cash : IAuditableEntity, IDelatableEntity
    {
        public Cash()
        {
            Expenses = new HashSet<Expense>();
            OpeningBalances = new HashSet<OpeningBalance>();
            CashLedgers = new HashSet<CashLedger>();
            Incomes = new HashSet<Income>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Please, provide Account Unit")]
        public int AccountUnitId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Name { get; set; }

        public string? Remarks { get; set; }

        public string CreatedById { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDateTime { get; set; }

        public AccountUnit AccountUnit { get; set; }
        public ICollection<Expense> Expenses { get; set; }
        public ICollection<OpeningBalance> OpeningBalances { get; set; }
        public ICollection<CashLedger> CashLedgers { get; set; }
        public ICollection<Income> Incomes { get; set; }
    }
}