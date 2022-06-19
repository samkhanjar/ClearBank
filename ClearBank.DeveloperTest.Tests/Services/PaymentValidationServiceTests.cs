using ClearBank.DeveloperTest.Domain.Services.Services;
using System.Collections.Generic;
using Moq;
using Xunit;
using ClearBank.DeveloperTest.Domain.Interfaces;
using ClearBank.DeveloperTest.Domain.Enum;
using ClearBank.DeveloperTest.Domain.Models;

namespace ClearBank.DeveloperTest.Tests.Services
{
    public class PaymentValidationServiceTests
    {        
        private readonly Mock<IValidator> _bacsValidator;
        private readonly Mock<IValidator> _fasterPaymentsValidator;
        private readonly Mock<IValidator> _chapsValidator;
        private readonly PaymentValidationService _paymentValidationService;

        public PaymentValidationServiceTests()
        {
            _fasterPaymentsValidator = new Mock<IValidator>();
            _bacsValidator = new Mock<IValidator>();
            _chapsValidator = new Mock<IValidator>();

            _paymentValidationService = new PaymentValidationService
            {
                Validations = new Dictionary<PaymentScheme, IValidator>
                {
                    {PaymentScheme.Chaps, _chapsValidator.Object},
                    {PaymentScheme.FasterPayments, _fasterPaymentsValidator.Object},
                    {PaymentScheme.Bacs, _bacsValidator.Object}                    
                }
            };
        }

        [Fact]
        public void Validate_Chaps_Uses_ChapsValidator()
        {
            //Arrange
            var paymentScheme = PaymentScheme.Chaps;

            //Act
            _paymentValidationService.Validate(paymentScheme, new Account());

            //Assert
            _chapsValidator.Verify(x => x.IsValid(It.IsAny<Account>(), It.IsAny<decimal>()), Times.Once);
        }

        [Fact]
        public void Validate_Bacs_Uses_BacsValidator()
        {
            //Arrange
            var paymentScheme = PaymentScheme.Bacs;

            //Act
            _paymentValidationService.Validate(paymentScheme, new Account());

            //Assert
            _bacsValidator.Verify(x => x.IsValid(It.IsAny<Account>(), It.IsAny<decimal>()), Times.Once);
        }

        [Fact]
        public void Validate_FasterPayments_Uses_FasterPaymentsValidator()
        {
            //Arrange
            var paymentScheme = PaymentScheme.FasterPayments;

            //Act
            _paymentValidationService.Validate(paymentScheme, new Account());

            //Assert
            _fasterPaymentsValidator.Verify(x => x.IsValid(It.IsAny<Account>(), It.IsAny<decimal>()), Times.Once);
        }
    }
}
