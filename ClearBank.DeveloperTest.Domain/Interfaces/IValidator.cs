using ClearBank.DeveloperTest.Domain.Models;

namespace ClearBank.DeveloperTest.Domain.Interfaces
{
    public interface IValidator
    {
        MakePaymentResult IsValid(Account account, decimal requestAmount = 0);
    }
}
