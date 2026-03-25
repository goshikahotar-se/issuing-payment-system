using IssuingPayment.Application.Authorizations;
using Xunit;
using Assert = Xunit.Assert;

namespace IssuingPayment.Tests.Application;

public class AuthorizationTests
{
    [Xunit.Theory]
    [InlineData("crd_t5rxaz3q1az54koo", 10, 2027, "112", "EUR", 10L, false, "CardNotFound")]
    [InlineData("crd_dwsxaz3q1az54cil", 11, 2027, "112", "EUR", 10L, true, "Approved")]
    [InlineData("crd_w3zmaz3q1az5oyq7", 10, 2027, "571", "GBP", 10L, false, "CardInactive")]
    [InlineData("crd_dwsxaz3q1az54cil", 11, 2028, "112", "EUR", 10L, false, "ExpiryMismatch")]
    [InlineData("crd_dwsxaz3q1az54cil", 10, 2027, "112", "EUR", 10L, false, "ExpiryMismatch")]
    [InlineData("crd_dwsxaz3q1az54cil", 11, 2027, "745", "EUR", 10L, false, "InvalidCvc")]
    [InlineData("crd_dwsxaz3q1az54cil", 11, 2027, "112", "MUR", 10L, false, "CurrencyMismatch")]
    [InlineData("crd_dwsxaz3q1az54cil", 11, 2027, "112", "EUR", 15L, false, "InsufficientFunds")]
    public async Task Handler_Should_Return_Correct_Result_Based_On_Card_Is_Existing_Or_Not(
        string cardId, 
        int expiryMonth, 
        int expiryYear,
        string cvc,
        string currency,
        decimal amount,
        bool approved, 
        string reasonCode)
    {
        //Arrange
        var cardLookupClient = new FakeCardLookupClient();
        var service = new AuthorizePaymentService(cardLookupClient);
        var command = new AuthorizePaymentCommand
        {
            CardId = cardId,
            Cvc = cvc,
            ExpiryMonth = expiryMonth,
            ExpiryYear = expiryYear,
            Amount = amount,
            Currency = currency
        };
        
        //Act
        var result = await service.Handle(command, CancellationToken.None);

        //Assert
        Assert.Equal(approved, result.Approved);
        Assert.Equal(reasonCode, result.ReasonCode);
        
        var hasAuthCode = !string.IsNullOrWhiteSpace(result.AuthorizationCode);
        Assert.Equal(approved, hasAuthCode);
    }
}