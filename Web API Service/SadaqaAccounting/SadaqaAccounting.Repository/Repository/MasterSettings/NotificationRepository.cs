namespace SadaqaAccounting.Repository.Repository.MasterSettings
{
    public class NotificationRepository : BaseRepository<Notifications>, INotificationRepository
    {
        public NotificationRepository(DatabaseContext databaseContext) : base(databaseContext) { }

        public override async Task<ICollection<Notifications>> GetAllAsync()
        {
            var getAllNotifications = await dbContext.Notifications
                .AsNoTracking()
                .Where(n => !n.IsDeleted)
                .ToListAsync();

            return getAllNotifications;
        }

        public override async Task<Notifications?> GetByIdAsync(int id)
        {
            var notifications = await dbContext.Notifications
                .Where(n => n.Id == id && !n.IsDeleted)
                .FirstOrDefaultAsync();

            return notifications!;
        }

        public async Task<ICollection<Notifications>> GetAllUnreadNotificationByReceiverId(string receiverEmployeeId)
        {
           var getAllnotifications = await dbContext.Notifications
                .Where (n => !n.IsDeleted && n.ReceiverUserId!.ToLower() == receiverEmployeeId.ToLower() && !n.IsRead)
                .OrderByDescending(o => o.Id)
                .ToListAsync();

            return getAllnotifications;
        }
    }
}