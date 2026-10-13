using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Partners.Aplication.Mappings;
using Partners.Application;
using Partners.Domain.Interfaces;
using Partners.Infra.Data;

namespace Partners.Infra.Ioc
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            return services;
        }

        public static IServiceCollection ConfigureRepositoryDependencies(this IServiceCollection services)
        {
            services.AddScoped<IPartnerRepository, PartnerRepository>();
            services.AddScoped<IPartnerService, PartnerService>();
            services.AddAutoMapper(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            return services;
        }
    }
}
