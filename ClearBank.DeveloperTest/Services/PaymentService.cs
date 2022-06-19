using ClearBank.DeveloperTest.Domain.Interfaces;
using ClearBank.DeveloperTest.Domain.Models;

namespace ClearBank.DeveloperTest.Domain.Services.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IAccountService _accountService;
        private readonly IPaymentValidationService _paymentValidationService;
        private readonly IPaymentProcessService _paymentProcessService;        

        public PaymentService(IAccountService accountService, 
            IPaymentValidationService paymentValidationService,
            IPaymentProcessService paymentProcessService)
        {
            _accountService = accountService;
            _paymentValidationService = paymentValidationService;
            _paymentProcessService = paymentProcessService;
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var account = _accountService.GetAccount(request.DebtorAccountNumber);

            var result = new MakePaymentResult();

            if (account != null)
            {
                result = _paymentValidationService.Validate(request.PaymentScheme, account, request.Amount);

                if (result.Success)
                {
                    _paymentProcessService.DebitAmount(account, request.Amount);
                    _accountService.UpdateAccount(account);
                    return result;
                }

                return result;
            }

            result.Success = false;
            return result;
        }
    }
}
