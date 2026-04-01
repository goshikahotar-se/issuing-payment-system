using IssuingPayment.Application.Authorizations.Events;

namespace IssuingPayment.Tests.Application;

public class FakeAuthorizationEventPublisher : IAuthorizationEventPublisher
{
    public List<IAuthorizationEvent> authorizationEvents = new();
    

    public Task PublishAsync(IAuthorizationEvent authorizationEvent, CancellationToken cancellationToken)
    {
        authorizationEvents.Add(authorizationEvent);
        return Task.CompletedTask;
    }
}