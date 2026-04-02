using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using IssuingPayment.Application.Authorizations.Events;

namespace IssuingPayment.Infrastructure;

public class SnsAuthorizationEventPublisher : IAuthorizationEventPublisher
{
    private readonly IAmazonSimpleNotificationService _snsAmazonSimpleNotificationService;
    private readonly string _topicArn;

    public SnsAuthorizationEventPublisher(IAmazonSimpleNotificationService snsAmazonSimpleNotificationService,  string topicArn)
    {
        _snsAmazonSimpleNotificationService = snsAmazonSimpleNotificationService;
        _topicArn = topicArn;
    }
    
    public async Task PublishAsync(IAuthorizationEvent authorizationEvent, CancellationToken cancellationToken)
    {
        string message;
        switch (authorizationEvent)
        {
            case AuthorizationApprovedEvent approvedEvent:
                message = JsonSerializer.Serialize(approvedEvent);
                break;
            
            case AuthorizationDeclinedEvent declinedEvent:
                message = JsonSerializer.Serialize(declinedEvent);
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(authorizationEvent), authorizationEvent, null);
        }
        
        var publishRequest = new PublishRequest(_topicArn, message);
        
        await _snsAmazonSimpleNotificationService.PublishAsync(publishRequest, cancellationToken);
    }
}