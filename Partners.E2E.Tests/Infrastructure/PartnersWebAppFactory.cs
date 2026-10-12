using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Partners.Infra.Data;

namespace Partners.E2E.Tests.Infrastructure;

/// <summary>
/// Fábrica que arranca a API em memória para os testes E2E,
/// substituindo o DbContext do SQL Server por um InMemory isolado por teste.
/// </summary>
public class PartnersWebAppFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"PartnersE2E-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            // Remove os registos originais do AppDbContext (SQL Server) feitos pelo AddInfrastructure.
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();

            // Regista o AppDbContext em InMemory (isolado por teste via _databaseName).
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });
        });
    }

    /// <summary>
    /// Cria um scope para aceder ao DbContext e semear dados de teste.
    /// </summary>
    public async Task SeedAsync(Func<AppDbContext, Task> seedAction)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();
        await seedAction(db);
        await db.SaveChangesAsync();
    }
}
