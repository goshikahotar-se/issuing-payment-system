using IssuingPayment.Application.Authorizations;
using Xunit;
using Assert = Xunit.Assert;

namespace IssuingPayment.Tests.Application;

public class AuthorizationTests
{
    [Xunit.Theory]
    [InlineData("crd_t5rxaz3q1az54koo", false, "CardNotFound")]
    [InlineData("crd_dwsxaz3q1az54cil", true, "Approved")]
    public async Task Handler_Should_Return_Correct_Result_Based_On_Card_Is_Existing_Or_Not(string cardId, bool approved, string reasonCode)
    {
        //Arrange
        var cardLookupClient = new FakeCardLookupClient();
        var service = new AuthorizePaymentService(cardLookupClient);
        var command = new AuthorizePaymentCommand
        {
            CardId = cardId,
            Cvc = "576",
            ExpiryMonth = 10,
            ExpiryYear = 2027,
            Amount = 100m,
            Currency = "GBP"
        };
        
        //Act
        var result = await service.Handle(command, CancellationToken.None);

        //Assert
        Assert.Equal(approved, result.Approved);
        Assert.Equal(reasonCode, result.ReasonCode);
    }
}