
namespace PaymentGateway.Api.Models.Payment.Requests
{
    public record PostPaymentRequest
    {

        public string CardNumber { get; init; }

        public int ExpiryMonth { get; init; }

        public int ExpiryYear { get; init; }

        public string Currency { get; init; }

        public int Amount { get; init; }


        public string Cvv { get; init; }



        public static bool IsValid(PostPaymentRequest paymentRequest)
        {
            return IsValidCardNumber(paymentRequest.CardNumber)
                 && IsValidExpiryDate(paymentRequest.ExpiryMonth, paymentRequest.ExpiryYear)
                && IsValidCurrency(paymentRequest.Currency)
                && IsValidAmount(paymentRequest.Amount)
                && IsValidCvv(paymentRequest.Cvv);
        }

        private static bool IsValidCardNumber(string cardNumber)
        {
            return !string.IsNullOrEmpty(cardNumber)
             && cardNumber.Length is >= 14 and <= 19
                 && cardNumber.All(char.IsDigit);

        }

        private static bool IsValidExpiryDate(int month, int year)
        {
            if (month < 1 || month > 12)
            {
                return false;
            }
            try
            {
                DateTime expiryDate = new(year, month, DateTime.DaysInMonth(year, month));
                return expiryDate >= DateTime.Now;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }

        private static bool IsValidCurrency(string currency)
        {
            string[] currencies = ["GBP", "AED", "EUR"];
            return !string.IsNullOrEmpty(currency)
                && currencies.Contains(currency.ToUpper());

        }

        private static bool IsValidAmount(int amount)
        {
            return amount >= 0;
        }

        private static bool IsValidCvv(string cvv)
        {
            return !string.IsNullOrEmpty(cvv)
                && (cvv.Length == 3 || cvv.Length == 4)
                && cvv.All(char.IsDigit);
        }
    }
}
