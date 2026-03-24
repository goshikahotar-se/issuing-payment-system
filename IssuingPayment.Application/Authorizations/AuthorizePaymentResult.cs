namespace IssuingPayment.Application.Authorizations;

public class AuthorizePaymentResult
{
    public bool Approved { get; init; }
    public string ReasonCode { get; init; }
    public string? AuthorizationCode { get; init; }
}