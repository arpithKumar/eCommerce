using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Infrastructure.Data;
using AuthenticationApi.Infrastructure.Repositories;
using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationApi.Infrastructure.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
    {
        SharedServiceContainer.AddSharedServices<AuthenticationDbContext>(services, config, config.GetSection("MySerilog:FileName").Value!);
        services.AddScoped<IUser, UserRepository>();
        return services;
    }

    public static IApplicationBuilder UseInfrastructureServicePolicy(this IApplicationBuilder app)
    {
        SharedServiceContainer.UseSharedPolicies(app);
        return app;
    }
}