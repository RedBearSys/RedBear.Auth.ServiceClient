using System;

namespace RedBear.Auth.ServiceClient
{
    /// <summary>
    /// Configuration values for use with OAuth2Client.
    /// </summary>
    public class OAuth2Params
    {
        /// <summary>
        /// Gets or sets the client identifier issued to you by Red Bear.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the audience provided to you by Red Bear.
        /// </summary>
        /// <value>
        /// The audience. Looks something like https://auth.supplier.redbearapp.io/ .
        /// </value>
        public string Audience { get; set; }

        /// <summary>
        /// Gets or sets the subject email address. This is optional; you would use this
        /// if you want to impersonate a particular user instead of acting as a system.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the path to the .pem private key provided to you by Red Bear.
        /// This should be stored securely - anybody who has access to it will be able
        /// to access Red Bear's API servers.
        /// </summary>
        /// <value>
        /// The certificate file path.
        /// </value>
        public string CertificateFilePath { get; set; }

        /// <summary>
        /// Gets or sets the .pem private key provided to you by Red Bear.
        /// This should be stored securely - anybody who has access to it will be able
        /// to access Red Bear's API servers.
        /// </summary>
        /// <value>
        /// The certificate file path.
        /// </value>
        public string Certificate { get; set; }

        /// <summary>
        /// Gets or sets the list of scopes. See the developer documentation for a full
        /// list of scopes on https://docs.redbear.co.uk/.
        /// </summary>
        /// <value>
        /// The scopes.
        /// </value>
        public string[] Scopes { get; set; } = new string[0];
        
        /// <summary>
        /// Gets or sets the authentication server URI. This will be something like
        /// https://auth.supplier.redbearapp.io/conect/token
        /// </summary>
        /// <value>
        /// The authentication server URI.
        /// </value>
        public Uri AuthServerUri { get; set; }

        public string ScopesAsString()
        {
            return string.Join(" ", Scopes).Trim();
        }
    }
}
