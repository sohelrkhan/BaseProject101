namespace SadaqaAccounting.Application.ApplicationLogic.ExpenseManagement.ExpenseLogic.Model
{
    public class ExpenseViewModel
    {
        public ExpenseCreateModel CreateModel { get; set; }
        public ExpenseUpdateModel UpdateModel { get; set; }
        public ExpenseGridModel GridModel { get; set; }
        public dynamic OptionsDataSources { get; set; } = new ExpandoObject();
    }

    public class ExpenseCreateModel : IMapFrom<Expense>
    {
        [Required(ErrorMessage = "Please, select a unit.")]
        public int AccountUnitId { get; set; }

        [Required(ErrorMessage = "Please, select a category.")]
        public int ExpenseCategoryId { get; set; }

        public int? EventId { get; set; }

        [Required(ErrorMessage = "Please, provide expense date.")]
        public string ExpenseDateString { get; set; }

        [Required(ErrorMessage = "Please, provide amount.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Please, provide payment mood.")]
        public int PaymentModeId { get; set; }

        public int? BankId { get; set; }
        public int? CashId { get; set; }
        public string? Description { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ExpenseCreateModel, Expense>();
            profile.CreateMap<Expense, ExpenseCreateModel>();
        }
    }

    public class ExpenseUpdateModel : IMapFrom<Expense>
    {
        public int Id { get; set; }
        [NotMapped] public string? EncryptedId { get; set; }

        [Required(ErrorMessage = "Please, select a unit.")]
        public int AccountUnitId { get; set; }

        [Required(ErrorMessage = "Please, select a category.")]
        public int ExpenseCategoryId { get; set; }

        public int? EventId { get; set; }

        [Required(ErrorMessage = "Please, provide expense date.")]
        public string ExpenseDateString { get; set; }

        [Required(ErrorMessage = "Please, provide amount.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Please, provide payment mood.")]
        public int PaymentModeId { get; set; }

        public int? BankId { get; set; }
        public int? CashId { get; set; }
        public string? Description { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ExpenseUpdateModel, Expense>();
            profile.CreateMap<Expense, ExpenseUpdateModel>();
        }
    }

    public class ExpenseGridModel : IMapFrom<Expense>
    {
        public int Id { get; set; }
        [NotMapped] public string? EncryptedId { get; set; }
        public string? AccountUnitName { get; set; }
        public string ExpenseCategoryName { get; set; }
        public string? EventName { get; set; }
        public DateTime ExpenseDate { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentModeName { get; set; }
        public int? BankId { get; set; }
        public string? BankName { get; set; }
        public int CashId { get; set; }
        public string? CashName { get; set; }
        public string? Description { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Expense, ExpenseGridModel>()
                .ForMember(d => d.EncryptedId, s => s.MapFrom(m => EncryptionService.Encrypt(m.Id.ToString())))
                .ForMember(d => d.AccountUnitName, s => s.MapFrom(m => m.AccountUnit.Name))
                .ForMember(d => d.ExpenseCategoryName,s => s.MapFrom(m => m.ExpenseCategory.Name))
                .ForMember(d => d.EventName,s => s.MapFrom(m => m.Event.Name))
                .ForMember(d => d.PaymentModeName, s => s.MapFrom(m => m.PaymentMode.Name))
                .ForMember(d => d.BankName,s => s.MapFrom(m => m.Bank.Name));
        }
    }
}