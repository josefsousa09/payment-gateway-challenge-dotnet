using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PaymentGateway.Api.Models.Payment.Requests;

namespace PaymentGateway.Api.Tests
{
    public class PostPaymentRequestTests
    {
        private readonly Random _random = new();

        [Theory]
        [InlineData("12345678901234", true)]
        [InlineData("1234567890123456", true)]
        [InlineData("1234567890123456789", true)]
        [InlineData("1234567890", false)]
        [InlineData("12345678123456a", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void ValidatesCardNumber(string cardNumber, bool expectedResult)
        {
            //Arrange
            PostPaymentRequest request = new()
            {
                CardNumber = cardNumber,
                ExpiryMonth = _random.Next(1, 12),
                ExpiryYear = _random.Next(2026, 2033),
                Currency = "GBP",
                Amount = _random.Next(1, 10000),
                Cvv = "123"
            };

            //Act
            bool result = PostPaymentRequest.IsValid(request);

            //Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(1, 2050, true)]
        [InlineData(12, 2050, true)]
        [InlineData(0, 2050, false)]
        [InlineData(13, 2050, false)]
        [InlineData(1, 2020, false)]
        public void ValidatesExpiryDate(int month, int year, bool expectedResult)
        {
            //Arrange
            PostPaymentRequest request = new()
            {
                CardNumber = "12345678901234",
                ExpiryMonth = month,
                ExpiryYear = year,
                Currency = "GBP",
                Amount = _random.Next(1, 10000),
                Cvv = "123"
            };
            //Act
            bool result = PostPaymentRequest.IsValid(request);
            //Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("GBP", true)]
        [InlineData("aed", true)]
        [InlineData("ABC", false)]
        [InlineData("AE", false)]
        [InlineData("", false)]
        public void ValidatesCurrency(string currency, bool expectedResult)
        {
            //Arrange
            PostPaymentRequest request = new()
            {
                CardNumber = "12345678901234",
                ExpiryMonth = _random.Next(1, 12),
                ExpiryYear = _random.Next(2026, 2033),
                Currency = currency,
                Amount = _random.Next(1, 10000),
                Cvv = "123"
            };
            //Act
            bool result = PostPaymentRequest.IsValid(request);
            //Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(100, true)]
        [InlineData(-50, false)]
        [InlineData(-1, false)]
        public void ValidatesAmount(int amount, bool expectedResult)
        {
            //Arrange
            PostPaymentRequest request = new()
            {
                CardNumber = "12345678901234",
                ExpiryMonth = _random.Next(1, 12),
                ExpiryYear = _random.Next(2026, 2033),
                Currency = "GBP",
                Amount = amount,
                Cvv = "123"
            };
            //Act
            bool result = PostPaymentRequest.IsValid(request);
            //Assert
            Assert.Equal(expectedResult, result);

        }

        [Theory]
        [InlineData("123", true)]
        [InlineData("1234", true)]
        [InlineData("12", false)]
        [InlineData("12345", false)]
        [InlineData("12a", false)]
        [InlineData("", false)]
        public void ValidatesCvv(string cvv, bool expectedResult)
        {
            //Arrange
            PostPaymentRequest request = new()
            {
                CardNumber = "12345678901234",
                ExpiryMonth = _random.Next(1, 12),
                ExpiryYear = _random.Next(2026, 2033),
                Currency = "GBP",
                Amount = _random.Next(1, 10000),
                Cvv = cvv
            };
            //Act
            bool result = PostPaymentRequest.IsValid(request);
            //Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
