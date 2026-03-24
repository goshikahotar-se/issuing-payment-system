namespace IssuingPayment.Application.Authorizations;

public class CardSummary
{
    public string CardId { get; init; }
    public string Cvc { get; init; }
    public int ExpiryMonth { get; init; }
    public int ExpiryYear { get; init; }
    public string Status { get; init; }
    public decimal AvailableLimit { get; init; }
    public string Currency { get; init; }
}