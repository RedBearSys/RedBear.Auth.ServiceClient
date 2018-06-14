using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedBear.Auth.ServiceClient.Exceptions;
using RedBear.Auth.ServiceClient.Net;
using RedBear.Auth.ServiceClient.Tokens;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RedBear.Auth.ServiceClient
{
    /// <summary>
    /// Implementation of a client which retrieves access tokens using a
    /// urn:ietf:params:oauth:grant-type:jwt-bearer grant type.
    /// </summary>
    public abstract class AbstractOAuth2Client : IOAuth2Client
    {
        private readonly IHttpClient _httpClient;
        private readonly OAuth2Params _oauthParams;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2Client"/> class.
        /// </summary>
        /// <param name="httpClient">A singleton instance of an RedBear.Auth.ServiceClient.Http.IHttpClient.</param>
        /// <param name="oauthParams">The oauth parameters.</param>
        protected AbstractOAuth2Client(IHttpClient httpClient, OAuth2Params oauthParams)
        {
            _httpClient = httpClient;
            _oauthParams = oauthParams;
        }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <returns></returns>
        public async Task<AccessToken> GetAccessTokenAsync()
        {
            // Build the JWT token
            var jwt = GetJwt();

            // Build the POST form submission
            var form = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"),
                new KeyValuePair<string, string>("assertion", jwt),
                new KeyValuePair<string, string>("client_id", _oauthParams.ClientId)
            });

            var request = new HttpRequestMessage(HttpMethod.Post, _oauthParams.AuthServerUri) { Content = form };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Send and get the response
            var response = await _httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ServiceClientException($"Unable to obtain an access token. HTTP Status Code: {response.StatusCode}. Response: {body}");
            }

            var content = JObject.Parse(body);

            return new AccessToken
            {
                Token = content["access_token"].Value<string>(),
                Expires = DateTime.UtcNow.AddSeconds(content["expires_in"].Value<int>())
            };
        }

        /// <summary>
        /// Creates a JWT signed with the RS256 algorithm
        /// </summary>
        /// <returns></returns>
        private string GetJwt()
        {
            var header = new
            {
                alg = "RS256",
                typ = "JWT"
            };

            var payload = new
            {
                iss = _oauthParams.ClientId,
                aud = _oauthParams.Audience,
                exp = ToUnixEpoch(DateTime.UtcNow.AddMinutes(1)),
                scope = _oauthParams.ScopesAsString(),
                sub = _oauthParams.Subject
            };

            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var headerEncoded = Base64UrlEncode(JsonConvert.SerializeObject(header, Formatting.None, settings));
            var payloadEncoded = Base64UrlEncode(JsonConvert.SerializeObject(payload, Formatting.None, settings));
            var stringToSign = $"{headerEncoded}.{payloadEncoded}";
            var bytesToSign = Encoding.UTF8.GetBytes(stringToSign);

            var signature = GenerateSignature(bytesToSign);

            return $"{headerEncoded}.{payloadEncoded}.{signature}";
        }

        public abstract string GenerateSignature(byte[] bytesToSign);

        /// <summary>
        /// Converts a date and time to a Unix epoch value - i.e. seconds since 01/01/1970.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        private long ToUnixEpoch(DateTime dateTime)
        {
            var ts = dateTime.Subtract(DateTime.Parse("1970-01-01"));
            return (long)ts.TotalSeconds;
        }

        /// <summary>
        /// Encodes binary for use within a JWT.
        /// </summary>
        /// <param name="content">The content as a string.</param>
        /// <returns>The Base64 URL-encoded value.</returns>
        private string Base64UrlEncode(string content)
        {
            return Base64UrlEncode(Encoding.UTF8.GetBytes(content));
        }

        /// <summary>
        /// Encodes binary for use within a JWT.
        /// </summary>
        /// <param name="input">The content as a byte array.</param>
        /// <returns>The Base64 URL-encoded value.</returns>
        protected string Base64UrlEncode(byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (input.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(input));
            return Convert.ToBase64String(input).Split('=')[0].Replace('+', '-').Replace('/', '_');
        }
    }
}
