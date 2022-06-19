using ClearBank.DeveloperTest.Domain.Interfaces;
using ClearBank.DeveloperTest.Domain.Models;
using ClearBank.DeveloperTest.Domain.Services.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Services
{
    public class AccountServiceTests
    {        
        private readonly Mock<IAccountDataStore> _accountDataStore;
        private readonly Mock<IDataStoreFactory> _dataStoreFactory;
        private readonly Mock<IConfigurationService> _configurationService;
        private IAccountService _accountService;

        public AccountServiceTests()
        {
            _accountDataStore = new Mock<IAccountDataStore>();
            _dataStoreFactory = new Mock<IDataStoreFactory>();
            _configurationService = new Mock<IConfigurationService>();
        }

        [Fact]
        public void GetAccount_AccountNumberNotExists_Return_Null()
        {
            // Arrange            
            _accountDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(() => null);
            _configurationService.Setup(x => x.DataStoreType).Verifiable();
            _dataStoreFactory.Setup(x => x.GetDataStoreType(It.IsAny<string>())).Returns(_accountDataStore.Object);            
            _accountService = new AccountService(_dataStoreFactory.Object, _configurationService.Object);            

            // Act
            var account = _accountService.GetAccount("No account number exists!");

            // Assert
            _accountDataStore.Verify(x => x.GetAccount(It.IsAny<string>()), Times.Once);
            _dataStoreFactory.Verify(x => x.GetDataStoreType(It.IsAny<string>()), Times.Once);
            _configurationService.Verify(x => x.DataStoreType, Times.Once);            
            account.Should().BeNull();
        }

        [Fact]
        public void UpdateAccount_Account_UpdateInDataStore()
        {
            // Arrange
            _accountDataStore.Setup(x => x.UpdateAccount(It.IsAny<Account>())).Verifiable();
            _configurationService.Setup(x => x.DataStoreType).Verifiable();            
            _dataStoreFactory.Setup(x => x.GetDataStoreType(It.IsAny<string>())).Returns(_accountDataStore.Object);            
            _accountService = new AccountService(_dataStoreFactory.Object, _configurationService.Object);

            // Act
            _accountService.UpdateAccount(new Account());

            // Assert
            _dataStoreFactory.Verify(x => x.GetDataStoreType(It.IsAny<string>()), Times.Once);
            _configurationService.Verify(x => x.DataStoreType, Times.Once);
            _accountDataStore.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Once);
        }

        [Fact]
        public void GetAccount_AccountNumberExists_Return_Account()
        {
            // Arrange
            _configurationService.Setup(x => x.DataStoreType).Verifiable();
            _accountDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(new Account());
            _dataStoreFactory.Setup(x => x.GetDataStoreType(It.IsAny<string>())).Returns(_accountDataStore.Object);            
            _accountService = new AccountService(_dataStoreFactory.Object, _configurationService.Object);

            // Act
            var account = _accountService.GetAccount("account number exists");

            // Assert
            _dataStoreFactory.Verify(x => x.GetDataStoreType(It.IsAny<string>()), Times.Once);
            _configurationService.Verify(x => x.DataStoreType, Times.Once);
            _accountDataStore.Verify(x => x.GetAccount(It.IsAny<string>()), Times.Once);
            account.Should().NotBeNull();
        }
    }
}