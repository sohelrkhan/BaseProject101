namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccountUnitLogic.Model
{
    public class AccountUnitViewModel
    {
        public AccountUnitCreateModel CreateModel { get; set; }
        public AccountUnitUpdateModel UpdateModel { get; set; }
        public AccountUnitGridModel GridModel { get; set; }
        public dynamic OptionsDataSources { get; set; } = new ExpandoObject();
    }

    public class AccountUnitCreateModel: IMapFrom<AccountUnit>
    {
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Code { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Name { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AccountUnit, AccountUnitCreateModel>();
            profile.CreateMap<AccountUnitCreateModel, AccountUnit>();
        }
    }

    public class AccountUnitUpdateModel : IMapFrom<AccountUnit>
    {
        public int Id { get; set; }
        public string EncryptedId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Code { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Name { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AccountUnit, AccountUnitUpdateModel>();
            profile.CreateMap<AccountUnitUpdateModel, AccountUnit>();
        }
    }

    public class AccountUnitGridModel : IMapFrom<AccountUnit>
    {
        public int Id { get; set; }
        public string EncryptedId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsExist { get; set; }
        public bool IsChecked { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AccountUnit, AccountUnitGridModel>();
            profile.CreateMap<AccountUnitGridModel, AccountUnit>();
        }
    }
}