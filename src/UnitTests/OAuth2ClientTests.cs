using RedBear.Auth.ServiceClient;
using RedBear.Auth.ServiceClient.Exceptions;
using RedBear.Auth.ServiceClient.Io;
using RedBear.Auth.ServiceClient.Net;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using FakeItEasy;
using Xunit;
using HttpClient = RedBear.Auth.ServiceClient.Net.HttpClient;
#pragma warning disable xUnit1013

namespace UnitTests
{
    public class OAuth2ClientTests
    {
        /// <summary>
        /// Uncomment [Fact] to use as an integration test with a real certificate.
        /// The certificate will need to be copied to the test's bin folder.
        /// The easiest way of doing this is to include the certificate within
        /// this test project and set its build action to "Copy Always".
        ///
        /// Remember to take care of your certificates. Anybody who has access to them
        /// can access Red Bear's APIs and your data.
        ///
        /// The certificate included in this project is a placeholder and isn't used
        /// by any Red Bear energy suppliers.
        /// </summary>
        /// 
        //[Fact]
        public async void IntegrationTest()
        {
            var httpClient = new HttpClient();
            var reader = new FileReader();
            var p = new OAuth2Params
            {
                ClientId = "MyTempApp",
                Audience = "https://auth.supplier.redbearapp.io",
                CertificateFilePath = "ServiceApp.cer",
                Subject = "you@your-domain.co.uk",
                Scopes = new[] { "https://auth.supplier.redbearapp.io/UI" },
                AuthServerUri = new Uri("https://auth.supplier.redbearapp.io/connect/token")
            };

            var client = new OAuth2Client(httpClient, reader, p);
            var token = await client.GetAccessTokenAsync();

            Assert.NotEmpty(token.Token);
        }

        [Fact]
        public async void AccessTokenReceivedSuccessfully()
        {
            var httpClient = A.Fake<IHttpClient>();
            A.CallTo(() => httpClient.SendAsync(A<HttpRequestMessage>.Ignored)).Returns(
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{ \"access_token\" : \"token\", \"expires_in\" : 5000 }",
                        Encoding.UTF8, "application/json")
                });

            var reader = new FileReader();

            var p = new OAuth2Params
            {
                ClientId = "MyTempApp",
                Audience = "https://auth.supplier.redbearapp.io",
                CertificateFilePath = "ServiceApp.cer",
                Subject = "you@your-domain.co.uk",
                Scopes = new[] { "https://auth.supplier.redbearapp.io/UI" },
                AuthServerUri = new Uri("https://auth.supplier.redbearapp.io/connect/token")
            };

            var client = new OAuth2Client(httpClient, reader, p);
            var token = await client.GetAccessTokenAsync();

            Assert.Equal("token", token.Token);
            Assert.True(token.Expires > DateTime.UtcNow);
        }

        [Fact]
        public async void AccessTokenReceivedSuccessfullyFromCert()
        {
            var httpClient = A.Fake<IHttpClient>();
            A.CallTo(() => httpClient.SendAsync(A<HttpRequestMessage>.Ignored)).Returns(
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{ \"access_token\" : \"token\", \"expires_in\" : 5000 }",
                        Encoding.UTF8, "application/json")
                });

            var reader = new FileReader();

            var p = new OAuth2Params
            {
                ClientId = "MyTempApp",
                Audience = "https://auth.supplier.redbearapp.io",
                Certificate = File.ReadAllText("ServiceApp.cer"),
                Subject = "you@your-domain.co.uk",
                Scopes = new[] { "https://auth.supplier.redbearapp.io/UI" },
                AuthServerUri = new Uri("https://auth.supplier.redbearapp.io/connect/token")
            };

            var client = new OAuth2Client(httpClient, reader, p);
            var token = await client.GetAccessTokenAsync();

            Assert.Equal("token", token.Token);
            Assert.True(token.Expires > DateTime.UtcNow);
        }

        [Fact]
        public async void BadRequestFails()
        {
            var httpClient = A.Fake<IHttpClient>();
            A.CallTo(() => httpClient.SendAsync(A<HttpRequestMessage>.Ignored)).Returns(
                new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(string.Empty, Encoding.UTF8, "application/json")
                });

            var reader = new FileReader();

            var p = new OAuth2Params
            {
                ClientId = "MyTempApp",
                Audience = "https://auth.supplier.redbearapp.io",
                CertificateFilePath = "ServiceApp.cer",
                Subject = "you@your-domain.co.uk",
                Scopes = new[] { "https://auth.supplier.redbearapp.io/UI" },
                AuthServerUri = new Uri("https://auth.supplier.redbearapp.io/connect/token")
            };

            var client = new OAuth2Client(httpClient, reader, p);

            await Assert.ThrowsAsync<ServiceClientException>(async () =>
            {
                await client.GetAccessTokenAsync();
            });
        }
    }
}
