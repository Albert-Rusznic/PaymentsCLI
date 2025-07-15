using Microsoft.Extensions.Logging;
using PaymentsCLI.Payments;
using PaymentsCLI.Payments.Models;

namespace PaymentsCLI.ConsoleUI;

class ConsoleUI : IConsoleUI
{
    private readonly IPaymentsService _paymentsService;
    private readonly ILogger<ConsoleUI> _logger;

    public ConsoleUI(IPaymentsService paymentsService, ILogger<ConsoleUI> logger)
    {
        _paymentsService = paymentsService;
        _logger = logger;
    }

    public async Task Start()
    {
        while (true)
        {
            Console.WriteLine("==== Menu ====");
            Console.WriteLine("1 - Make Payment");
            Console.WriteLine("0 - Quit");
            Console.Write("Choose option: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    await MakePayment();
                    break;
                case "0":
                    Console.WriteLine("Shutting down...");
                    return;
                default:
                    Console.WriteLine("Please choose a valid option");
                    break;
            }
        }
    }

    private async Task MakePayment()
    {
        Console.Write("Amount: ");
        var inputAmount = Console.ReadLine();
        if (!decimal.TryParse(inputAmount, out var amount))
        {
            Console.WriteLine("Invalid amount. Please enter a valid decimal number.");
            return;
        }

        Console.Write("PaymentId: ");
        var inputPaymentId = Console.ReadLine();
        if (!int.TryParse(inputPaymentId, out var paymentId))
        {
            Console.WriteLine("Invalid paymentId. Please enter a valid integer number.");
            return;
        }

        var request = new PaymentRequest();
        request.Amount = amount;
        request.PaymentId = paymentId;

        try
        {
            _logger.LogInformation("Making payment. Request: {@PaymentRequest}", request);
            var response = await _paymentsService.MakePayment(request);
            Console.WriteLine("Payment successful!");
            _logger.LogInformation("Payment successful. Response: {@PaymentResponse}", response);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Payment failed!");
            _logger.LogError(ex, "Unexpected error occurred.");
        }
    }
}
