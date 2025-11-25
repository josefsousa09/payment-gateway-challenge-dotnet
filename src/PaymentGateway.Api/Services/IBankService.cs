using PaymentGateway.Api.Models.Bank.Responses;
using PaymentGateway.Api.Models.Payment.Requests;

namespace PaymentGateway.Api.Services;

public interface IBankService
{
    Task<BankAuthorisationResponse> ProcessPaymentAsync(PostPaymentRequest request);
}
