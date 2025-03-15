namespace MsClean.Presentation;

using MsClean.Presentation.Extensions;

public partial class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication
            .CreateBuilder(args)
            .ConfigureApplicationBuilder();

        var app = builder
            .Build()
            .ConfigureApplication()
            .ApplyMigrations();

        await app.RunAsync();
    }
}
