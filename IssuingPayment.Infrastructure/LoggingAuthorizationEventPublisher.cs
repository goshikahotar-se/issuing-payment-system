using IssuingPayment.Application.Authorizations.Events;
using Serilog;

namespace IssuingPayment.Infrastructure;

public class LoggingAuthorizationEventPublisher : IAuthorizationEventPublisher
{
    public Task PublishAsync(IAuthorizationEvent authorizationEvent, CancellationToken cancellationToken)
    {
        switch (authorizationEvent)
        {
            case AuthorizationApprovedEvent approvedEvent:
                Log.Information("Authorization Approved, CardId: {CardId} Amount: {Amount} Currency: {Currency} AuthorizationCode: {AuthorizationCode} CreatedOn: {CreatedOn}",
                                approvedEvent.CardId, approvedEvent.Amount, approvedEvent.Currency, approvedEvent.AuthorizationCode,  approvedEvent.CreatedOn);
                break;
            
            case AuthorizationDeclinedEvent declinedEvent:
                Log.Information("Authorization Declined, CardId: {CardId} Amount: {Amount} Currency: {Currency} ReasonCode: {ReasonCode} CreatedOn: {CreatedOn}",
                    declinedEvent.CardId, declinedEvent.Amount,  declinedEvent.Currency, declinedEvent.ReasonCode, declinedEvent.CreatedOn);
                break;
            
            default:
                Log.Warning("Unknown authorization event {Event}", authorizationEvent.GetType().Name);
                break;
        }
        
        return Task.CompletedTask;
    }
}