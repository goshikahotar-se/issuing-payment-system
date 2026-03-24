using IssuingPayment.Application.Authorizations;

namespace IssuingPayment.Tests.Application;

public class FakeCardLookupClient : ICardLookupClient
{
    public List<CardSummary> cards = new()
    {
        new()
        {
            CardId = "crd_dwsxaz3q1az54cil",
            AvailableLimit = 10L,
            Currency = "EUR",
            Cvc = "112",
            ExpiryMonth = 11,
            ExpiryYear = 2027,
            Status =  "Active"
        }
    };
    
    public Task<CardSummary?> GetCardById(string cardId, CancellationToken cancellationToken)
    {
        return Task.FromResult(cards
            .SingleOrDefault(card => card.CardId == cardId));
    }
}