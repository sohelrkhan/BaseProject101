namespace SadaqaAccounting.Application.ApplicationLogic.IncomeManagement.IncomeLogic.Model
{
    public class IncomeViewModel
    {
        public IncomeCreateModel CreateModel { get; set; }
        public IncomeUpdateModel UpdateModel { get; set; }
        public IncomeGridModel GridModel { get; set; }
        public dynamic OptionsDataSources { get; set; } = new ExpandoObject();
    }

    public class IncomeCreateModel : IMapFrom<Income>
    {
        [Required(ErrorMessage = "Please, provide account unit.")]
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
        public string ReceiptDateString { get; set; }

        [Required(ErrorMessage = "Please, provide amount.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Please, provide payment mode.")]
        public int PaymentModeId { get; set; }

        public int? BankId { get; set; }
        public int? MonthId { get; set; }
        public int? Year { get; set; }
        public string? Description { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<IncomeCreateModel, Income>();
            profile.CreateMap<Income, IncomeCreateModel>();
        }
    }

    public class IncomeUpdateModel : IMapFrom<Income>
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please, provide account unit.")]
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
        public string ReceiptDateString { get; set; }

        [Required(ErrorMessage = "Please, provide amount.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Please, provide payment mode.")]
        public int PaymentModeId { get; set; }

        public int? BankId { get; set; }
        public int? MonthId { get; set; }
        public int? Year { get; set; }
        public string? Description { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<IncomeUpdateModel, Income>();
            profile.CreateMap<Income, IncomeUpdateModel>();
        }
    }

    public class IncomeGridModel: IMapFrom<Income>
    {
        public int Id { get; set; }
        [NotMapped] public string? EncryptedId { get; set; }
        public string? AccountUnitName { get; set; }
        public string IncomeCategoryName { get; set; }
        public string? EventName { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string? DonorName { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentModeName { get; set; }
        public string? BankName { get; set; }
        public string? CashName { get; set; }
        public string? ReceiptBookNo { get; set; }
        public string? ReceiptNo { get; set; }
        public string? Description { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Income, IncomeGridModel>()
                .ForMember(d => d.EncryptedId, s => s.MapFrom(m => EncryptionService.Encrypt(m.Id.ToString())))
                .ForMember(d => d.AccountUnitName, s => s.MapFrom(m => m.AccountUnit.Name))
                .ForMember(d => d.IncomeCategoryName, s => s.MapFrom(m => m.IncomeCategory.Name))
                .ForMember(d => d.EventName, s => s.MapFrom(m => m.Event.Name))
                .ForMember(d => d.DonorName, s => s.MapFrom(m => m.Donor.Name))
                .ForMember(d => d.PaymentModeName, s => s.MapFrom(m => m.PaymentMode.Name))
                .ForMember(d => d.BankName, s => s.MapFrom(m => m.Bank.Name))
                .ForMember(d => d.CashName, s => s.MapFrom(m => m.Cash.Name));
        }
    }
}