using PaymentGateway.Api.Models.Payment.Responses;

namespace PaymentGateway.Api.Services;

public class PaymentsRepository
{
    public List<GetPaymentResponse> Payments = [];
    
    public void Add(PostPaymentResponse payment)
    {
        var paymentRecord = new GetPaymentResponse
        {
            Id = payment.Id,
            Status = payment.Status,
            CardNumberLastFour = payment.CardNumberLastFour,
            ExpiryMonth = payment.ExpiryMonth,
            ExpiryYear = payment.ExpiryYear,
            Currency = payment.Currency,
            Amount = payment.Amount
        };

        Payments.Add(paymentRecord);
    }

    public GetPaymentResponse Get(Guid id)
    {
        return Payments.FirstOrDefault(p => p.Id == id);
    }
}