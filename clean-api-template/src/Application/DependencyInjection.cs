namespace MsClean.Application;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        _ = services.AddMediatR(Assembly.GetExecutingAssembly());
        _ = services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient);
        _ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        _ = services.AddAutoMapper(Assembly.GetExecutingAssembly());

        _ = services.AddTransient<IPermissionRepository, PermissionRepository>();

        return services;
    }
}
