using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Sample.Functions.Encryption
{
    /// <summary>
    /// Implements transparent encryption during serialization process for given property
    /// </summary>
    public class EncryptedValueProvider : IValueProvider
    {
        private PropertyInfo _targetProperty;
        private IEncryptionService _encryption;

        public EncryptedValueProvider(PropertyInfo targetProperty, IEncryptionService encryption)
        {
            _targetProperty = targetProperty;
            _encryption = encryption;
        }

        /// <summary>
        /// Called by Json.NET prior to serialization
        /// </summary>
        /// <param name="target">The object instance to which the target property belongs</param>
        /// <returns>Encrypted Base64 Encoded string of the target property's original value</returns>
        public object GetValue(object target)
        {
            string decryptedString = (string)_targetProperty.GetValue(target);
            return _encryption.Encrypt(decryptedString);
        }

        /// <summary>
        /// Called by Json.NET prior to deserialization
        /// </summary>
        /// <param name="target">The object instance to which the target property belongs</param>
        /// <param name="value">The target properties immediately deserialized value. Expected to be encrypted and base64 encoded string</param>
        public void SetValue(object target, object value)
        {
            string decryptedString = _encryption.Decrypt((string)value);
            _targetProperty.SetValue(target, decryptedString);
        }
    }
}
