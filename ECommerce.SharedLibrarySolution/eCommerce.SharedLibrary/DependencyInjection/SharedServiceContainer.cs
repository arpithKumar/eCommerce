using eCommerce.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace eCommerce.SharedLibrary.DependencyInjection;

public static class SharedServiceContainer
{
    public static IServiceCollection AddSharedServices<TContext>(this IServiceCollection services,
        IConfiguration config, string fileName) where TContext : DbContext
    {
        
        // Add Generic Database COntext
        services.AddDbContext<TContext>(options =>
            options.UseMySQL(
                config.GetConnectionString("eCommerceConnectionString")
                ?? throw new InvalidOperationException("Connection string 'eCommerceConnectionString' is not found."),
                mySqlOptions => mySqlOptions.EnableRetryOnFailure()
            )
        );
        
        //Add Logger Configuration
       ConfigureLogger(fileName);
        
        // Add JWT Authentication Scheme
        JwtAuthenticationScheme.AddJwtAuthenticationScheme(services, config);
        return services;
    }

    public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
    {
        // Use Global Exception
        app.UseMiddleware<GlobalException>();
        
        // Use Middleware to block all the non api-gateway calls
        app.UseMiddleware<ListenToOnlyApiGateway>();
        return app;
    }
    
    private static void ConfigureLogger(string fileName)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Debug()
            .WriteTo.Console()
            .WriteTo.File(path: $"{fileName}-{DateTime.Now:yyyyMMddHHmmss}.log",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level: u3}] {message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }
}