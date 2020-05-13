using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sample.Functions.Encryption
{
    /// <summary>
    /// Sets up transparent encryption for any properties annotated with `Encrypted` attribute
    /// </summary>
    public class EncryptedContractResolver : DefaultContractResolver
    {
        private readonly IEncryptionService _encryption;

        public EncryptedContractResolver(IEncryptionService encryption)
        {
            _encryption = encryption;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var rvalues = base.CreateProperties(type, memberSerialization);

            foreach (JsonProperty prop in rvalues.Where(p => p.PropertyType == typeof(string)))
            {
                PropertyInfo pi = type.GetProperty(prop.UnderlyingName);
                if (pi != null && pi.GetCustomAttribute(typeof(EncryptedAttribute), true) != null)
                {
                    prop.ValueProvider = new EncryptedValueProvider(pi, _encryption);
                }
            }

            return rvalues;
        }
    }
}
