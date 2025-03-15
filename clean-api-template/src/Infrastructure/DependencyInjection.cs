namespace MsClean.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MsClean.Infraestructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ConnectionEntity");

        _ = services.AddTransient<EntityInterceptor>();

        _ = services.AddDbContext<ApplicationDbContext>(m =>
                m.UseSqlServer(connectionString)
                .EnableDetailedErrors()
                .AddInterceptors(new EntityInterceptor())
            );

        _ = services.AddHealthChecks().AddSqlServer(connectionString);

        _ = services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        _ = services.AddScoped<IUnitOfWork, UnitOfWork>();
        _ = services.AddScoped<IMemoryCacheService, MemoryCacheService>();
        _ = services.AddScoped<IHttpService, HttpService>();
        _ = services.AddScoped<IExecutionStrategyWrapper, ExecutionStrategyWrapper>();
        _ = services.AddScoped<IKakfaService, KakfaService>();
        _ = services.AddScoped(typeof(IElasticSearchService<>), typeof(ElasticSearchService<>));

        return services;
    }
}
