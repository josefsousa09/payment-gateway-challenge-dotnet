using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Bank.Responses
{
    public record BankAuthorisationResponse
    {
        [JsonPropertyName("authorized")]
        public bool Authorized { get; init; }

        [JsonPropertyName("authorization_code")]
        public string AuthorizationCode { get; init; }
    }
}
