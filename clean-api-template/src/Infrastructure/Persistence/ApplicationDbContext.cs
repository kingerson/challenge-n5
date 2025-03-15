namespace MsClean.Infrastructure;
using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MsClean.Domain;

public class ApplicationDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    private readonly EntityInterceptor _entityInterceptor;
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IConfiguration configuration,
        EntityInterceptor entityInterceptor
        ) : base(options)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _entityInterceptor = entityInterceptor ?? throw new ArgumentNullException(nameof(entityInterceptor));
    }

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configurationPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Presentation", "appsettings.json");

            var configuration = new ConfigurationBuilder()
                                    .SetBasePath(Path.GetDirectoryName(configurationPath)!).AddJsonFile(configurationPath).Build();
                                    
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            DbContextOptionsBuilder<ApplicationDbContext> builder = new();
            builder.UseSqlServer(connectionString)
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .AddInterceptors(new EntityInterceptor());
            return new ApplicationDbContext(builder.Options, configuration, new EntityInterceptor());
        }
    }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<PermissionType> PermissionTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        var useInMemory = Environment.GetEnvironmentVariable("USE_INMEMORY_DATABASE");

        if (!(useInMemory == "true"))
        {
            optionsBuilder.UseSqlServer(connectionString)
                         .EnableDetailedErrors()
                         .EnableSensitiveDataLogging()
                         .AddInterceptors(_entityInterceptor);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PermissionConfiguration());
        modelBuilder.ApplyConfiguration(new PermissionTypeConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
