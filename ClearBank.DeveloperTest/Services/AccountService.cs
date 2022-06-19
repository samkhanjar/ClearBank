using ClearBank.DeveloperTest.Data.Command;
using ClearBank.DeveloperTest.Data.Query;
using ClearBank.DeveloperTest.Domain.Interfaces;
using ClearBank.DeveloperTest.Domain.Models;
using MediatR;

namespace ClearBank.DeveloperTest.Domain.Services.Services
{    
    public class AccountService : IAccountService
    {
        private readonly IConfigurationService _configurationService;
        private readonly IMediator _mediator;

        public AccountService(IConfigurationService configurationService, IMediator mediator)
        {
            _configurationService = configurationService;            
            _mediator = mediator;
        }

        public Account GetAccount(string accountNumber)
        {
            return _mediator.Send(new GetAccountQuery(accountNumber, _configurationService.DataStoreType)).Result;
        }

        public void UpdateAccount(Account account)
        {
            _mediator.Send(new UpdateAccountCommand(account, _configurationService.DataStoreType));
        }
    }
}
