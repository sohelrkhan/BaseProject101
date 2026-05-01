using SadaqaAccounting.Model.Models.AssetManagement;
using SadaqaAccounting.Repository.Contracts.AssetManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadaqaAccounting.Repository.Repository.AssetManagement
{
    public class AssettRepository : BaseRepository<Asset>, IAssettRepository
    {
        public AssettRepository(DatabaseContext databaseContext) : base(databaseContext) { }
    }
}
