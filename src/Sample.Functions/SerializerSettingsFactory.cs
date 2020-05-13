using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

[assembly: FunctionsStartup(typeof(Sample.Functions.Startup))]

namespace Sample.Functions
{
    public class SerializerSettingsFactory : IMessageSerializerSettingsFactory
    {
        private readonly JsonSerializerSettings _settings;

        public SerializerSettingsFactory(JsonSerializerSettings settings)
        {
            _settings = settings;
        }

        public JsonSerializerSettings CreateJsonSerializerSettings()
        {
            return _settings;
        }
    }
}
