namespace IssuingPayment.Application.Authorizations.Events;

public interface IAuthorizationEventPublisher
{
    Task PublishAsync(IAuthorizationEvent authorizationEvent, CancellationToken cancellationToken);
}