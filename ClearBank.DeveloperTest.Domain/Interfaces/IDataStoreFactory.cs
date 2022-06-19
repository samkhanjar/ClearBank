namespace ClearBank.DeveloperTest.Domain.Interfaces
{
    public interface IDataStoreFactory
    {
        IAccountDataStore GetDataStoreType(string dataStoreType);
    }
}
