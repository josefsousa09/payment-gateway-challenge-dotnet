using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Payment.Requests;
using PaymentGateway.Api.Models.Payment.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests;

public class PaymentsControllerTests
{
    private readonly Random _random = new();
    
    [Fact]
    public async Task RetrievesAPaymentSuccessfully()
    {
        // Arrange
        var payment = new PostPaymentResponse
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Authorized,
            ExpiryYear = _random.Next(2026, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = _random.Next(1111, 9999),
            Currency = "GBP"
        };


        var paymentsRepository = new PaymentsRepository();
        paymentsRepository.Add(payment);

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton(paymentsRepository)))
            .CreateClient();

        // Act
        var response = await client.GetAsync($"/api/Payments/{payment.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<GetPaymentResponse>();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
        Assert.Equal(payment.Id, paymentResponse.Id);
    }

    [Fact]
    public async Task Returns404IfPaymentNotFound()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();
        
        // Act
        var response = await client.GetAsync($"/api/Payments/{Guid.NewGuid()}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ProcessesPaymentSuccessfully()
    {
        // Arrange
        var postPaymentRequest = new PostPaymentRequest
        {
            CardNumber = "12345678912345",
            ExpiryMonth = 12,
            ExpiryYear = 2025,
            Cvv = "123",
            Amount = 1000,
            Currency = "GBP"
        };
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();
        
        // Act
        var response = await client.PostAsJsonAsync("/api/Payments", postPaymentRequest);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Returns400IfPaymentRequestIsInvalid()
    {
        // Arrange
        var postPaymentRequest = new PostPaymentRequest
        {
            CardNumber = "invalid_card_number",
            ExpiryMonth = 12,
            ExpiryYear = 2025,
            Cvv = "123",
            Amount = 1000,
            Currency = "GBP"
        };
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();
        
        // Act
        var response = await client.PostAsJsonAsync("/api/Payments", postPaymentRequest);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Rejected", await response.Content.ReadAsStringAsync());
    }
}