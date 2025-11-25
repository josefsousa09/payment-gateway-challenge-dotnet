using PaymentGateway.Api.Models.Bank.Responses;
using PaymentGateway.Api.Models.Payment.Requests;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests
{
    public class BankServiceTests
    {
        private readonly BankService _bankService;
        private readonly Random _random = new();

        public BankServiceTests()
        {
            _bankService = new BankService();
        }

        [Theory]
        [InlineData("1234567890123451")]
        [InlineData("1234567890123453")]
        [InlineData("1234567890123455")]
        [InlineData("1234567890123457")]
        [InlineData("1234567890123459")]
        public async Task ReturnsAuthorizedIfCardEndsOdd(string cardNumber)
        {
            // Arrange
            PostPaymentRequest request = new()
            {
                CardNumber = cardNumber,
                ExpiryMonth = _random.Next(1, 12),
                ExpiryYear = _random.Next(2026, 2033),
                Currency = "GBP",
                Amount = _random.Next(1, 10000),
                Cvv = "123"
            };

            // Act
            BankAuthorisationResponse result = await _bankService.ProcessPaymentAsync(request);

            // Assert
            Assert.True(result.Authorized);
            Assert.NotNull(result.AuthorizationCode);
            Assert.NotEmpty(result.AuthorizationCode);
        }



        [Theory]
        [InlineData("1234567890123452")]
        [InlineData("1234567890123454")]
        [InlineData("1234567890123456")]
        [InlineData("1234567890123458")]
        public async Task ReturnsDeclinedIfCardEndsEven(string cardNumber)
        {
            // Arrange
            PostPaymentRequest request = new()
            {
                CardNumber = cardNumber,
                ExpiryMonth = _random.Next(1, 12),
                ExpiryYear = _random.Next(2026, 2033),
                Currency = "GBP",
                Amount = _random.Next(1, 10000),
                Cvv = "123"
            };

            // Act
            BankAuthorisationResponse result = await _bankService.ProcessPaymentAsync(request);

            // Assert
            Assert.False(result.Authorized);
            Assert.Empty(result.AuthorizationCode);
        }


        [Fact]
        public async Task ReturnsUniqueAuthorisationCodesIfSuccessful()
        {
            // Arrange
            PostPaymentRequest request1 = new()
            {
                CardNumber = "1234567890123451",
                ExpiryMonth = _random.Next(1, 12),
                ExpiryYear = _random.Next(2026, 2033),
                Currency = "GBP",
                Amount = _random.Next(1, 10000),
                Cvv = "123"
            };

            PostPaymentRequest request2 = new()
            {
                CardNumber = "1234567890123457",
                ExpiryMonth = _random.Next(1, 12),
                ExpiryYear = _random.Next(2026, 2033),
                Currency = "AED",
                Amount = _random.Next(1, 10000),
                Cvv = "1234"
            };

            // Act
            BankAuthorisationResponse result1 = await _bankService.ProcessPaymentAsync(request1);
            BankAuthorisationResponse result2 = await _bankService.ProcessPaymentAsync(request2);

            // Assert
            Assert.True(result1.Authorized);
            Assert.True(result2.Authorized);
            Assert.NotEmpty(result1.AuthorizationCode);
            Assert.NotEmpty(result2.AuthorizationCode);
            Assert.NotEqual(result1.AuthorizationCode, result2.AuthorizationCode);
        }

        [Fact]
        public async Task HandlesServiceUnavailableAndReturnsDeclined()
        {
            // Arrange
            PostPaymentRequest request = new()
            {
                CardNumber = "1234567890123450",
                ExpiryMonth = _random.Next(1, 12),
                ExpiryYear = _random.Next(2026, 2033),
                Currency = "AED",
                Amount = _random.Next(1, 10000),
                Cvv = "1234"
            };

            // Act
            BankAuthorisationResponse result = await _bankService.ProcessPaymentAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Authorized);

        }

        [Fact]
        public async Task HandlesErrorAndThrows()
        {
            PostPaymentRequest request = new()
            {
                CardNumber = "abc",
                ExpiryMonth = _random.Next(1, 12),
                ExpiryYear = _random.Next(2026, 2033),
                Currency = "USD",
                Amount = _random.Next(1, 10000),
                Cvv = "999"
            };

            await Assert.ThrowsAsync<HttpRequestException>(async () => await _bankService.ProcessPaymentAsync(request));
        }

    }
}
