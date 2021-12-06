using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace HI.DevOps.DomainCore.Helper
{
    public static class ConfigReader
    {
        #region public constants

        private const string KEY_VALUE_NOT_SET = "";

        #endregion

        /// <summary>
        ///     Return configuration instance to read app settings and from given environment settings file
        /// </summary>
        /// <param name="path">Path where the settings file resides</param>
        /// <param name="environmentName">Settings file that is specific to desired environment. ie. Development </param>
        /// <returns></returns>
        public static IConfigurationRoot GetEnvironmentConfiguration(string path, string environmentName)
        {
            // get app settings
            if (string.IsNullOrEmpty(path)) path = Directory.GetCurrentDirectory();


            var builder = new ConfigurationBuilder().SetBasePath(path).AddJsonFile("appsettings.json", true, false);

            // get environment specific settings
            if (!string.IsNullOrEmpty(environmentName))
                builder = builder.AddJsonFile($"appsettings.{environmentName}.json", true, false);

            // environment variables
            builder = builder.AddEnvironmentVariables();

            return builder.Build();
        }

        /// <summary>
        ///     This method is an easy way to read a configuration setting.
        /// </summary>
        /// <param name="key">key in the configuration section</param>
        /// <param name="defaultValue">A desired default value if key is not found</param>
        /// <returns></returns>
        public static string QuickRead(string key, string defaultValue = KEY_VALUE_NOT_SET)
        {
            // location of settings file
            var contentRootPath = Environment.GetEnvironmentVariable(aspContentRootKey);
            var environmentName = Environment.GetEnvironmentVariable(environmentNameKey);

            // read settings in configuration class
            var configuration = GetEnvironmentConfiguration(contentRootPath, environmentName);

            // extract the key value
            var configurationValue = configuration.GetValue(key, defaultValue);

            return configurationValue;
        }

        #region private constants

        private const string environmentNameKey = "ASPNETCORE_ENVIRONMENT";
        private const string aspContentRootKey = "ASPNETCORE_CONTENTROOT";

        #endregion
    }
}