using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Salesforce.Common;
using Salesforce.Force;

namespace LogicMine.DataObject.Salesforce
{
    /// <summary>
    /// A factory for IForceClients - https://github.com/developerforce/Force.com-Toolkit-for-NET
    /// </summary>
    internal static class ForceClientFactory
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

        private static readonly TimeSpan ReauthenticateFrequency = TimeSpan.FromMinutes(2);
        private static readonly SemaphoreSlim AuthSemaphore = new SemaphoreSlim(1, 1);
        private static readonly object CreateClientLocker = new object();

        private static readonly Dictionary<string, CacheableAuthenticationClient> AuthClients =
            new Dictionary<string, CacheableAuthenticationClient>();

        private static readonly Dictionary<string, KeyValuePair<string, ForceClient>> ForceClients =
            new Dictionary<string, KeyValuePair<string, ForceClient>>();

        public static async Task<IForceClient> CreateAsync(SalesforceConnectionConfig connectionConfig)
        {
            var authClient = await GetAuthClientAsync(connectionConfig)
                .ConfigureAwait(false);

            var connectionKey = GetConnectionKey(connectionConfig);
            if (ForceClients.TryGetValue(connectionKey, out var forceClient) &&
                forceClient.Key == authClient.AccessToken)
            {
                return forceClient.Value;
            }

            lock (CreateClientLocker)
            {
                // another thread may have created client while current thread was blocked
                if (ForceClients.TryGetValue(connectionKey, out forceClient))
                {
                    if (forceClient.Key == authClient.AccessToken)
                        return forceClient.Value;

                    forceClient.Value.Dispose();
                    ForceClients.Remove(connectionKey);
                }

                var client = new ForceClient(authClient.InstanceUrl, authClient.AccessToken, authClient.ApiVersion);
                ForceClients.Add(connectionKey, new KeyValuePair<string, ForceClient>(authClient.AccessToken, client));

                return client;
            }
        }

        private static async Task<AuthenticationClient> GetAuthClientAsync(SalesforceConnectionConfig connectionConfig)
        {
            var authClientKey = GetConnectionKey(connectionConfig);
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

                var authClient = cacheableAuthClient.Client;

                if (!string.IsNullOrWhiteSpace(authClient.RefreshToken))
                {
                    try
                    {
                        await authClient.TokenRefreshAsync(connectionConfig.ClientId, authClient.RefreshToken,
                            connectionConfig.ClientSecret, connectionConfig.AuthEndpoint).ConfigureAwait(false);

                        cacheableAuthClient.LastAuthenticated = DateTime.Now;
                        return authClient;
                    }
                    catch (ForceException) // this is correct, not sure why it's not ForceAuthException
                    {
                        // swallow this one so that a full auth attempt is tried next
                    }
                }

                await authClient.UsernamePasswordAsync(connectionConfig.ClientId, connectionConfig.ClientSecret,
                        connectionConfig.Username, connectionConfig.Password, connectionConfig.AuthEndpoint)
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

        private static string GetConnectionKey(SalesforceConnectionConfig connectionConfig)
        {
            return
                $"{connectionConfig.ClientId}{connectionConfig.ClientSecret}{connectionConfig.Username}{connectionConfig.Password}";
        }
    }
}