using System.ComponentModel.DataAnnotations;

namespace PaymentsCLI.Payments.Models;

public class PaymentResponse
{
    [Range(1, int.MaxValue)]
    public int TransactionId { get; set; }

    [Required]
    public string Status { get; set; }
}
