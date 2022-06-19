using ClearBank.DeveloperTest.Data.Command;
using ClearBank.DeveloperTest.Data.Query;
using ClearBank.DeveloperTest.Domain.Interfaces;
using ClearBank.DeveloperTest.Domain.Models;
using ClearBank.DeveloperTest.Domain.Services.Services;
using FluentAssertions;
using MediatR;
using Moq;
using System.Threading;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Services
{
    public class AccountServiceTests
    {        
        private readonly Mock<IConfigurationService> _configurationService;
        private readonly Mock<IMediator> _mediator;
        private IAccountService _accountService;

        public AccountServiceTests()
        {
            _configurationService = new Mock<IConfigurationService>();
            _mediator = new Mock<IMediator>();
        }

        [Fact]
        public void GetAccount_AccountNumberNotExists_Return_Null()
        {
            // Arrange
            _mediator.Setup(x => x.Send(It.IsAny<GetAccountQuery>(), default(CancellationToken))).ReturnsAsync(() => null);
            
            _accountService = new AccountService(_configurationService.Object, _mediator.Object);            

            // Act
            var account = _accountService.GetAccount("No account number exists!");

            // Assert            
            _mediator.Verify(x => x.Send(It.IsAny<GetAccountQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            account.Should().BeNull();
        }

        [Fact]
        public void UpdateAccount_Account_UpdateInDataStore()
        {
            // Arrange
            _mediator.Setup(x => x.Send(It.IsAny<UpdateAccountCommand>(), default(CancellationToken))).Verifiable();
            _configurationService.Setup(x => x.DataStoreType).Verifiable();            
            _accountService = new AccountService(_configurationService.Object, _mediator.Object);

            // Act
            _accountService.UpdateAccount(new Account());

            // Assert
            _mediator.Verify(x => x.Send(It.IsAny<UpdateAccountCommand>(), It.IsAny<CancellationToken>()), Times.Once);            
        }

        [Fact]
        public void GetAccount_AccountNumberExists_Return_Account()
        {
            // Arrange
            _mediator.Setup(x => x.Send(It.IsAny<GetAccountQuery>(), default(CancellationToken))).ReturnsAsync(new Account());
            _configurationService.Setup(x => x.DataStoreType).Verifiable();
            _accountService = new AccountService(_configurationService.Object, _mediator.Object);

            // Act
            var account = _accountService.GetAccount("account number exists");

            // Assert
            _mediator.Verify(x => x.Send(It.IsAny<GetAccountQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _configurationService.Verify(x => x.DataStoreType, Times.Once);            
            account.Should().NotBeNull();
        }
    }
}