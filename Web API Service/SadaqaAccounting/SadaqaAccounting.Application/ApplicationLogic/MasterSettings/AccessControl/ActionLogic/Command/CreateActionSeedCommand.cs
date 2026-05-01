namespace SadaqaAccounting.Application.ApplicationLogic.MasterSettings.AccessControl.ActionLogic.Command
{
    public class CreateActionSeedCommand
    {
        private IActionRepository _actionRepository;

        public CreateActionSeedCommand(IActionRepository actionRepository)
        {
            _actionRepository = actionRepository;
        }

        public async Task SeedAsync()
        {
            // Get newly action collection
            var actions = CreateAction();

            // For actions
            foreach (var action in actions)
            {
                // Check action is exist or not
                var isActionExit = await _actionRepository.GetActionByName(action.Name);

                if (isActionExit == null)
                    await _actionRepository.CreateAsync(action);
            }
        }

        // Create seed action
        private ICollection<SadaqaAccounting.Model.Models.MasterSettings.AccessControl.Action> CreateAction()
        {
            return new List<SadaqaAccounting.Model.Models.MasterSettings.AccessControl.Action>()
            {
                new SadaqaAccounting.Model.Models.MasterSettings.AccessControl.Action { Name = "Create", StatusId = GlobalStatus.Active, IsDeleted = false, DeletedDateTime = null },
                new SadaqaAccounting.Model.Models.MasterSettings.AccessControl.Action { Name = "Update", StatusId = GlobalStatus.Active, IsDeleted = false, DeletedDateTime = null },
                new SadaqaAccounting.Model.Models.MasterSettings.AccessControl.Action { Name = "Delete", StatusId = GlobalStatus.Active, IsDeleted = false, DeletedDateTime = null },
                new SadaqaAccounting.Model.Models.MasterSettings.AccessControl.Action { Name = "List", StatusId = GlobalStatus.Active, IsDeleted = false, DeletedDateTime = null }
            };
        }
    }
}