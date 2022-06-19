using ClearBank.DeveloperTest.Data.Query;
using ClearBank.DeveloperTest.Domain.Interfaces;
using ClearBank.DeveloperTest.Domain.Models;
using MediatR;

namespace ClearBank.DeveloperTest.Data.Handler
{
    public class GetAccountHandler : IRequestHandler<GetAccountQuery, Account>
    {
        private readonly IDataStoreFactory _dataStoreFactory;

        public GetAccountHandler(IDataStoreFactory dataStoreFactory)
        {
            _dataStoreFactory = dataStoreFactory;
        }

        public Task<Account> Handle(GetAccountQuery request, CancellationToken cancellationToken)
        {
            var dataStore = _dataStoreFactory.GetDataStoreType(request.DataStoreType);
            return Task.FromResult(dataStore.GetAccount(request.AccountNumber));
        }
    }
}
