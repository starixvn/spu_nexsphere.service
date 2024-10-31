using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;

namespace NexSphere.Core
{
    public interface IAppSettings
    {
        string DataAccessApiSecretKey { get; }

        string ApiIssuer { get; }

        string GatewaySecretKey { get; }

        string SentryDsn { get; }

        string SPName { get; }

        IConfiguration Configuration { get; }

        T GetValue<T>(string key);

        T GetSection<T>(string key);
    }

    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration configuration;

        private readonly string dataAccessApiSecretKey;

        private readonly string apiIssuer;

        private readonly string gatewaySecretKey;

        private readonly string sentryDsn;

        private readonly string spName;

        public AppSettings(IConfiguration configuration)
        {
            this.configuration = configuration;

            dataAccessApiSecretKey = GetValue<string>("ApiSettings:DataAccess:Secret") ?? string.Empty;

            apiIssuer = GetValue<string>("ApiSettings:Issuer") ?? string.Empty;

            gatewaySecretKey = GetValue<string>("ApiSettings:Gateway:Secret") ?? string.Empty;

            sentryDsn = GetValue<string>("Sentry:Dsn");

            spName = GetValue<string>(nameof(SPName));

        }

        public string DataAccessApiSecretKey => dataAccessApiSecretKey;
        
        public string ApiIssuer => apiIssuer;

        public string GatewaySecretKey => gatewaySecretKey;

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
