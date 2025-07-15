using PaymentsCLI.Payments.Models;

namespace PaymentsCLI.Payments;

interface IPaymentsService
{
    Task<PaymentResponse> MakePayment(PaymentRequest paymentRequest);
}
