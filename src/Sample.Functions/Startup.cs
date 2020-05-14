using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sample.Functions.Encryption;
using System.IO;

[assembly: FunctionsStartup(typeof(Sample.Functions.Startup))]

namespace Sample.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
               .AddEnvironmentVariables()
               .Build();

            builder.Services.AddTransient<IAccountEntity, AccountEntity>();

            // setup the encryption service
            builder.Services.AddSingleton(sp => new EncryptionOptions(config["ENCRYPTIONKey"]));
            builder.Services.AddSingleton<IEncryptionService, EncryptionService>();

            //necessary to engage custom settings during serialization
            builder.Services.AddSingleton<IContractResolver, EncryptedContractResolver>();
            builder.Services.AddSingleton(sp => new JsonSerializerSettings { ContractResolver = sp.GetService<IContractResolver>() });
            builder.Services.AddSingleton<IMessageSerializerSettingsFactory, SerializerSettingsFactory>();
            //necessary to engage custom settings during deserialization
            JsonConvert.DefaultSettings = () => builder.Services.BuildServiceProvider().GetService<JsonSerializerSettings>();
        }
    }
}
