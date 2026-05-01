namespace SadaqaAccounting.Application.ApplicationLogic.ExpenseManagement.ExpenseCategory.Model
{
    public class ExpenseCategoryViewModel
    {
        public ExpenseCategoryCreateModel CreateModel { get; set; }
        public ExpenseCategoryUpdateModel UpdateModel { get; set; }
        public ExpenseCategoryGridModel GridModel { get; set; }
        public dynamic OptionsDataSources { get; set; } = new ExpandoObject();
    }

    public class ExpenseCategoryCreateModel : IMapFrom<SadaqaAccounting.Model.Models.ExpenseManagement.ExpenseCategory>
    {
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Name { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SadaqaAccounting.Model.Models.ExpenseManagement.ExpenseCategory, ExpenseCategoryCreateModel>();
            profile.CreateMap<ExpenseCategoryCreateModel, SadaqaAccounting.Model.Models.ExpenseManagement.ExpenseCategory>();
        }
    }

    public class ExpenseCategoryUpdateModel : IMapFrom<SadaqaAccounting.Model.Models.ExpenseManagement.ExpenseCategory>
    {
        public int Id { get; set; }
        public string? EncryptedId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Name { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SadaqaAccounting.Model.Models.ExpenseManagement.ExpenseCategory, ExpenseCategoryUpdateModel>();
            profile.CreateMap<ExpenseCategoryUpdateModel, SadaqaAccounting.Model.Models.ExpenseManagement.ExpenseCategory>();
        }
    }

    public class ExpenseCategoryGridModel : IMapFrom<SadaqaAccounting.Model.Models.ExpenseManagement.ExpenseCategory>
    {
        public int Id { get; set; }
        public string? EncryptedId { get; set; }
        public string AccountUnitName { get; set; }
        public string Name { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SadaqaAccounting.Model.Models.ExpenseManagement.ExpenseCategory, ExpenseCategoryGridModel>()
                .ForMember(d => d.EncryptedId, s => s.MapFrom(m => EncryptionService.Encrypt(m.Id.ToString())))
                .ForMember(d => d.AccountUnitName, s => s.MapFrom(m => m.AccountUnit.Name));
        }
    }
}