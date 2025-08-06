using ECommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Interfaces;
using OrderApi.Infrastructure.Data;
using OrderApi.Infrastructure.Repositories;

namespace OrderApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {

            // Add Database Connectivity
            // Add Authentication scheme

            SharedServiceContainer.AddSharedServices<OrderDbContext>(services, configuration, configuration["MySerilog:FileName"]!);

            // Create dependency Injection
            services.AddScoped<IOrder, OrderRepository>();
            return services;
        }

        public static IApplicationBuilder UserInfrastructurePolicy(this IApplicationBuilder app)
        {
            // Register middleware such as:
            // Global Exception Middleware
            // Listen to api gateway only -> blocks all outsiders calls
            SharedServiceContainer.UseSharedPolicies(app);
            return app;
        }
    }
}
