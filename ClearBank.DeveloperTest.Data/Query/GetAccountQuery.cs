using ClearBank.DeveloperTest.Domain.Models;
using MediatR;

namespace ClearBank.DeveloperTest.Data.Query
{
    public class GetAccountQuery : IRequest<Account>
    {
        public string AccountNumber { get; set; }
        public string DataStoreType { get; set; }

        public GetAccountQuery(string accountNumber, string dataStoreType)
        {
            AccountNumber = accountNumber;
            DataStoreType = dataStoreType;
        }
    }
}
