using IssuingPayment.Application.Authorizations.Events;
using IssuingPayment.Application.Helper;

namespace IssuingPayment.Application.Authorizations;

public class AuthorizePaymentService
{
    private readonly ICardLookupClient _cardLookupClient;
    private readonly IAuthorizationEventPublisher _authorizationEventPublisher;

    public AuthorizePaymentService(ICardLookupClient cardLookupClient, IAuthorizationEventPublisher authorizationEventPublisher)
    {
        _cardLookupClient = cardLookupClient;
        _authorizationEventPublisher = authorizationEventPublisher;
    }

    public async Task<AuthorizePaymentResult> Handle(AuthorizePaymentCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.CardId))
        {
            return new AuthorizePaymentResult()
            {
                Approved = false,
                AuthorizationCode = null,
                ReasonCode = "CardNotFound"
            };
        }
        
        var card = await _cardLookupClient.GetCardById(command.CardId, cancellationToken);
        
        if (card is null)
        {
            var declinedEvent = new AuthorizationDeclinedEvent(command.CardId,
                command.Amount,
                command.Currency,
                "CardNotFound",
                DateTime.UtcNow);
            
            await _authorizationEventPublisher
                .PublishAsync(declinedEvent, cancellationToken);
            
            return new AuthorizePaymentResult()
            {
                Approved = false,
                AuthorizationCode = null,
                ReasonCode = "CardNotFound"
            };
        }

        if (card.Status != 0)
        {
            var declinedEvent = new AuthorizationDeclinedEvent(command.CardId,
                command.Amount,
                command.Currency,
                "CardInactive",
                DateTime.UtcNow);
            
            await _authorizationEventPublisher
                .PublishAsync(declinedEvent, cancellationToken);
            
            return new AuthorizePaymentResult()
            {
                Approved = false,
                AuthorizationCode = null,
                ReasonCode = "CardInactive"
            };
        }

        if (card.ExpiryYear != command.ExpiryYear || card.ExpiryMonth != command.ExpiryMonth)
        {
            var declinedEvent = new AuthorizationDeclinedEvent(command.CardId,
                command.Amount,
                command.Currency,
                "ExpiryMismatch",
                DateTime.UtcNow);
            
            await _authorizationEventPublisher
                .PublishAsync(declinedEvent, cancellationToken);
            
            return new AuthorizePaymentResult()
            {
                Approved = false,
                AuthorizationCode = null,
                ReasonCode = "ExpiryMismatch"
            };
        }

        if (card.Cvc != command.Cvc)
        {
            var declinedEvent = new AuthorizationDeclinedEvent(command.CardId,
                command.Amount,
                command.Currency,
                "InvalidCvc",
                DateTime.UtcNow);
            
            await _authorizationEventPublisher
                .PublishAsync(declinedEvent, cancellationToken);
            
            return new AuthorizePaymentResult()
            {
                Approved = false,
                AuthorizationCode = null,
                ReasonCode = "InvalidCvc"
            };
        }

        if (card.Currency != command.Currency)
        {
            var declinedEvent = new AuthorizationDeclinedEvent(command.CardId,
                command.Amount,
                command.Currency,
                "CurrencyMismatch",
                DateTime.UtcNow);
            
            await _authorizationEventPublisher
                .PublishAsync(declinedEvent, cancellationToken);
            
            return new AuthorizePaymentResult()
            {
                Approved = false,
                AuthorizationCode = null,
                ReasonCode = "CurrencyMismatch"
            };
        }

        if (card.AvailableLimit < command.Amount)
        {
            var declinedEvent = new AuthorizationDeclinedEvent(command.CardId,
                command.Amount,
                command.Currency,
                "InsufficientFunds",
                DateTime.UtcNow);
            
            await _authorizationEventPublisher
                .PublishAsync(declinedEvent, cancellationToken);
         
            return new AuthorizePaymentResult()
            {
                Approved = false,
                AuthorizationCode = null,
                ReasonCode = "InsufficientFunds"
            };
        }

        var authorizationCode = GenerateAuthorizationFields.GenerateAuthorizationCode();
        
        var approvedEvent = new AuthorizationApprovedEvent(command.CardId,
                                                           command.Amount,
                                                           command.Currency,
                                                           authorizationCode,
                                                           DateTime.UtcNow);
        await _authorizationEventPublisher
            .PublishAsync(approvedEvent, cancellationToken);
        
        return new AuthorizePaymentResult()
        {
            Approved = true,
            AuthorizationCode = authorizationCode,
            ReasonCode = "Approved"
        };
    }
}