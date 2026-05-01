public class UnitOfWork : IUnitOfWork
{
    private readonly DatabaseContext _context;
    private IDbContextTransaction _transaction;

    public UnitOfWork(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
        return _transaction;
    }

    public async Task CommitTransactionAsync()
    {
        await _transaction?.CommitAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await _transaction?.RollbackAsync();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context?.Dispose();
    }
}