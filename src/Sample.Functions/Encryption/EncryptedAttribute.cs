using System;

namespace Sample.Functions.Encryption
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class EncryptedAttribute : Attribute
    {
    }
}
