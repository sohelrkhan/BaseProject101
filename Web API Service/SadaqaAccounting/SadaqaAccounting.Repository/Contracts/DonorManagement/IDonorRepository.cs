namespace SadaqaAccounting.Repository.Contracts.DonorManagement
{
    public interface IDonorRepository: IBaseRepository<Donor>
    {
        Task<ICollection<Donor>> GetAllAccountUnitWiseDonorAsync(int id);
        Task<ICollection<SelectModel>> GetSelectListDonorAsync(int id);
        Task<IEnumerable<SelectModel>> GetDonerSelectListByAccountUnitAsync(int accountUnitId, CancellationToken cancellationToken);
        Task<PaginatedResponse<Donor>> GetDonorsFilterAsync(PaginationRequest paginationRequest, CancellationToken cancellationToken = default);
        Task<bool> CheckDuplicateDonorByNameAndMobileAsync(string name, string mobileNo);
        Task<string> GetUniqueDonorCodeAsync();
    }
}