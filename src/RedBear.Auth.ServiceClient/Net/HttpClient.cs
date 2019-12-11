using System.Net.Http;
using System.Threading.Tasks;

namespace RedBear.Auth.ServiceClient.Net
{
    /// <inheritdoc />
    /// <summary>
    /// Wrapper of System.Net.Http.HttpClient that uses the IHttpClient interface.
    /// </summary>
    /// <seealso cref="T:RedBear.Auth.ServiceClient.Net.IHttpClient" />
    public class HttpClient : IHttpClient
    {
        private readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient();

        public HttpClient()
        {
#if NET46
                  System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;     
#endif
        }

        /// <summary>
        /// Sends the web request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The HTTP response result.</returns>
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return await _httpClient.SendAsync(request);
        }
    }
}
