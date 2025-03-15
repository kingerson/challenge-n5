namespace MsClean.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using MsClean.Domain;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : Entity;
    Task<int> SaveEntitiesAsync(CancellationToken cancellationToken);
    Task<IDbContextTransaction> BeginTransactionAsync();
    IExecutionStrategy CreateExecutionStrategy();
}
