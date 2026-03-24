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
        return new AuthorizePaymentResult()
        {
            Approved = true,
            AuthorizationCode = GenerateAuthorizationFields.GenerateAuthorizationCode(),
            ReasonCode = "Approved"
        };
    }
}