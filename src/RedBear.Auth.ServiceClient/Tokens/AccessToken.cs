using System;

namespace RedBear.Auth.ServiceClient.Tokens
{
    /// <summary>
    /// Stores an access token and its expiry date.
    /// </summary>
    public class AccessToken
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}
