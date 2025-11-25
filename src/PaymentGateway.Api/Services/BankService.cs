using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Bank.Responses;
using PaymentGateway.Api.Models.Payment.Requests;
using PaymentGateway.Api.Models.Payment.Responses;

namespace PaymentGateway.Api.Services;

public class BankService : IBankService
{
    private readonly static HttpClient bankClient = new()
    {
        BaseAddress = new Uri("http://localhost:8080"),
    };



    public async Task<BankAuthorisationResponse> ProcessPaymentAsync(PostPaymentRequest request)
    {
        try
        {
            var bankRequest = new
            {
                card_number = request.CardNumber,
                expiry_date = $"{request.ExpiryMonth:D2}/{request.ExpiryYear}",
                currency = request.Currency,
                amount = request.Amount,
                cvv = request.Cvv
            };


            var response = await bankClient.PostAsJsonAsync("/payments", bankRequest);

            if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                return new BankAuthorisationResponse { Authorized = false };
            }

            response.EnsureSuccessStatusCode();

            var bankResponse = await response.Content.ReadFromJsonAsync<BankAuthorisationResponse>();

            var paymentResponse = new PostPaymentResponse
            {
                Id = Guid.NewGuid(),
                Status = bankResponse.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined,
                CardNumberLastFour = int.Parse(request.CardNumber[^4..]),
                ExpiryMonth = request.ExpiryMonth,
                ExpiryYear = request.ExpiryYear,
                Currency = request.Currency,
                Amount = request.Amount
            };

            return bankResponse ?? new BankAuthorisationResponse { Authorized = false };


        }
        catch (Exception)
        {
            throw;
        }
    }
}
