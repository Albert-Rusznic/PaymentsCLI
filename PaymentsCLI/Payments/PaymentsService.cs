using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentsCLI.Payments.Models;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Text.Json;

namespace PaymentsCLI.Payments;

public class PaymentsService : IPaymentsService
{
    private readonly HttpClient _httpClient;
    private readonly PaymentsOptions _options;
    private readonly ILogger<PaymentsService> _logger;

    public PaymentsService(HttpClient httpClient, IOptions<PaymentsOptions> options, ILogger<PaymentsService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;

        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
    }

    public async Task<PaymentResponse> MakePayment(PaymentRequest paymentRequest)
    {
        var response = await _httpClient.PostAsJsonAsync(_options.Endpoint, paymentRequest);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        try
        {
            var paymentResponse = JsonSerializer.Deserialize<PaymentResponse>(body);
            Validator.ValidateObject(paymentResponse, new ValidationContext(paymentResponse), true);
            return paymentResponse;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse payment response: {Body}", body);
            throw;
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Invalid or missing fields in deserialized payment response.: {Body}", body);
            throw;
        }
    }
}
