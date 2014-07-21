using System;


namespace ClaimsAuth.Infrastructure.Configuration
{
    public static class ConfigurationContext
    {
        private static IMyConfiguration configuration;

        public static IMyConfiguration Current
        {
            get
            {
                if (configuration == null)
                {
                    configuration = new MyConfiguration();
                }
                return configuration;
            }
            set
            {
                if (value == null)
                {
                    throw new NullReferenceException("configuration");
                }
                configuration = value;
            }
        }

        public static void ResetToDefault()
        {
            configuration = new MyConfiguration();
        }
    }
}