using ClearBank.DeveloperTest.Domain.Models;

namespace ClearBank.DeveloperTest.Domain.Interfaces
{
    public interface IAccountService
    {
        Account GetAccount(string accountNumber);
        void UpdateAccount(Account account);
    }
}
