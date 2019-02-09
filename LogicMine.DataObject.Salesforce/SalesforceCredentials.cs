using System;

namespace LogicMine.DataObject.Salesforce
{
    public class SalesforceCredentials
    {
        public string ClientId { get; }
        public string ClientSecret { get; }
        public string Username { get; }
        public string Password { get; }
        public string AuthEndpoint { get; }

        public SalesforceCredentials(string clientId, string clientSecret, string username, string password,
            string authEndpoint)
        {
            if (string.IsNullOrWhiteSpace(clientId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(clientId));
            if (string.IsNullOrWhiteSpace(clientSecret))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(clientSecret));
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(username));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));
            if (string.IsNullOrWhiteSpace(authEndpoint))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(authEndpoint));

            ClientId = clientId;
            ClientSecret = clientSecret;
            Username = username;
            Password = password;
            AuthEndpoint = authEndpoint;
        }
    }
}