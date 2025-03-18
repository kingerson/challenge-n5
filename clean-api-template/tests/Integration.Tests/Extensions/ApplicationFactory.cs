namespace Integration.Tests.Extensions;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MsClean.Domain;
using MsClean.Infrastructure;

public class ApplicationFactory : WebApplicationFactory<MsClean.Presentation.Program>
{
    private readonly SqliteConnection _connection;
    public ApplicationFactory()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", "Data Source=:memory:" }
            }));

        builder.ConfigureServices(services =>
        {
            services.AddHttpClient();
            services.RemoveAll<IConfigureOptions<JwtBearerOptions>>();

            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

            services.PostConfigure<AuthenticationOptions>(o =>
            {
                o.DefaultAuthenticateScheme = "Test";
                o.DefaultChallengeScheme = "Test";
                o.DefaultScheme = "Test";
            });

            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            Environment.SetEnvironmentVariable("USE_INMEMORY_DATABASE", "true");

            services.AddDbContext<ApplicationDbContext>(options => options
                    .UseSqlite(_connection)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
                    .AddInterceptors(new FakeInterceptor()));

        });

        var host = base.CreateHost(builder);

        using (var scope = host.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();
            SeedData(context);
        }

        return host;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection.Dispose();
    }

    public static void SeedData(ApplicationDbContext dbContext)
    {
        if (!dbContext.PermissionTypes.Any())
        {
            dbContext.PermissionTypes.AddRange([
                new("Admin"){
                    Id = 1,
                    UserRegister = "admin",
                    DateTimeRegister = DateTime.UtcNow,
                    IsActive = true
                },
                new("User"){
                    Id = 2,
                    UserRegister = "admin",
                    DateTimeRegister = DateTime.UtcNow,
                    IsActive = true
                }
            ]);
            dbContext.SaveChanges();
        }

        dbContext.SaveChanges();

        if (!dbContext.Permissions.Any())
        {
            var permissionOne = new Permission
            {
                Id = 1,
                UserRegister = "admin",
                DateTimeRegister = DateTime.UtcNow,
                IsActive = true
            };
            permissionOne.Register("Gerson", "Navarro", 1, DateTime.UtcNow);

            var permissionTwo = new Permission
            {
                Id = 2,
                UserRegister = "admin",
                DateTimeRegister = DateTime.UtcNow,
                IsActive = true
            };
            permissionTwo.Register("Eduardo", "Navarro", 1, DateTime.UtcNow);

            dbContext.Permissions.AddRange([permissionOne,permissionTwo]);

            dbContext.SaveChanges();
        }
    }

}
