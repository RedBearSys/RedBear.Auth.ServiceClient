using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using RedBear.Auth.ServiceClient.Io;
using RedBear.Auth.ServiceClient.Net;

namespace RedBear.Auth.ServiceClient
{
    /// <summary>
    /// Implementation of a client which retrieves access tokens using a
    /// urn:ietf:params:oauth:grant-type:jwt-bearer grant type.
    /// </summary>
    public class OAuth2Client : AbstractOAuth2Client
    {
        private readonly IFileReader _fileReader;
        private readonly OAuth2Params _oauthParams;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2Client"/> class.
        /// </summary>
        /// <param name="httpClient">A singleton instance of an RedBear.Auth.ServiceClient.Http.IHttpClient.</param>
        /// <param name="fileReader">A file reader.</param>
        /// <param name="oauthParams">The oauth parameters.</param>
        public OAuth2Client(IHttpClient httpClient, IFileReader fileReader, OAuth2Params oauthParams) : base(httpClient, oauthParams)
        {
            _fileReader = fileReader;
            _oauthParams = oauthParams;
        }
        
        public override string GenerateSignature(byte[] bytesToSign)
        {
            _fileReader.Open(_oauthParams.CertificateFilePath);
            var pemReader = new PemReader(_fileReader.Reader);
            var keyPair = (AsymmetricCipherKeyPair)pemReader.ReadObject();

            var privateKey = (RsaKeyParameters)keyPair.Private;
            var sig = SignerUtilities.GetSigner("SHA256withRSA");

            sig.Init(true, new RsaKeyParameters(true, privateKey.Modulus, privateKey.Exponent));

            sig.BlockUpdate(bytesToSign, 0, bytesToSign.Length);
            return Base64UrlEncode(sig.GenerateSignature());
        }
    }
}
