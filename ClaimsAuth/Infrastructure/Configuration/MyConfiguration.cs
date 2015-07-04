using System;
using System.Collections.Generic;
using System.Configuration;


namespace ClaimsAuth.Infrastructure.Configuration
{
    public class MyConfiguration : IMyConfiguration
    {
        private readonly Dictionary<String, String> cachedSettings;

        private readonly Object thisLock = new Object();


        public MyConfiguration()
        {
            this.cachedSettings = new Dictionary<string, string>();
        }


        public string GetDatabaseConnectionString()
        {
            return GetConnectionString("MyConnectionString");
        }


        private string GetConnectionString(String connectionStringName)
        {
            if (string.IsNullOrEmpty(connectionStringName))
            {
                throw new ArgumentNullException("connectionStringName");
            }

            string cachedSetting;
            if (cachedSettings.TryGetValue(connectionStringName, out cachedSetting))
            {
                return cachedSetting;
            }

            // need thread safety if we are writing to the dictionary. 
            // Multiple simultaneous requests can try to write the same keys at the same time, causing an exception.
            String result;
            lock (thisLock)
            {
                result = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
                if (!cachedSettings.ContainsKey(connectionStringName))
                {
                    cachedSettings.Add(connectionStringName, result);
                }
            }
            return result;
        }


        private string GetAppSetting(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            string cachedSetting;
            if (cachedSettings.TryGetValue(key, out cachedSetting))
            {
                return cachedSetting;
            }

            // need thread safety if we are writing to the dictionary. 
            // Multiple simultaneous requests can try to write the same keys at the same time, causing an exception.
            String result;
            lock (thisLock)
            {
                result = ConfigurationManager.AppSettings[key];
                if (!cachedSettings.ContainsKey(key))
                {
                    cachedSettings.Add(key, result);
                }
            }
            return result;
        }

    }
}