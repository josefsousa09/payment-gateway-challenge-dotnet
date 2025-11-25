# Payment Gateway - Design Considerations

## Key Design Considerations

### Architecture
- Interface-based design: IBankService allows for easy testing and mocking
- Separation of concerns: Controllers handle HTTP, services handle business logic
- Decoupled model validation with models
- SOLID principles focused: Single responsibility, Interface segregation and Substitution principles.
- Used record types where appropriate for immutable data models.

### Payment Request Validation
Payment request card validation handled in PostPaymentRequest via custom validators in the PostPaymentRequest.
No use of annotation validation attributes due to complex validation logic e.g. Expiry date check.
Best practice as assumed to be returning Payment Status rejected for invalid requests.

### Bank Simulator Service
- Error handling: 503 Service Unavailable when card number ends with '0', save as declined payment
- Only allow 200 OK and 503 responses from bank simulator, anything else is treated as an error and is not saved in the repository

### Payment Domain Models
Although PostPaymentResponse and GetPaymentResponse are the same, they are kept seperate for future work where they may differ.
PostPaymentResponse is used to return the result of a payment request, while GetPaymentResponse is used to retrieve payment details.
Created Bank folder to hold bank related models, keeping them separate from payment models.

## Unit Testing
- Used xUnit for unit testing
- Used Theory and Inline Data to simplify validation tests with multiple possible combinations 
---

## Assumptions
- Payment requests with invalid data are rejected with only Payment Status "rejected" with 400 Bad Request response.
- Payment amount can be 0, for coupons, giveaways, etc.
- Month to be represented as 1,2,3 e,g instead of 01, 02 etc.
- Year is always represented as 4 digits.
- When bank simulator returns 503 the payment is saved with all its info but its status as "declined"




