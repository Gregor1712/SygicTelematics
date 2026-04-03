using Shared.Kernel.Entities;

namespace Catalog.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
    Task<bool> Complete();
}