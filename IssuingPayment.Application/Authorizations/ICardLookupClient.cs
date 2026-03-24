namespace IssuingPayment.Application.Authorizations;

public interface ICardLookupClient
{
    Task<CardSummary?> GetCardById(string cardId, CancellationToken cancellationToken);
}