using ClearBank.DeveloperTest.Domain.Interfaces;
using ClearBank.DeveloperTest.Domain.Models;

namespace ClearBank.DeveloperTest.Domain.Services.Services
{    
    public class AccountService : IAccountService
    {
        private readonly IConfigurationService _configurationService;
        private readonly IDataStoreFactory _dataStoreFactory;  

        public AccountService(IDataStoreFactory dataStoreFactory, IConfigurationService configurationService)
        {
            _dataStoreFactory = dataStoreFactory;
            _configurationService = configurationService;            
        }

        public Account GetAccount(string accountNumber)
        {
            var dataStore = _dataStoreFactory.GetDataStoreType(_configurationService.DataStoreType);
            return dataStore.GetAccount(accountNumber);
        }

        public void UpdateAccount(Account account)
        {
            var dataStore = _dataStoreFactory.GetDataStoreType(_configurationService.DataStoreType);
            dataStore.UpdateAccount(account);
        }
    }
}
