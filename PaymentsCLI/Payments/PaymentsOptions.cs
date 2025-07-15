namespace PaymentsCLI.Payments;

public class PaymentsOptions
{
    public string BaseUrl { get; set; }
    public string Endpoint { get; set; }
    public int HandlerLifetimeMinutes { get; set; }
    public int RetryAttempts { get; set; }
    public int HandledEventsAllowedBeforeBreaking { get; set; }
    public int DurationOfBreakSeconds {  get; set; }
}
