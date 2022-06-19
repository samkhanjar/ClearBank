using ClearBank.DeveloperTest.Domain.Enum;
using ClearBank.DeveloperTest.Domain.Interfaces;
using ClearBank.DeveloperTest.Domain.Models;

namespace ClearBank.DeveloperTest.Common.PaymentValidators
{
    public class FasterPaymentsValidator : IValidator
    {
        public MakePaymentResult IsValid(Account account, decimal requestAmount = 0)
        {
            var result = new MakePaymentResult();

            if (account != null && account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments) && 
                requestAmount != 0 && account.Balance > requestAmount)
                result.Success = true;
            
            return result;
        }
    }
}
