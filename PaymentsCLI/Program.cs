using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PaymentsCLI;
using PaymentsCLI.ConsoleUI;
using PaymentsCLI.Payments;

class Program
{
    static async Task Main(string[] args)
    {
        using var host = CreateHostBuilder(args).Build();
        var app = host.Services.GetRequiredService<App>();
        await app.RunAsync();
    }

    static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host
            .CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddTransient<App>();
                services.AddTransient<IConsoleUI, ConsoleUI>();
                services.AddTransient<IPaymentsService,  PaymentsService>();

                var configuration = context.Configuration;
                services.Configure<PaymentsOptions>(configuration.GetSection("Payments"));
                var paymentsOptions = configuration.GetSection("Payments").Get<PaymentsOptions>();
                if (paymentsOptions == null)
                {
                    throw new InvalidOperationException("Missing or invalid 'Payments' configuration section. Check appsettings.json");
                }

                services.AddHttpClient<IPaymentsService, PaymentsService>()
                    .SetHandlerLifetime(TimeSpan.FromMinutes(paymentsOptions.HandlerLifetimeMinutes))
                    .AddPolicyHandler(HttpPolicies.GetRetryPolicy(paymentsOptions.RetryAttempts))
                    .AddPolicyHandler(HttpPolicies.GetCircuitBreakerPolicy(paymentsOptions.HandledEventsAllowedBeforeBreaking, paymentsOptions.DurationOfBreakSeconds));
            });
    }
}