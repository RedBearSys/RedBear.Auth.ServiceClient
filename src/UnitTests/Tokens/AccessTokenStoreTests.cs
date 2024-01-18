using FakeItEasy;
using RedBear.Auth.ServiceClient;
using RedBear.Auth.ServiceClient.Tokens;
using System;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Tokens
{
    public class AccessTokenStoreTests
    {
        [Fact]
        public async void ReceivesNewTokenUponExpiry()
        {
            var count = 0;

            var client = A.Fake<IOAuth2Client>();

            A.CallTo(() => client.GetAccessTokenAsync())
                .ReturnsLazily(() =>
                {
                    count += 1;
                    return Task.FromResult(new AccessToken
                    {
                        Expires = DateTime.UtcNow.AddSeconds(5),
                        Token = $"myaccesstoken{count}"
                    });
                });
            
            var store = new AccessTokenStore(client);

            var token = await store.RetrieveAccessTokenAsync();
            Assert.Equal("myaccesstoken1", token);

            // Access Token should have expired within this timeframe.
            await Task.Delay(7000);

            // This will force the store to get a new access token from the auth
            // server.
            token = await store.RetrieveAccessTokenAsync();

            Assert.Equal("myaccesstoken2", token);
        }

        [Fact]
        public async void ReceivesNewTokenUponExpiryWithConcurrency()
        {
            var count = 0;

            var client = A.Fake<IOAuth2Client>();

            A.CallTo(() => client.GetAccessTokenAsync())
                .ReturnsLazily(() =>
                {
                    count += 1;
                    return Task.FromResult(new AccessToken
                    {
                        // Expire in 5 seconds on the first request, 20 minutes on the second.
                        Expires = count == 1 ? DateTime.UtcNow.AddSeconds(5) : DateTime.UtcNow.AddMinutes(20),
                        Token = $"myaccesstoken{count}"
                    });
                });
            
            var store = new AccessTokenStore(client);

            var token = await store.RetrieveAccessTokenAsync();
            Assert.Equal("myaccesstoken1", token);

            // Access Token should have expired within this timeframe.
            await Task.Delay(7000);

            // This will force the store to get a new access token from the auth
            // server.
            var task1 = store.RetrieveAccessTokenAsync();
            var task2 = store.RetrieveAccessTokenAsync();

            token = await task1;
            Assert.Equal("myaccesstoken2", token);

            token = await task2;
            Assert.Equal("myaccesstoken2", token);
        }

        [Fact]
        public async void ManuallyUpdateToken()
        {
            var client = A.Fake<IOAuth2Client>();

            A.CallTo(() => client.GetAccessTokenAsync())
                .ReturnsLazily(() => Task.FromResult(new AccessToken
                {
                    Expires = DateTime.UtcNow.AddMinutes(20),
                    Token = "myaccesstoken"
                }));

            var store = new AccessTokenStore(client);

            await store.RetrieveAccessTokenAsync();

            await store.StoreTokenAsync(new AccessToken
            {
                Expires = DateTime.UtcNow.AddMinutes(20),
                Token = "newtoken"
            });

            // Simply asserting that we haven't got an exception
            // when cancelling the expiry task within the AccessTokenStore.
            // We got here, so that's success.
        }
    }
}
