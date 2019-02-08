using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Salesforce.Common;
using Salesforce.Force;

namespace LogicMine.DataObject.Salesforce
{
    /// <summary>
    /// A factory for IForceClients - https://github.com/developerforce/Force.com-Toolkit-for-NET
    /// </summary>
    public static class ForceClientFactory
    {
        private class CacheableAuthenticationClient
        {
            public AuthenticationClient Client { get; }
            public DateTime? LastAuthenticated { get; set; }

            public CacheableAuthenticationClient(AuthenticationClient client, DateTime? lastAuthenticated = null)
            {
                Client = client ?? throw new ArgumentNullException(nameof(client));
                LastAuthenticated = lastAuthenticated;
            }
        }

        private const string SalesforceAuthEndpoint = "https://login.salesforce.com/services/oauth2/token";
        private const string SalesforceTestAuthEndpoint = "https://test.salesforce.com/services/oauth2/token";
        private static readonly TimeSpan ReauthenticateFrequency = TimeSpan.FromMinutes(2);

        private static readonly SemaphoreSlim AuthSemaphore = new SemaphoreSlim(1, 1);

        private static readonly Dictionary<string, CacheableAuthenticationClient> AuthClients =
            new Dictionary<string, CacheableAuthenticationClient>();

        /// <summary>
        /// Create an IForceClient
        /// </summary>
        /// <param name="clientId">The Salesforce clientId for the app to connect as</param>
        /// <param name="clientSecret">The Salesforce clientSecret for the app to connect as</param>
        /// <param name="username">The Salesforce user to connect as</param>
        /// <param name="password">The password of the Salesforce user to connect as</param>
        /// <param name="isProduction">True if connecting to a production environment, otherwise false for a Sandbox</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static async Task<IForceClient> CreateAsync(string clientId, string clientSecret, string username,
            string password, bool isProduction)
        {
            if (string.IsNullOrWhiteSpace(clientId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(clientId));
            if (string.IsNullOrWhiteSpace(clientSecret))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(clientSecret));
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(username));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));

            var authClient = await GetAuthClientAsync(clientId, clientSecret, username, password, isProduction)
                .ConfigureAwait(false);

            // I would much prefer to use the overload which accepts HttpClients so that new instances 
            // don't have to be created each time however this type blindly disposes of HttpClients even 
            // if they're injected so this isn't safe.
            // It's also not possible to safely cache ForceClients because the access token can only be set 
            // during construction, meaning that once an access token changes a new client is needed.
            // I've raised a ticket on the Github page for this and may submit a PR
            return new ForceClient(authClient.InstanceUrl, authClient.AccessToken, authClient.ApiVersion);
        }

        private static async Task<AuthenticationClient> GetAuthClientAsync(string clientId, string clientSecret,
            string username, string password, bool isProduction)
        {
            var authClientKey = $"{clientId}{clientSecret}{username}{password}";
            var cacheableAuthClient = GetCacheableAuthClient(authClientKey);

            if (!IsAuthRequired(cacheableAuthClient.LastAuthenticated))
                return cacheableAuthClient.Client;

            await AuthSemaphore.WaitAsync();
            try
            {
                // now we're within the critical section check if another thread
                // has authenticated since we last checked
                if (AuthClients.ContainsKey(authClientKey))
                {
                    cacheableAuthClient = AuthClients[authClientKey];
                    if (!IsAuthRequired(cacheableAuthClient.LastAuthenticated))
                        return cacheableAuthClient.Client;
                }

                var authEndpoint = isProduction ? SalesforceAuthEndpoint : SalesforceTestAuthEndpoint;
                var authClient = cacheableAuthClient.Client;

                if (!string.IsNullOrWhiteSpace(authClient.RefreshToken))
                {
                    try
                    {
                        await authClient.TokenRefreshAsync(clientId, authClient.RefreshToken, clientId, authEndpoint)
                            .ConfigureAwait(false);

                        cacheableAuthClient.LastAuthenticated = DateTime.Now;
                        return authClient;
                    }
                    catch (ForceException) // this is correct, not sure why it's not ForceAuthException
                    {
                        // swallow this one so that a full auth attempt is tried next
                    }
                }

                await authClient.UsernamePasswordAsync(clientId, clientSecret, username, password, authEndpoint)
                    .ConfigureAwait(false);

                cacheableAuthClient.LastAuthenticated = DateTime.Now;
                if (!AuthClients.ContainsKey(authClientKey))
                    AuthClients.Add(authClientKey, cacheableAuthClient);

                return authClient;
            }
            finally
            {
                AuthSemaphore.Release();
            }
        }

        private static bool IsAuthRequired(DateTime? lastAuthenticated)
        {
            // should the client still have a valid access token?
            return lastAuthenticated == null || lastAuthenticated.Value.Add(ReauthenticateFrequency) <= DateTime.Now;
        }

        private static CacheableAuthenticationClient GetCacheableAuthClient(string authClientKey)
        {
            if (AuthClients.ContainsKey(authClientKey))
                return AuthClients[authClientKey];

            // there is an overloaded ctor which accepts an HttpClient, I'd prefer to use this however 
            // this type blindly disposes of HttpClients whether they were passed in or not so this isn't safe.
            // Having said this, there will typically only be a single AuthenticationClient per process
            var authClient = new AuthenticationClient();
            return new CacheableAuthenticationClient(authClient);
        }
    }
}