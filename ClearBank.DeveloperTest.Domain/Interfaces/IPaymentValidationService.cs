using ClearBank.DeveloperTest.Domain.Enum;
using ClearBank.DeveloperTest.Domain.Models;

namespace ClearBank.DeveloperTest.Domain.Interfaces
{
    public interface IPaymentValidationService
    {
        MakePaymentResult Validate(PaymentScheme paymentScheme, Account account, decimal amount = 0);
    }
}
