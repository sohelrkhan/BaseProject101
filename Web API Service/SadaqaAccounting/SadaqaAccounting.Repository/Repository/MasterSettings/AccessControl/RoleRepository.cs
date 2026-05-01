namespace SadaqaAccounting.Repository.Repository.MasterSettings.AccessControl
{
    public class RoleRepository : BaseRepository<Role>,IRoleRepository
    {
        public RoleRepository(DatabaseContext databaseContext) : base(databaseContext) { }
    }
}