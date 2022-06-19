using ClearBank.DeveloperTest.Domain.Models;

namespace ClearBank.DeveloperTest.Domain.Interfaces
{
    public interface IPaymentService
    {
        MakePaymentResult MakePayment(MakePaymentRequest request);
    }
}
