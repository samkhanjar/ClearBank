using ClearBank.DeveloperTest.Data.Command;
using ClearBank.DeveloperTest.Domain.Interfaces;
using MediatR;

namespace ClearBank.DeveloperTest.Data.Handler
{
    public class UpdateAccountHandler : IRequestHandler<UpdateAccountCommand>
    {
        private readonly IDataStoreFactory _dataStoreFactory;

        public UpdateAccountHandler(IDataStoreFactory dataStoreFactory)
        {
            _dataStoreFactory = dataStoreFactory;
        }

        public Task<Unit> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        {
            var dataStore = _dataStoreFactory.GetDataStoreType(request.DataStoreType);
            dataStore.UpdateAccount(request.Account);
            return Task.FromResult(Unit.Value);
        }
    }
}
