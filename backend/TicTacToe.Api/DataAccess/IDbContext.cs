using Microsoft.EntityFrameworkCore;

namespace TicTacToe.Api.DataAccess;

public interface IDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
