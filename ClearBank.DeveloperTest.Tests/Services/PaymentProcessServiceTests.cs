using ClearBank.DeveloperTest.Domain.Interfaces;
using ClearBank.DeveloperTest.Domain.Models;
using ClearBank.DeveloperTest.Domain.Services.Services;
using FizzWare.NBuilder;
using FluentAssertions;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Services
{
    public class PaymentProcessServiceTests
    {
        private readonly IPaymentProcessService _paymentProcessService;

        public PaymentProcessServiceTests()
        {
            _paymentProcessService = new PaymentProcessService();
        }

        [Fact]
        public void DeductAmountFromAccount_WithDeductRequest_Deduct_Money_From_Account()
        {
            // Arrange
            var account = Builder<Account>.CreateNew().With(x => x.Balance = 100).Build();

            // Act
            _paymentProcessService.DebitAmount(account, 40);

            // Assert
            account.Balance.Should().Be(60);
        }
    }
}
