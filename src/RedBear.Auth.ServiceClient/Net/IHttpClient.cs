using System.Net.Http;
using System.Threading.Tasks;

namespace RedBear.Auth.ServiceClient.Net
{
    /// <summary>
    /// Interface for an HttpClient to allow easier unit testing and mocking.
    /// </summary>
    public interface IHttpClient
    {
        /// <summary>
        /// Sends the web request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The HTTP response result.</returns>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }
}
