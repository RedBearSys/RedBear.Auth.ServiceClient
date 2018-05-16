using RedBear.Auth.ServiceClient.Tokens;
using System.Threading.Tasks;

namespace RedBear.Auth.ServiceClient
{
    /// <summary>
    /// Interface for a client which retrieves access tokens using a
    /// urn:ietf:params:oauth:grant-type:jwt-bearer grant type.
    /// </summary>
    public interface IOAuth2Client
    {
        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <returns></returns>
        Task<AccessToken> GetAccessTokenAsync();
    }
}
