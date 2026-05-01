using SadaqaAccounting.Model.Models.IncomeManagement;

namespace SadaqaAccounting.Application.ApplicationLogic.IncomeManagement.IncomeCategoryLogic.Model
{
    public class IncomeCategoryViewModel
    {
        public IncomeCategoryCreateModel CreateModel { get; set; }
        public IncomeCategoryUpdateModel UpdateModel { get; set; }
        public IncomeCategoryGridModel GridModel { get; set; }
    }
    public class IncomeCategoryCreateModel: IMapFrom<IncomeCategory>
    {
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Name { get; set; }
        public bool IsDonorBased { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<IncomeCategory, IncomeCategoryCreateModel>();
            profile.CreateMap<IncomeCategoryCreateModel, IncomeCategory>();
        }
    }
    public class IncomeCategoryUpdateModel: IMapFrom<IncomeCategory>
    {
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Name { get; set; }
        public bool IsDonorBased { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<IncomeCategory, IncomeCategoryUpdateModel>();
            profile.CreateMap<IncomeCategoryUpdateModel, IncomeCategory>();
        }
    }
    public class IncomeCategoryGridModel: IMapFrom<IncomeCategory>
    {
        public int Id { get; set; }
        public string EncryptedId { get; set; }
        public int AccountUnitId { get; set; }
        public string Name { get; set; }
        public bool IsDonorBased { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<IncomeCategory, IncomeCategoryGridModel>()
                .ForMember(d => d.EncryptedId, s => s.MapFrom(m => EncryptionService.Encrypt(m.Id.ToString())));
            profile.CreateMap<IncomeCategoryGridModel, IncomeCategory>();
        }
    }
}
