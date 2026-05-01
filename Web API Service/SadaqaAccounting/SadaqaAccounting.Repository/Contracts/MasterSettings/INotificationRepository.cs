namespace SadaqaAccounting.Repository.Contracts.MasterSettings
{
    public interface INotificationRepository : IBaseRepository<Notifications>
    {
        Task<ICollection<Notifications>> GetAllUnreadNotificationByReceiverId(string receiverEmployeeId);
    }
}