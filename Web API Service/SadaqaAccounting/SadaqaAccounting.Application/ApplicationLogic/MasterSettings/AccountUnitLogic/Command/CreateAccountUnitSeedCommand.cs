namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccountUnitLogic.Command
{
    public class CreateAccountUnitSeedCommand
    {
        private readonly IAccountUnitRepository _accountUnitRepository;
        private readonly ICashRepository _cashRepository;

        public CreateAccountUnitSeedCommand(
            IAccountUnitRepository accountUnitRepository,
            ICashRepository cashRepository)
        {
            _accountUnitRepository = accountUnitRepository;
            _cashRepository = cashRepository;
        }

        public async Task SeedAsync()
        {
            var seedAccountUnits = CreateAccountUnits().ToList();

            // Use Code as the stable unique key
            var seedCodes = seedAccountUnits.Select(x => x.Code).ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Fetch existing AccountUnits once 
            var existingAccountUnits = await _accountUnitRepository.GetByCodesAsync(seedCodes);
            var existingCodes = existingAccountUnits
                .Select(x => x.Code)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Insert missing AccountUnits
            var toInsertAccountUnits = seedAccountUnits
                .Where(x => !existingCodes.Contains(x.Code))
                .ToList();

            if (toInsertAccountUnits.Count > 0)
                await _accountUnitRepository.CreateBulkAsync(toInsertAccountUnits);

            // Load all required AccountUnits with IDs
            var allSeedAccountUnits = await _accountUnitRepository.GetByCodesAsync(seedCodes);

            // Build cash seeds using REAL AccountUnitId from database 
            var seedCashes = CreateSeedCashes(allSeedAccountUnits);

            // Fetch existing Cash once for those AccountUnitIds
            var unitIds = allSeedAccountUnits.Select(x => x.Id).ToHashSet();
            var existingCashes = await _cashRepository.GetByAccountUnitIdsAsync(unitIds);

            // Define uniqueness key for cash
            var existingCashKeys = existingCashes
                .Select(c => CashKey(c.AccountUnitId, c.Name))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Insert only missing cash
            var toInsertCash = seedCashes
                .Where(c => !existingCashKeys.Contains(CashKey(c.AccountUnitId, c.Name)))
                .ToList();

            if (toInsertCash.Count > 0)
                await _cashRepository.CreateBulkAsync(toInsertCash);
        }

        private static string CashKey(int accountUnitId, string name) => $"{accountUnitId}|{name?.Trim()}";

        private ICollection<AccountUnit> CreateAccountUnits()
        {
            var now = DateTime.UtcNow;

            return new List<AccountUnit>
            {
                new AccountUnit { Code = "MOSJID", Name = "Mosque Account", CreatedById = "Super Admin", CreatedDateTime = now },
                new AccountUnit { Code = "MAD_GEN", Name = "Madrasha General Account", CreatedById = "Super Admin", CreatedDateTime = now },
                new AccountUnit { Code = "MAD_ZAKAT", Name = "Madrasha Zakat Account", CreatedById = "Super Admin", CreatedDateTime = now }
            };
        }

        private ICollection<Cash> CreateSeedCashes(List<AccountUnit> accountUnits)
        {
            var now = DateTime.UtcNow;

            // Map by code Account Units so it’s stable
            var mapAcountUnits = accountUnits.ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase);

            return new List<Cash>
            {
                new Cash { AccountUnitId = mapAcountUnits["MOSJID"].Id, Name = "Cash Balance", Remarks = "N/A", CreatedById = "Administrator", CreatedDateTime = now },
                new Cash { AccountUnitId = mapAcountUnits["MAD_GEN"].Id, Name = "Cash Entry", Remarks = "N/A", CreatedById = "Administrator", CreatedDateTime = now },
                new Cash { AccountUnitId = mapAcountUnits["MAD_ZAKAT"].Id, Name = "Cash Value", Remarks = "N/A", CreatedById = "Administrator", CreatedDateTime = now }
            };
        }
    }
}