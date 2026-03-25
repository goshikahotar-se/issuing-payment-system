using IssuingPayment.Application.Authorizations;
using IssuingPayment.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var issuingCardBaseUrl = builder.Configuration["IssuingCard:BaseUrl"];

builder.Services.AddScoped<AuthorizePaymentService>();
builder.Services.AddHttpClient<ICardLookupClient, HttpCardLookupClient>(client =>
{
    if (!string.IsNullOrWhiteSpace(issuingCardBaseUrl)) client.BaseAddress = new Uri(issuingCardBaseUrl);
    else throw new InvalidOperationException("IssuingCard:BaseUrl is not set");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/authorizations",
    async (CreateAuthorizationRequest authorizationRequest,
        AuthorizePaymentService service,
        CancellationToken cancellationToken) =>
    {
        var command = new AuthorizePaymentCommand
        {
            CardId = authorizationRequest.CardId,
            Cvc = authorizationRequest.Cvc,
            ExpiryMonth = authorizationRequest.ExpiryMonth,
            ExpiryYear = authorizationRequest.ExpiryYear,
            Amount = authorizationRequest.Amount,
            Currency = authorizationRequest.Currency
        };
        
        var result = await service.Handle(command, cancellationToken);
        
        return Results.Ok(result); 
    });

app.Run();

record CreateAuthorizationRequest(string CardId, string Cvc, int ExpiryMonth, int ExpiryYear, decimal Amount, string Currency);