using System.Threading.Tasks;

namespace RedBear.Auth.ServiceClient.Tokens
{
    /// <summary>
    /// Interface for a repository that stores access tokens and will automatically retrieve a new
    /// token when one expires. 
    /// </summary>
    public interface IAccessTokenStore
    {
        Task StoreTokenAsync(AccessToken accessToken);
        Task<string> RetrieveAccessTokenAsync();
    }
}