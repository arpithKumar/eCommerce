using System.Collections.Immutable;
using eCommerce.SharedLibrary.Logs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Services;
using Polly;
using Polly.Retry;

namespace OrderApi.Application.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddOrderApiApplicationService(this IServiceCollection services,
        IConfiguration config)
    {
        services.AddHttpClient<IOrderService, OrderService>(options =>
        {
            options.BaseAddress = new Uri(config["ApiGateway:BaseAddress"]!);
            options.Timeout = TimeSpan.FromSeconds(1);
        });

        var retryStrategy = new RetryStrategyOptions()
        {
            ShouldHandle = new PredicateBuilder().Handle<TaskCanceledException>(),
            BackoffType = DelayBackoffType.Constant,
            UseJitter = true,
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromMilliseconds(500),
            OnRetry = args =>
            {
                string message = $"OnRetry, Attempt: {args.AttemptNumber}, Outcomes :{args.Outcome}";
                LogExceptions.LogToConsole(message);
                LogExceptions.LogToDebugger(message);
                return ValueTask.CompletedTask;
            }
        };
        services.AddResiliencePipeline("order-retry-pipeline", builder => builder.AddRetry(retryStrategy));
        return services;
    }
}