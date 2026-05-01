namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.UserAccountUnitLogic.Model
{
    public class UserAccountUnitViewModel
    {
        public UserAccountUnitCreateModel CreateModel { get; set; }
        public UserAccountUnitGridModel GridModel { get; set; }
    }

    public class  UserAccountUnitCreateModel: IMapFrom<UserAccountUnit>
    {
        public string UserId { get; set; }
        public int[] AccountUnitIds { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<UserAccountUnit, UserAccountUnitCreateModel>();
            profile.CreateMap<UserAccountUnitCreateModel, UserAccountUnit>();
        }
    }

    public class UserAccountUnitGridModel : IMapFrom<UserAccountUnit>
    {
        public int Id { get; set; }
        public string EncryptId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public ICollection<AccountUnitGridModel> AccountUnitList { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<UserAccountUnit, UserAccountUnitGridModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));
            profile.CreateMap<UserAccountUnitGridModel, UserAccountUnit>();
        }
    }
}