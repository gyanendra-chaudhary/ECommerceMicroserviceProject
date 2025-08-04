using ECommerce.SharedLibrary.DependencyInjection;
using ECommerce.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;

namespace ProductApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            // Add database connectivity
            // Add authentication scheme
            SharedServiceContainer.AddSharedServices<ProductDbContext>(services, configuration, configuration["MySerilog:FileName"]!);

            // Create dependency injection DI
            services.AddScoped<IProduct, ProductRepository>();

            return services;
        }
    }
    public static IApplicationBuilder UseInfrastructurePolicies(this IApplicationBuilder app)
        {
            // Register middlewars
            // Global Exception: handles external errors.
            // Listens to only API Gateway: blocks outsiders API calls.
            SharedServiceContainer.UseSharedPolicies(app);
            return app;
        }
    }
}