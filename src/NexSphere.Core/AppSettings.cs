using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;
using System.Xml.Linq;

namespace NexSphere.Core
{
    public interface IAppSettings
    {
        string ApiSecretKey { get; }

        string ApiIssuer { get; }

        string SentryDsn { get; }

        string SPName { get; }

        IConfiguration Configuration { get; }

        T GetValue<T>(string key);

        T GetSection<T>(string key);
    }

    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration configuration;

        private readonly string apiSecretKey;

        private readonly string apiIssuer;

        private readonly string sentryDsn;

        private readonly string spName;

        public AppSettings(IConfiguration configuration)
        {
            this.configuration = configuration;

            apiSecretKey = GetValue<string>("ApiSettings:Secret") ?? string.Empty;

            apiIssuer = GetValue<string>("ApiSettings:Issuer") ?? string.Empty;

            sentryDsn = GetValue<string>("Sentry:Dsn");

            spName = GetValue<string>(nameof(SPName));

        }

        public string ApiSecretKey => apiSecretKey;
        
        public string ApiIssuer => apiIssuer;

        public string SentryDsn => sentryDsn;

        public string SPName => spName;

        public T GetValue<T>(string key)
        {
            try
            {
                var value = configuration[key];

                var typeConverter = TypeDescriptor.GetConverter(typeof(T));

                return (T)typeConverter.ConvertFromString(value);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"Key: {key}", ex);
            }
        }

        public T GetSection<T>(string key)
        {
            var schedulerSection = configuration.GetSection(key);

            return schedulerSection.Get<T>();
        }

        public IConfiguration Configuration => configuration;

    }
}
