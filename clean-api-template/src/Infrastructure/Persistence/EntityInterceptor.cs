namespace MsClean.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MsClean.Domain;

public class EntityInterceptor : SaveChangesInterceptor
{
    public async override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public static void UpdateEntities(DbContext context)
    {
        if (context == null)
            return;

        foreach (var entry in context.ChangeTracker.Entries<Entity>())
        {
            var user = "user";

            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.DateTimeRegister = DateTime.Now;
                    entry.Entity.UserRegister = user;
                    entry.Entity.IsActive = true;
                    break;

                case EntityState.Modified:
                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted:
                    entry.Entity.DateTimeUpdated = DateTime.Now;
                    entry.Entity.UserUpdated = user;
                    break;

                default:
                    break;
            }
        }
    }
}
