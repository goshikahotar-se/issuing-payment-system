namespace IssuingPayment.Application.Authorizations;

public class AuthorizePaymentCommand
{
    public string CardId { get; init; }
    public string Cvc { get; init; }
    public int ExpiryMonth { get; init; }
    public int ExpiryYear { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; }
}