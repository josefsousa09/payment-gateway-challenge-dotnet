using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Payment.Requests;
using PaymentGateway.Api.Models.Payment.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(PaymentsRepository paymentsRepository, IBankService bankService) : Controller
{
    private readonly PaymentsRepository _paymentsRepository = paymentsRepository;
    private readonly IBankService _bankService = bankService;

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        var payment = _paymentsRepository.Get(id);

        if (payment == null)
        {
            return NotFound();
        }

        return Ok(payment);
    }

    [HttpPost]
    public async Task<ActionResult<PostPaymentResponse>> ProcessPaymentAsync([FromBody] PostPaymentRequest request)
    {
        if (!PostPaymentRequest.IsValid(request))
        {
            return BadRequest(PaymentStatus.Rejected);
        }

        var bankResponse = await _bankService.ProcessPaymentAsync(request);

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

        _paymentsRepository.Add(paymentResponse);

        return Ok(paymentResponse);
    }
}