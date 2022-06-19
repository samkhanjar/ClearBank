using ClearBank.DeveloperTest.Domain.Models;
using MediatR;

namespace ClearBank.DeveloperTest.Data.Command
{
    public class UpdateAccountCommand : IRequest
    {
        public Account Account { get; set; } 
        public string DataStoreType { get; set; }

        public UpdateAccountCommand(Account account, string dataStoreTpye)
        {
            Account = account;
            DataStoreType = dataStoreTpye;
        }
    }
}
