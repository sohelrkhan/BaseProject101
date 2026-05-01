namespace SadaqaAccounting.Application.ApplicationLogic.CashBankManagement.OpeningBalanceLogic.Model
{
    public class OpeningBalanceViewModel
    {
        public OpeningBalanceCreateModel CreateModel { get; set; }
        public OpeningBalanceUpdateModel UpdateModel { get; set; }
        public OpeningBalanceGridModel GridModel { get; set; }
        public dynamic OptionsDataSources { get; set; } = new ExpandoObject();
    }

    public class OpeningBalanceCreateModel : IMapFrom<OpeningBalance>
    {
        [Required(ErrorMessage = "Please, provide account unit.")]
        public int AccountUnitId { get; set; }

        [Required(ErrorMessage = "Please, provide payment mode.")]
        public int PaymentModeId { get; set; }

        public int? BankId { get; set; }
        public int? CashId { get; set; }

        [Required(ErrorMessage = "Please, provide amount.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Please, provide opening date.")]
        public string OpeningDateText { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<OpeningBalanceCreateModel, OpeningBalance>();
        }
    }

    public class OpeningBalanceUpdateModel : IMapFrom<OpeningBalance>
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please, provide account unit.")]
        public int AccountUnitId { get; set; }

        [Required(ErrorMessage = "Please, provide payment mode.")]
        public int PaymentModeId { get; set; }

        public int? BankId { get; set; }
        public int? CashId { get; set; }

        [Required(ErrorMessage = "Please, provide amount.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Please, provide opening date.")]
        public string OpeningDateText { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<OpeningBalanceUpdateModel, OpeningBalance>();
            profile.CreateMap<OpeningBalance, OpeningBalanceUpdateModel>();
        }
    }

    public class OpeningBalanceGridModel : IMapFrom<OpeningBalance>
    {
        public int Id { get; set; }
        [NotMapped] public string? EncryptedId { get; set; }
        public string PaymentModeName { get; set; }
        public string? BankName { get; set; }
        public string? CashName { get; set; }
        public decimal Amount { get; set; }
        public DateTime OpeningDate { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<OpeningBalance, OpeningBalanceGridModel>()
                .ForMember(d => d.EncryptedId, s => s.MapFrom(m => EncryptionService.Encrypt(m.Id.ToString())))
                .ForMember(d => d.PaymentModeName, s => s.MapFrom(m => m.PaymentMode.Name))
                .ForMember(d => d.BankName, s => s.MapFrom(m => m.Bank != null ? m.Bank.Name : null))
                .ForMember(d => d.CashName, s => s.MapFrom(m => m.Cash != null ? m.Cash.Name : null));
        }
    }
}