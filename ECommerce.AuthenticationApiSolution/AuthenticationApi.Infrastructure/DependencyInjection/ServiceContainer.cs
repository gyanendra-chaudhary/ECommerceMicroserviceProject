using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Infrastructure.Data;
using AuthenticationApi.Infrastructure.Repositories;
using ECommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            // Add database connectivity
            // JWT Add authentication scheme
            SharedServiceContainer.AddSharedServices<AuthenticationDbContext>(services, configuration, configuration["MySerilog:FileName"]!);

            // Create dependency injection
            services.AddScoped<IUser, UserRepository>();

            return services;
        }

        public static IApplicationBuilder UserInfrastructurePolicy(this IApplicationBuilder app)
        {
            // Register middleware such as:
            // - Global Exception Middleware
            // - ListenToOnlyApiGateway Middleware -> blocks all outsiders API calls
            SharedServiceContainer.UseSharedPolicies(app);
            return app;
        }

    }
}
