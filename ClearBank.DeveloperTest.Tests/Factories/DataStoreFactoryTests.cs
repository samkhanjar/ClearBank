using ClearBank.DeveloperTest.Data.Data;
using ClearBank.DeveloperTest.Domain.Services.Factories;
using FluentAssertions;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Factories
{
    public class DataStoreFactoryTests
    {
        [Fact]
        public void GetDataStoreType_WithDefaultDataStoreConfig_Factory_To_Create_DefaultDataStore()
        {
            // Arrange
            var factory = new DataStoreFactory();

            // Act
            var dataStore = factory.GetDataStoreType("");

            // Assert
            dataStore.Should().BeOfType<AccountDataStore>();
        }

        [Fact]
        public void GetDataStoreType_WithBackupDatastoreConfig_Factory_To_Create_BackupDataStore()
        {
            // Arrange
            var factory = new DataStoreFactory();

            // Act
            var dataStore = factory.GetDataStoreType("Backup");

            // Assert
            dataStore.Should().BeOfType<BackupAccountDataStore>();
        }
    }
}
