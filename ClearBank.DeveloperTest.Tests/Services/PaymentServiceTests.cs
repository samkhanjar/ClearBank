using ClearBank.DeveloperTest.Data.Command;
using ClearBank.DeveloperTest.Data.Query;
using ClearBank.DeveloperTest.Domain.Enum;
using ClearBank.DeveloperTest.Domain.Interfaces;
using ClearBank.DeveloperTest.Domain.Models;
using ClearBank.DeveloperTest.Domain.Services.Services;
using FizzWare.NBuilder;
using FluentAssertions;
using MediatR;
using Moq;
using System.Threading;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Services
{
    public class PaymentServiceTests
    {
        private readonly Mock<IPaymentProcessService> _paymentProcessService;        
        private readonly Mock<IConfigurationService> _configurationService;
        private readonly Mock<IMediator> _mediator;
        private readonly PaymentValidationService _paymentValidationService;

        public PaymentServiceTests()
        {
            _paymentProcessService = new Mock<IPaymentProcessService>();
            _configurationService = new Mock<IConfigurationService>();
            _mediator = new Mock<IMediator>();
            _paymentValidationService = new PaymentValidationService();
        }

        [Fact]
        public void MakePayment_FasterPaymentsScheme_And_AccountNotExist_Return_False()
        {
            // Arrange
            _mediator.Setup(x => x.Send(It.IsAny<GetAccountQuery>(), default(CancellationToken))).ReturnsAsync(() => null);
            _paymentProcessService.Setup(x => x.DebitAmount(It.IsAny<Account>(), It.IsAny<decimal>()));

            var paymentService = new PaymentService(
                new AccountService(_configurationService.Object, _mediator.Object), 
                _paymentValidationService, 
                _paymentProcessService.Object);

            var makePaymentRequest = Builder<MakePaymentRequest>.CreateNew()
                .With(x => x.PaymentScheme = PaymentScheme.FasterPayments)
                .Build();

            // Act
            var result = paymentService.MakePayment(makePaymentRequest);

            // Assert            
            _mediator.Verify(x => x.Send(It.IsAny<GetAccountQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(x => x.Send(It.IsAny<UpdateAccountCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _configurationService.Verify(x => x.DataStoreType, Times.Exactly(1));       
            _paymentProcessService.Verify(x => x.DebitAmount(It.IsAny<Account>(), It.IsAny<decimal>()), Times.Never);

            result.Success.Should().BeFalse();
        }

        [Fact]
        public void MakePayment_ChapsPaymentScheme_And_AccountNotExist_Return_False()
        {
            // Arrange
            _mediator.Setup(x => x.Send(It.IsAny<GetAccountQuery>(), default(CancellationToken))).ReturnsAsync(() => null);            
            _paymentProcessService.Setup(x => x.DebitAmount(It.IsAny<Account>(), It.IsAny<decimal>()));

            var paymentService = new PaymentService(
                new AccountService(_configurationService.Object, _mediator.Object),
                _paymentValidationService,
                _paymentProcessService.Object);

            var makePaymentRequest = Builder<MakePaymentRequest>.CreateNew()
                .With(x => x.PaymentScheme = PaymentScheme.Chaps)
                .Build();

            // Act
            var result = paymentService.MakePayment(makePaymentRequest);

            // Assert            
            _mediator.Verify(x => x.Send(It.IsAny<GetAccountQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(x => x.Send(It.IsAny<UpdateAccountCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _configurationService.Verify(x => x.DataStoreType, Times.Exactly(1));            
            _paymentProcessService.Verify(x => x.DebitAmount(It.IsAny<Account>(), It.IsAny<decimal>()), Times.Never);

            result.Success.Should().BeFalse();
        }

        [Fact]
        public void MakePayment_BacsPaymentScheme_And_AccountNotExist_Return_False()
        {
            // Arrange
            _mediator.Setup(x => x.Send(It.IsAny<GetAccountQuery>(), default(CancellationToken))).ReturnsAsync(() => null);            
            _paymentProcessService.Setup(x => x.DebitAmount(It.IsAny<Account>(), It.IsAny<decimal>()));

            var paymentService = new PaymentService(
                new AccountService(_configurationService.Object, _mediator.Object),
                _paymentValidationService,
                _paymentProcessService.Object);

            var makePaymentRequest = Builder<MakePaymentRequest>.CreateNew()
                .With(x => x.PaymentScheme = PaymentScheme.Bacs)
                .Build();

            // Act
            var result = paymentService.MakePayment(makePaymentRequest);

            // Assert
            _mediator.Verify(x => x.Send(It.IsAny<UpdateAccountCommand>(), default(CancellationToken)), Times.Never);
            _mediator.Verify(x => x.Send(It.IsAny<GetAccountQuery>(), default(CancellationToken)), Times.Once);
            _configurationService.Verify(x => x.DataStoreType, Times.Exactly(1));            
            _paymentProcessService.Verify(x => x.DebitAmount(It.IsAny<Account>(), It.IsAny<decimal>()), Times.Never);

            result.Success.Should().BeFalse();
        }

        [Fact]
        public void MakePayment_FasterPaymentsScheme_And_AccountExists_AndFasterPaymentsIsAllowed_And_RequestAmount_Is_Less_Than_Balance_Return_True_And_Update_Balance()
        {
            // Arrange
            var account = Builder<Account>.CreateNew()
                .With(x => x.AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments)
                .With(x => x.Balance = 100)
                .Build();

            _mediator.Setup(x => x.Send(It.IsAny<GetAccountQuery>(), default(CancellationToken))).ReturnsAsync(() => account);

            var makePaymentRequest = Builder<MakePaymentRequest>.CreateNew()
                .With(x => x.PaymentScheme = PaymentScheme.FasterPayments)
                .With(x => x.Amount = 40)
                .Build();

            _paymentProcessService.Setup(x => x.DebitAmount(account, It.IsAny<decimal>()))
                .Callback(() => account.Balance -= makePaymentRequest.Amount);

            var paymentService = new PaymentService(
                new AccountService(_configurationService.Object, _mediator.Object),
                _paymentValidationService,
                _paymentProcessService.Object);

            // Act
            var result = paymentService.MakePayment(makePaymentRequest);

            // Assert            
            _mediator.Verify(x => x.Send(It.IsAny<GetAccountQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(x => x.Send(It.IsAny<UpdateAccountCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _configurationService.Verify(x => x.DataStoreType, Times.Exactly(2));            
            _paymentProcessService.Verify(x => x.DebitAmount(It.IsAny<Account>(), It.IsAny<decimal>()), Times.Once);

            result.Success.Should().BeTrue();
            account.Balance.Should().Be(60);
        }

        [Fact]
        public void MakePayment_ChapsPaymentScheme_And_AccountExists_AndChapsIsAllowed_And_Account_Is_Live_Return_True_And_Update_Balance()
        {
            // Arrange
            var account = Builder<Account>.CreateNew()
                .With(x => x.AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps)
                .With(x => x.Balance = 100)
                .With(x => x.Status = AccountStatus.Live)
                .Build();

            _mediator.Setup(x => x.Send(It.IsAny<GetAccountQuery>(), default(CancellationToken))).ReturnsAsync(() => account);            

            var makePaymentRequest = Builder<MakePaymentRequest>.CreateNew()
                .With(x => x.PaymentScheme = PaymentScheme.Chaps)
                .With(x => x.Amount = 40)
                .Build();

            _paymentProcessService.Setup(x => x.DebitAmount(account, It.IsAny<decimal>()))
                .Callback(() => account.Balance -= makePaymentRequest.Amount);

            var paymentService = new PaymentService(
                new AccountService(_configurationService.Object, _mediator.Object),
                _paymentValidationService, 
                _paymentProcessService.Object);

            // Act
            var result = paymentService.MakePayment(makePaymentRequest);

            // Assert            
            _mediator.Verify(x => x.Send(It.IsAny<GetAccountQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(x => x.Send(It.IsAny<UpdateAccountCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _configurationService.Verify(x => x.DataStoreType, Times.Exactly(2));            
            _paymentProcessService.Verify(x => x.DebitAmount(It.IsAny<Account>(), It.IsAny<decimal>()), Times.Once);

            result.Success.Should().BeTrue();
            account.Balance.Should().Be(60);
        }

        [Fact]
        public void MakePayment_BacsPaymentScheme_And_AccountExists_AndBacsIsAllowed_Return_True_And_Update_Balance()
        {
            // Arrange
            var account = Builder<Account>.CreateNew()
                .With(x => x.AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs)
                .With(x => x.Balance = 100)
                .Build();
            
            _mediator.Setup(x => x.Send(It.IsAny<GetAccountQuery>(), default(CancellationToken))).ReturnsAsync(() => account);

            var makePaymentRequest = Builder<MakePaymentRequest>.CreateNew()
                .With(x => x.PaymentScheme = PaymentScheme.Bacs)
                .With(x => x.Amount = 40)
                .Build();

            _paymentProcessService.Setup(x => x.DebitAmount(account, It.IsAny<decimal>()))
                .Callback(() => account.Balance -= makePaymentRequest.Amount);

            var paymentService = new PaymentService(
                new AccountService(_configurationService.Object, _mediator.Object),
                _paymentValidationService,
                _paymentProcessService.Object);

            // Act
            var result = paymentService.MakePayment(makePaymentRequest);

            // Assert            
            _mediator.Verify(x => x.Send(It.IsAny<UpdateAccountCommand>(), default(CancellationToken)), Times.Once);
            _mediator.Verify(x => x.Send(It.IsAny<GetAccountQuery>(), default(CancellationToken)), Times.Once);
            _configurationService.Verify(x => x.DataStoreType, Times.Exactly(2));            
            _paymentProcessService.Verify(x => x.DebitAmount(It.IsAny<Account>(), It.IsAny<decimal>()), Times.Once);

            result.Success.Should().BeTrue();
            account.Balance.Should().Be(60);
        }

        [Fact]
        public void MakePayment_FasterPaymentsScheme_And_AccountExists_AndFasterPaymentsNotAllowed_Return_False()
        {
            // Arrange
            var account = Builder<Account>.CreateNew()
                .With(x => x.AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs)
                .With(x => x.Balance = 100)
                .Build();

            _mediator.Setup(x => x.Send(It.IsAny<GetAccountQuery>(), default(CancellationToken))).ReturnsAsync(() => account);
            _paymentProcessService.Setup(x => x.DebitAmount(It.IsAny<Account>(), It.IsAny<decimal>()));            

            var paymentService = new PaymentService(
                new AccountService(_configurationService.Object, _mediator.Object),
                _paymentValidationService,
                _paymentProcessService.Object);

            var makePaymentRequest = Builder<MakePaymentRequest>.CreateNew()
                .With(x => x.PaymentScheme = PaymentScheme.FasterPayments)
                .Build();

            // Act
            var result = paymentService.MakePayment(makePaymentRequest);

            // Assert            
            _mediator.Verify(x => x.Send(It.IsAny<GetAccountQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(x => x.Send(It.IsAny<UpdateAccountCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _configurationService.Verify(x => x.DataStoreType, Times.Exactly(1));            
            _paymentProcessService.Verify(x => x.DebitAmount(It.IsAny<Account>(), It.IsAny<decimal>()), Times.Never);

            result.Success.Should().BeFalse();
            account.Balance.Should().Be(100);
        }

        [Fact]
        public void MakePayment_ChapsPaymentScheme_And_AccountExists_AndChapsNotAllowed_Return_False()
        {
            // Arrange
            var account = Builder<Account>.CreateNew()
                .With(x => x.AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs)
                .With(x => x.Balance = 100)
                .Build();

            _mediator.Setup(x => x.Send(It.IsAny<GetAccountQuery>(), default(CancellationToken))).ReturnsAsync(() => account);            
            _paymentProcessService.Setup(x => x.DebitAmount(It.IsAny<Account>(), It.IsAny<decimal>()));            

            var paymentService = new PaymentService(
                new AccountService(_configurationService.Object, _mediator.Object),
                _paymentValidationService,
                _paymentProcessService.Object);

            var makePaymentRequest = Builder<MakePaymentRequest>.CreateNew()
                .With(x => x.PaymentScheme = PaymentScheme.Chaps)
                .Build();

            // Act
            var result = paymentService.MakePayment(makePaymentRequest);

            // Assert            
            _mediator.Verify(x => x.Send(It.IsAny<GetAccountQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(x => x.Send(It.IsAny<UpdateAccountCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _configurationService.Verify(x => x.DataStoreType, Times.Exactly(1));            
            _paymentProcessService.Verify(x => x.DebitAmount(It.IsAny<Account>(), It.IsAny<decimal>()), Times.Never);

            result.Success.Should().BeFalse();
            account.Balance.Should().Be(100);
        }

        [Fact]
        public void MakePayment_BacsPaymentScheme_And_AccountExists_AndBacsNotAllowed_Return_False()
        {
            // Arrange
            var account = Builder<Account>.CreateNew()
                .With(x => x.AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps)
                .With(x => x.Balance = 100)
                .Build();

            _mediator.Setup(x => x.Send(It.IsAny<GetAccountQuery>(), default(CancellationToken))).ReturnsAsync(() => account);            
            _paymentProcessService.Setup(x => x.DebitAmount(account, It.IsAny<decimal>()));            

            var paymentService = new PaymentService(
                new AccountService(_configurationService.Object, _mediator.Object),
                _paymentValidationService,
                _paymentProcessService.Object);

            var makePaymentRequest = Builder<MakePaymentRequest>.CreateNew()
                .With(x => x.PaymentScheme = PaymentScheme.Bacs)
                .Build();

            // Act
            var result = paymentService.MakePayment(makePaymentRequest);

            // Assert            
            _mediator.Verify(x => x.Send(It.IsAny<UpdateAccountCommand>(), default(CancellationToken)), Times.Never);
            _mediator.Verify(x => x.Send(It.IsAny<GetAccountQuery>(), default(CancellationToken)), Times.Once);
            _configurationService.Verify(x => x.DataStoreType, Times.Exactly(1));            
            _paymentProcessService.Verify(x => x.DebitAmount(It.IsAny<Account>(), It.IsAny<decimal>()), Times.Never);

            result.Success.Should().BeFalse();
            account.Balance.Should().Be(100);
        }

        [Fact]
        public void MakePayment_FasterPaymentsScheme_And_AccountExists_AndFasterPaymentsIsAllowed_And_RequestAmount_Is_More_Than_Balance_Return_False()
        {
            // Arrange
            var account = Builder<Account>.CreateNew()
                .With(x => x.AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments)
                .With(x => x.Balance = 100)
                .Build();

            _mediator.Setup(x => x.Send(It.IsAny<GetAccountQuery>(), default(CancellationToken))).ReturnsAsync(() => account);            
            _paymentProcessService.Setup(x => x.DebitAmount(account, It.IsAny<decimal>()));            

            MakePaymentRequest makePaymentRequest = Builder<MakePaymentRequest>.CreateNew()
                .With(x => x.PaymentScheme = PaymentScheme.FasterPayments)
                .With(x => x.Amount = 1000)
                .Build();

            var paymentService = new PaymentService(
                new AccountService(_configurationService.Object, _mediator.Object),
                _paymentValidationService,
                _paymentProcessService.Object);

            // Act
            var result = paymentService.MakePayment(makePaymentRequest);

            // Assert
            _mediator.Verify(x => x.Send(It.IsAny<UpdateAccountCommand>(), default(CancellationToken)), Times.Never);
            _mediator.Verify(x => x.Send(It.IsAny<GetAccountQuery>(), default(CancellationToken)), Times.Once);
            _configurationService.Verify(x => x.DataStoreType, Times.Exactly(1));            
            _paymentProcessService.Verify(x => x.DebitAmount(It.IsAny<Account>(), It.IsAny<decimal>()), Times.Never);

            result.Success.Should().BeFalse();
            account.Balance.Should().Be(100);
        }

        [Fact]
        public void MakePayment_ChapsPaymentScheme_And_AccountExists_AndChapsAllowed_And_Account_Is_Disabled_Return_False()
        {
            // Arrange
            var account = Builder<Account>.CreateNew()
                .With(x => x.AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps)
                .With(x => x.Balance = 100)
                .With(x => x.Status = AccountStatus.Disabled)
                .Build();

            _mediator.Setup(x => x.Send(It.IsAny<GetAccountQuery>(), default(CancellationToken))).ReturnsAsync(() => account);            
            _paymentProcessService.Setup(x => x.DebitAmount(It.IsAny<Account>(), It.IsAny<decimal>()));            

            var paymentService = new PaymentService(
                new AccountService(_configurationService.Object, _mediator.Object),
                _paymentValidationService,
                _paymentProcessService.Object);

            var makePaymentRequest = Builder<MakePaymentRequest>.CreateNew()
                .With(x => x.PaymentScheme = PaymentScheme.Chaps)
                .Build();

            // Act
            var result = paymentService.MakePayment(makePaymentRequest);

            // Assert            
            _mediator.Verify(x => x.Send(It.IsAny<GetAccountQuery>(), It.IsAny<CancellationToken>()), Times.Once);            
            _mediator.Verify(x => x.Send(It.IsAny<UpdateAccountCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _configurationService.Verify(x => x.DataStoreType, Times.Exactly(1));            
            _paymentProcessService.Verify(x => x.DebitAmount(It.IsAny<Account>(), It.IsAny<decimal>()), Times.Never);

            result.Success.Should().BeFalse();
            account.Balance.Should().Be(100);
        }

        [Fact]
        public void MakePayment_FasterPaymentsScheme_And_AccountExists_AndFasterPaymentsIsNotAllowedAllowed_And_RequestAmount_Is_Less_Than_Balance_Return_False()
        {
            // Arrange
            var account = Builder<Account>.CreateNew()
                .With(x => x.AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs)
                .With(x => x.Balance = 100)
                .Build();

            _mediator.Setup(x => x.Send(It.IsAny<GetAccountQuery>(), default(CancellationToken))).ReturnsAsync(() => account);
            _paymentProcessService.Setup(x => x.DebitAmount(It.IsAny<Account>(), It.IsAny<decimal>()));            

            var paymentService = new PaymentService(
                new AccountService(_configurationService.Object, _mediator.Object),
                _paymentValidationService,
                _paymentProcessService.Object);

            var makePaymentRequest = Builder<MakePaymentRequest>.CreateNew()
                .With(x => x.PaymentScheme = PaymentScheme.FasterPayments)
                .With(x => x.Amount = 40)
                .Build();

            // Act
            var result = paymentService.MakePayment(makePaymentRequest);

            // Assert            
            _mediator.Verify(x => x.Send(It.IsAny<GetAccountQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(x => x.Send(It.IsAny<UpdateAccountCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _configurationService.Verify(x => x.DataStoreType, Times.Exactly(1));            
            _paymentProcessService.Verify(x => x.DebitAmount(It.IsAny<Account>(), It.IsAny<decimal>()), Times.Never);

            result.Success.Should().BeFalse();
            account.Balance.Should().Be(100);
        }
    }
}
