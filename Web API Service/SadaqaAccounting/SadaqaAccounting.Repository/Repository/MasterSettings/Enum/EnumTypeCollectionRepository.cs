namespace SadaqaAccounting.Repository.Repository.MasterSettings.Enum
{
    public class EnumTypeCollectionRepository : BaseRepository<EnumTypeCollection>, IEnumTypeCollectionRepository
    {
        public EnumTypeCollectionRepository(DatabaseContext databaseContext) : base(databaseContext) { }

        public override async Task<EnumTypeCollection?> GetByIdAsync(int id)
        {
            var getEnum = await dbContext.EnumTypeCollections
                .Where(c => c.Id == id && !c.IsDeleted)
                .FirstOrDefaultAsync();

            return getEnum;
        }

        public async Task<List<string>> GetEnumTypeCollectionByIds(List<int> ids)
        {
            if(ids.Count() > 0)
            {
                // Store enum type collection name
                var enumTypeCollections = new List<string>();

                foreach(int id in ids)
                {
                    var name = await dbContext.EnumTypeCollections
                        .Where(etc => etc.Id == id && !etc.IsDeleted)
                        .Select(etc => etc.Name)
                        .FirstOrDefaultAsync();

                    if(name is not null)
                        enumTypeCollections.Add(name);
                }

                return enumTypeCollections;
            }

            return new List<string>();
        }
    }
}