namespace MsClean.Presentation.Extensions;

using Microsoft.EntityFrameworkCore;
using MsClean.Infrastructure;

public static class DatabaseMigrationExtension
{
    public static WebApplication ApplyMigrations(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated(); 
                context.Database.Migrate();
                SeedData(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al aplicar migraciones: {ex.Message}");
            }
        }

        return app;
    }

    public static void SeedData(ApplicationDbContext dbContext)
    {
        if (!dbContext.PermissionTypes.Any())
        {
            dbContext.PermissionTypes.AddRange([
                    new("Admin"){
                        UserRegister = "admin",
                        DateTimeRegister = DateTime.UtcNow,
                        IsActive = true
                    },
                    new("User"){
                        UserRegister = "admin",
                        DateTimeRegister = DateTime.UtcNow,
                        IsActive = true
                    }
            ]);
            dbContext.SaveChanges();
        }
    }

}
