using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence;

public class UnitOfWork(AppDbContext dbContext, ILogger<UnitOfWork> logger) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            foreach (var entry in ex.Entries)
            {
                logger.LogDebug($"Concurrency entity: {entry.Entity.GetType().Name}, state: {entry.State}");
            }

            throw;
        }
    }
}
