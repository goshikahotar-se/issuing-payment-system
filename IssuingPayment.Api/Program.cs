using IssuingPayment.Application.Authorizations;
using IssuingPayment.Application.Authorizations.Events;
using IssuingPayment.Infrastructure;
using IssuingPayment.Infrastructure.Services;
using Serilog;

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
builder.Services.AddSingleton<IAuthorizationEventPublisher, LoggingAuthorizationEventPublisher>();

//Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Seq(builder.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341")
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.MapPost("/authorizations",
    async (CreateAuthorizationRequest authorizationRequest,
        AuthorizePaymentService service,
        CancellationToken cancellationToken) =>
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(authorizationRequest.CardId))
            errors.Add("CardId is required");
        
        if (string.IsNullOrWhiteSpace(authorizationRequest.Cvc))
            errors.Add("Cvc is required");
        
        if (authorizationRequest.ExpiryMonth < 1 || authorizationRequest.ExpiryMonth > 12)
            errors.Add("ExpiryMonth must be between 1 and 12");
        
        if (authorizationRequest.ExpiryYear < DateTime.UtcNow.Year)
            errors.Add("ExpiryYear cannot be in the past");
        
        if (authorizationRequest.Amount < 0)
            errors.Add("Amount must be greater than or equal to zero");
        
        if (string.IsNullOrWhiteSpace(authorizationRequest.Currency))
            errors.Add("Currency is required");

        if (errors.Count > 0)
        {
            return Results.BadRequest(new { error = "ValidationFailed", details = errors });
        }
        
        Log.Information("Processing authorization request with CardID {CardId} Amount {Amount} Currency {Currency}",
            authorizationRequest.CardId,
            authorizationRequest.Amount,
            authorizationRequest.Currency);
            
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
        
        Log.Information("AuthorizationResult for {CardId}: Approved = {IsApproved} Reason = {ReasonCode} AuthCode = {AuthorizationCode}",
            authorizationRequest.CardId,
            result.Approved,
            result.ReasonCode,
            result.AuthorizationCode);
        
        return Results.Ok(result); 
    });

try
{
    Log.Information("Starting IssuingPayment API");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}

record CreateAuthorizationRequest(string CardId, string Cvc, int ExpiryMonth, int ExpiryYear, decimal Amount, string Currency);