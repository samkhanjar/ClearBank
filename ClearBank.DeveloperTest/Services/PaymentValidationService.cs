using ClearBank.DeveloperTest.Common.PaymentValidators;
using ClearBank.DeveloperTest.Domain.Enum;
using ClearBank.DeveloperTest.Domain.Interfaces;
using ClearBank.DeveloperTest.Domain.Models;
using System.Collections.Generic;

namespace ClearBank.DeveloperTest.Domain.Services.Services
{
    public class PaymentValidationService : IPaymentValidationService
    {
        public Dictionary<PaymentScheme, IValidator> Validations { get; set; }

        public PaymentValidationService()
        {
            Validations = new Dictionary<PaymentScheme, IValidator>
            {
                {PaymentScheme.FasterPayments, new FasterPaymentsValidator()},
                {PaymentScheme.Chaps, new ChapsPaymentsValidator()},
                {PaymentScheme.Bacs, new BacsValidator()}
            };
        }

        public MakePaymentResult Validate(PaymentScheme paymentScheme, Account account, decimal amount = 0)
        {
            return Validations[paymentScheme].IsValid(account, amount);
        }
    }
}
