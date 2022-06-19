using ClearBank.DeveloperTest.Common.Constants;
using ClearBank.DeveloperTest.Data.Data;
using ClearBank.DeveloperTest.Domain.Interfaces;

namespace ClearBank.DeveloperTest.Domain.Services.Factories
{
    public class DataStoreFactory : IDataStoreFactory
    {        
        public IAccountDataStore GetDataStoreType(string dataStoreType)
        {
            return dataStoreType == DataStoreTypeConstant.Backup ? new BackupAccountDataStore() : new AccountDataStore();
        }
    }
}
