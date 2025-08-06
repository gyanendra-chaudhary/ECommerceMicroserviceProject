using ECommerce.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ECommerce.SharedLibrary.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<TContext>(this IServiceCollection services, IConfiguration configuration, string fileName) where TContext : DbContext
        {
            // Add Generic Database context
            services.AddDbContext<TContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ECommerceConnection")
            , sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));

            // configure serilog logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{fileName}-.text", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day).CreateLogger();

            // Add JWT authentication Scheme
            JWTAuthenticationScheme.AddJWTAuthenticationScheme(services, configuration);

            return services;
        }

        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            // Use global  Exception Middleware 
            app.UseMiddleware<GlobalException>();

            // Register middleware to block outsiders API calls
           // app.UseMiddleware<ListenToOnlyApiGateway>();

            return app;

        }
    }
}
