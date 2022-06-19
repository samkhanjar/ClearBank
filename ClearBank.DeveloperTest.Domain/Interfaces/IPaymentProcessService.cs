using ClearBank.DeveloperTest.Domain.Models;

namespace ClearBank.DeveloperTest.Domain.Interfaces
{
    public interface IPaymentProcessService
    {
        void DebitAmount(Account account, decimal amount);
    }
}
