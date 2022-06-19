using System.Configuration;

namespace ClearBank.DeveloperTest.Domain.Services.Services
{     
    public class ConfigurationService
    {        
        public string GetConfig(string key)
        {            
            return ConfigurationManager.AppSettings[key];
        }
    }
}
