namespace MsClean.Infrastructure;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class ExecutionStrategyWrapper : IExecutionStrategyWrapper
{
    private readonly ApplicationDbContext _context;

    public ExecutionStrategyWrapper(ApplicationDbContext context)
       => _context = context ?? throw new ArgumentNullException(nameof(context));
    public async Task ExecuteAsync(Func<Task> operation)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(operation);
    }
}
