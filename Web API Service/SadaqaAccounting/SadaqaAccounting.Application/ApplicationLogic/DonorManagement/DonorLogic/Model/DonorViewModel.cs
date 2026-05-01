using SadaqaAccounting.Model.Models.DonorManagement;

namespace SadaqaAccounting.Application.ApplicationLogic.DonorManagement.DonorLogic.Model
{
    public class DonorViewModel
    {
        public DonorCreateModel CreateModel { get; set; }
        public DonorUpdateModel UpdateModel { get; set; }
        public DonorGridModel GridModel { get; set; }
        public dynamic OptionsDataSources { get; set; } = new ExpandoObject();
        public string GenerateDonorCode { get; set; }
    }
    public class DonorCreateModel: IMapFrom<Donor>
    {
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Name { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Code { get; set; }
        public string MobileNo { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Donor,DonorCreateModel>();
            profile.CreateMap<DonorCreateModel, Donor>();
        }
    }
    public class DonorUpdateModel: IMapFrom<Donor>
    {
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Name { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Code { get; set; }
        public string MobileNo { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Donor, DonorUpdateModel>()
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.Name));
            profile.CreateMap<DonorUpdateModel, Donor>();
        }
    }
    public class DonorGridModel: IMapFrom<Donor>
    {
        public int Id { get; set; }
        public string EncryptedId { get; set; }
        public int AccountUnitId { get; set; }
        public string AccountUnitName { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string MobileNo { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Donor, DonorGridModel>()
             .ForMember(dest => dest.EncryptedId, s => s.MapFrom(m => EncryptionService.Encrypt(m.Id.ToString())))
             .ForMember(dest => dest.AccountUnitName, opt => opt.MapFrom(src => src.AccountUnit.Name))
             .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.Name));
            profile.CreateMap<DonorGridModel, Donor>();
        }
    }
}
