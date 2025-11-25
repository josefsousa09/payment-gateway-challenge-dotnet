using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Bank.Requests;
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
        
        
            var bankRequest = new BankAuthorisationRequest
            {
                CardNumber = request.CardNumber,
                ExpiryDate = $"{request.ExpiryMonth:D2}/{request.ExpiryYear}",
                Currency = request.Currency,
                Amount = request.Amount,
                Cvv = request.Cvv
            };


            var response = await bankClient.PostAsJsonAsync("/payments", bankRequest);

            if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                return new BankAuthorisationResponse { Authorized = false };
            }

            response.EnsureSuccessStatusCode();

            var bankResponse = await response.Content.ReadFromJsonAsync<BankAuthorisationResponse>();

            return bankResponse ?? new BankAuthorisationResponse { Authorized = false };


        }
        
    }

