using IssuingPayment.Application.Authorizations;

namespace IssuingPayment.Infrastructure.Services;

public class HttpCardLookupClient : ICardLookupClient
{
    private readonly HttpClient _httpClient;
    
    public HttpCardLookupClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<CardSummary?> GetCardById(string cardId, CancellationToken cancellationToken)
    {
        return _httpClient.GetFromJsonAsync<CardSummary>($"/cards/{cardId}", cancellationToken);
    }
}