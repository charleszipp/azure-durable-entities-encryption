# Application Level Encryption for Azure Durable Entities

The code within this repository demonstrates a means by which durable entity state can be encrypted at the application layer. A key is configured for the function app. That key is then used during the serialization process to encrypt any properties annotated with the `Encrypted` attribute.

## Dependencies

[.NET Core 3.1](https://dotnet.microsoft.com/download)
[Azure Storage Emulator](https://go.microsoft.com/fwlink/?linkid=717179&clcid=0x409)
[Azure Storage Explorer](https://azure.microsoft.com/en-us/features/storage-explorer/)

## Bootstrapping Encryption

As part of the `Configure` method in the `FunctionsStartup` class register the dependencies necessary to enable encryption

Register how to resolve the encryption options/configuration. The key is expected to come from a key management solution such as Azure Key Vault.

```csharp
builder.Services.AddSingleton(sp => new EncryptionOptions(config["ENCRYPTIONKey"]));
```

Register the implementation of `IEncryptionService` to be used

```csharp
builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
```

Register customization to `IMessageSerializerSettingsFactory` to customize how durable functions serializes the state

```csharp
//necessary to engage custom settings during serialization
builder.Services.AddSingleton<IContractResolver, EncryptedContractResolver>();
builder.Services.AddSingleton(sp => new JsonSerializerSettings { ContractResolver = sp.GetService<IContractResolver>() });
builder.Services.AddSingleton<IMessageSerializerSettingsFactory, SerializerSettingsFactory>();
```

Register customization to Json.NET's default serialization settings to customize how durable functions deserializes the state

```csharp
//necessary to engage custom settings during deserialization
JsonConvert.DefaultSettings = () => builder.Services.BuildServiceProvider().GetService<JsonSerializerSettings>();
```

### Annotate Properties for Encryption

To indicate which entity properties should be encrypted, annotate the properties with the `Encrypted` attribute. The `EncryptedContractResolver` will scan for this attribute to determine which properties need to be encrypted/decrypted.

```csharp
[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
public class AccountEntity : IAccountEntity
{
    // ensure account number is encrypted when saved
    [Encrypted]
    public string AccountNumber { get; set; }

    public void Set(string accountNumber) => AccountNumber = accountNumber;

    [FunctionName(nameof(AccountEntity))]
    public static Task Run([EntityTrigger] IDurableEntityContext ctx) => ctx.DispatchAsync<AccountEntity>();
}
```

### Testing The Encryption

To test the encryption/decryption in this sample, the following two endpoints can be used.

#### PUT /accounts/{accountId}

This will set the account number to a value for the account of the provided `accountId`.

The value when saved to Azure Table Storage should be encrypted. The following is from the Hub Instances table; Input column for the row that represents the Durable Entity.

```json
{"exists":true,"state":"{\"AccountNumber\":\"A1k9ufGmHFMgM83sE/8EdCMls/TLeXbWDEU32ZXrhE4=\"}","sorter":{}}
```

#### GET /accounts/{accountId}

This will retrieve the account entity for the account of the provided `accountId`.

The returned value should be decrypted

```json
{ "accountNumber": "123ABC" }
```

### Customize the Encryption

To customize the specific encryption algorithm used, a new implementation of `IEncryptionService` can be created. This will not change what properties are selected for encryption. It will only change how the properties annotated are encrypted & decrypted. If for example, something other than `AesManaged` is needed, a new implementation of IEncryptionService could be created to use a different cryptography implementation.

```csharp
public interface IEncryptionService
{
    string Decrypt(string value);
    string Encrypt(string value);
}

public class MyEncryptionService
{
    public string Encrypt(string value)
    {
        //todo: return encrypted value
    }

    public string Decrypt(string value)
    {
        //todo: return the decrypted value
    }
}
```

Then, register the new implementation with the IServiceProvider

```csharp
builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
```
