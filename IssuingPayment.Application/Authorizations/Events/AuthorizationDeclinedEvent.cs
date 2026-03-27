namespace IssuingPayment.Application.Authorizations.Events;

public record AuthorizationDeclinedEvent(
    string CardId,
    decimal Amount,
    string Currency,
    string ReasonCode,
    DateTime CreatedOn) : IAuthorizationEvent;