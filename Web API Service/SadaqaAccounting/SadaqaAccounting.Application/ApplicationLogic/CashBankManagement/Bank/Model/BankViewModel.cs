namespace SadaqaAccounting.Application.ApplicationLogic.CashBankManagement.Bank.Model
{
    public class BankViewModel
    {
        public BankCreateModel CreateModel { get; set; }
        public BankUpdateModel UpdateModel { get; set; }
        public BankGridModel GridModel { get; set; }
        public dynamic OptionsDataSources { get; set; } = new ExpandoObject();
    }

    public class BankCreateModel : IMapFrom<SadaqaAccounting.Model.Models.Cash_BankManagement.Bank>
    {
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

        public void Mapping(Profile profile)
        {
            profile.CreateMap<BankCreateModel, SadaqaAccounting.Model.Models.Cash_BankManagement.Bank>();
        }
    }

    public class BankUpdateModel : IMapFrom<SadaqaAccounting.Model.Models.Cash_BankManagement.Bank>
    {
        public int Id { get; set; }
        [NotMapped] public string? EncryptedId { get; set; }

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

        [Required(ErrorMessage = "Please, provide status.")]
        public int StatusId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<BankUpdateModel, SadaqaAccounting.Model.Models.Cash_BankManagement.Bank>();
            profile.CreateMap<SadaqaAccounting.Model.Models.Cash_BankManagement.Bank, BankUpdateModel>();
        }
    }

    public class BankGridModel : IMapFrom<SadaqaAccounting.Model.Models.Cash_BankManagement.Bank>
    {
        public int Id { get; set; }
        [NotMapped] public string? EncryptedId { get; set; }

        public int AccountUnitId { get; set; }
        public string AccountUnitName { get; set; }
        public string Name { get; set; }
        public string BranchName { get; set; }
        public string AccountNumber { get; set; }
        public decimal OpeningBalance { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SadaqaAccounting.Model.Models.Cash_BankManagement.Bank, BankGridModel>()
                .ForMember(d => d.EncryptedId, s => s.MapFrom(m => (EncryptionService.Encrypt(m.Id.ToString()))))
                .ForMember(d => d.StatusName, s => s.MapFrom(m => m.Status.Name));
        }
    }
}