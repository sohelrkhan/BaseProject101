namespace SadaqaAccounting.Model.Models.Cash_BankManagement
{
    [Table("Banks", Schema = "Cash&BankManagement")]
    public class Bank: IAuditableEntity, IDelatableEntity
    {
        public Bank()
        {
            Incomes = new HashSet<Income>();
            Expenses = new HashSet<Expense>();
            BankLedgers = new HashSet<BankLedger>();
            OpeningBalances = new HashSet<OpeningBalance>();
        }

        public int Id { get; set; }

        public int AccountUnitId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please, provide branch name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string BranchName { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide account number.")]
        [StringLength(maximumLength: 25, MinimumLength = 5)]
        public string AccountNumber { get; set; }

        public decimal OpeningBalance { get; set; }
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
        public ICollection<Expense> Expenses { get; set; }
        public ICollection<BankLedger> BankLedgers { get; set; }
        public ICollection<OpeningBalance> OpeningBalances { get; set; }
    }
}