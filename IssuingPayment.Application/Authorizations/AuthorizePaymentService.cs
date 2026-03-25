using IssuingPayment.Application.Helper;

namespace IssuingPayment.Application.Authorizations;

public class AuthorizePaymentService
{
    private readonly ICardLookupClient _cardLookupClient;

    public AuthorizePaymentService(ICardLookupClient cardLookupClient)
    {
        _cardLookupClient = cardLookupClient;
    }

    public async Task<AuthorizePaymentResult> Handle(AuthorizePaymentCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.CardId))
            return new AuthorizePaymentResult()
            {
                Approved = false,
                AuthorizationCode = null,
                ReasonCode = "CardNotFound"
            };

        var card = await _cardLookupClient.GetCardById(command.CardId, cancellationToken);
        if (card is null)
            return new AuthorizePaymentResult()
            {
                Approved = false,
                AuthorizationCode = null,
                ReasonCode = "CardNotFound"
            };

        if (card.Status != 0)
            return new AuthorizePaymentResult()
            {
                Approved = false,
                AuthorizationCode = null,
                ReasonCode = "CardInactive"
            };
        
        if (card.ExpiryYear != command.ExpiryYear || card.ExpiryMonth != command.ExpiryMonth)
            return new AuthorizePaymentResult()
            {
                Approved = false,
                AuthorizationCode = null,
                ReasonCode = "ExpiryMismatch"
            };

        if (card.Cvc != command.Cvc)
            return new AuthorizePaymentResult()
            {
                Approved = false,
                AuthorizationCode = null,
                ReasonCode = "InvalidCvc"
            };

        if (card.Currency != command.Currency)
            return new AuthorizePaymentResult()
            {
                Approved = false,
                AuthorizationCode = null,
                ReasonCode = "CurrencyMismatch"
            };

        if (card.AvailableLimit < command.Amount)
            return new AuthorizePaymentResult()
            {
                Approved = false,
                AuthorizationCode = null,
                ReasonCode = "InsufficientFunds"
            };
        
        return new AuthorizePaymentResult()
        {
            Approved = true,
            AuthorizationCode = GenerateAuthorizationFields.GenerateAuthorizationCode(),
            ReasonCode = "Approved"
        };
    }
}