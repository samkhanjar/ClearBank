using ClearBank.DeveloperTest.Domain.Interfaces;
using ClearBank.DeveloperTest.Domain.Models;

namespace ClearBank.DeveloperTest.Domain.Services.Services
{
    public class PaymentProcessService : IPaymentProcessService
    {
        public void DebitAmount(Account account, decimal amount)
        {
            account.Balance -= amount;
        }
    }
}
