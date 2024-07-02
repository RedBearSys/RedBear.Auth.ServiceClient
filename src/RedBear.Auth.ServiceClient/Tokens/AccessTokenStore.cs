using System;
using System.Threading;
using System.Threading.Tasks;

namespace RedBear.Auth.ServiceClient.Tokens
{
    /// <summary>
    /// A repository that stores access tokens and will automatically retrieve a new
    /// token when one expires. This should be used as a singleton class in your
    /// application.
    /// </summary>
    /// <seealso cref="RedBear.Auth.ServiceClient.Tokens.IAccessTokenStore" />
    public class AccessTokenStore : IAccessTokenStore
    {
        private readonly IOAuth2Client _client;
        private AccessToken _token;
        private readonly SemaphoreSlim _storeSemaphore = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _receiveSemaphore = new SemaphoreSlim(1, 1);
        private Task _expiryTask;
        private CancellationTokenSource _cancellationSource;

        public AccessTokenStore(IOAuth2Client client)
        {
            _client = client;
        }

        /// <summary>
        /// Stores the token in the repository.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns></returns>
        public async Task StoreTokenAsync(AccessToken accessToken)
        {
            // Use a semaphore so that only one instance of this code can be running at a time.
            await _storeSemaphore.WaitAsync();

            try
            {
                if (_expiryTask != null && !_expiryTask.IsCompleted)
                    _cancellationSource.Cancel();

                _token = accessToken;

                _expiryTask = ExpireToken();
            }
            finally
            {
                _storeSemaphore.Release();
            }
        }

        /// <summary>
        /// Async routine that waits until 2 minutes before the token expires. It
        /// then clears the stored access token. The async routine can also be cancelled.
        /// </summary>
        /// <returns></returns>
        private async Task ExpireToken()
        {
            _cancellationSource = new CancellationTokenSource();
            var delay = (int) _token.Expires.AddMinutes(-2).Subtract(DateTime.UtcNow).TotalMilliseconds;

            if (delay < 0)
                delay = 100;

            try
            {
                await Task.Delay(delay, _cancellationSource.Token);
                _token = null;
            }
            catch (TaskCanceledException)
            {
                // Ignored.
            }
        }

        /// <summary>
        /// Retrieves the access token from the repository if one is available.
        /// If a token isn't available, it will try to obtain a new access token.
        /// </summary>
        /// <returns></returns>
        public async Task<AccessToken> RetrieveAccessTokenAsync()
        {
            if (_token != null)
                return _token;

            // Use a semaphore so that only one instance of this code can be running at a time.
            await _receiveSemaphore.WaitAsync();

            try
            {
                // Deliberately repeated - another instance may have now received the token since
                // we were waiting on the semaphore above. If so, nothing to do but return the
                // new value.
                if (_token != null)
                    return _token;

                await StoreTokenAsync(await _client.GetAccessTokenAsync());

                return _token;
            }
            finally
            {
                _receiveSemaphore.Release();
            }

        }
    }
}
