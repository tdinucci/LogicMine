using System;

namespace Sample.LogicMine.Shop.Service
{
    /// <summary>
    /// This class just exists so that we've got a type to use with the dependency injection container
    /// </summary>
    public class DbConnectionString
    {
        public string Value { get; }

        public DbConnectionString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));

            Value = value;
        }
    }
}