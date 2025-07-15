using Polly.Extensions.Http;
using Polly;

namespace PaymentsCLI;

internal static class HttpPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryAttempts) =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(retryAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(int handledEventsAllowedBeforeBreaking, int durationOfBreakSeconds) =>
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: handledEventsAllowedBeforeBreaking,
            durationOfBreak: TimeSpan.FromSeconds(durationOfBreakSeconds)
        );
}
