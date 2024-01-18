# RedBear.Auth.ServiceClient
OAuth2 service client for .NET Framework 4.6.1 and .NET Standard 2.0. Use it to get access tokens for Red Bear APIs.

## Usage

### Overview

This library makes it easy to obtain an access token to Red Bear APIs for a service application.

Under the hood, the `OAuth2Client` class will create what is known as a [JWT](https://jwt.io/) and sign this with the private key (`.cer` file) that you've been given by Red Bear. Our Auth server will receive the JWT, will validate the signature using the private key's corresponding public key, and will then issue an access token.

The access token must then be included in the `Authorization` header to any API request as follows:

```http
Authorization: Bearer {access_token}
```

### OAuth2Client

The `OAuth2Client` class implements an `IOAuth2Client` interface. 

The implementation class takes an `IHttpClient` and an `IFileReader` in its constructor. The implementations of each are thin wrappers around out-of-the-box .NET libraries (`System.Net.Http.HttpClient` and static `File` methods respectively) but the interface approach allows each to be mocked in a unit test scenario.

Any class that uses the `OAuth2Client` class will require the following `using` statements:

```csharp
using RedBear.Auth.ServiceClient;
using System;
using System.Threading.Tasks;
```

The following demonstrates use of the class:

```csharp
var oauthParams = new OAuth2Params
{
	Audience = "https://auth.rbdebug.redbearapp.io",
	AuthServerUri = new Uri("https://auth.rbdebug.redbearapp.io/connect/token"),
	CertificateFilePath = "MyCertificate.cer",
	ClientId = "MyApp",
	Scopes = new[] { "https://rbdebug.redbearapp.io/UI" }
};

AccessToken token;

using (var fileReader = new FileReader())
{
    var httpClient = new HttpClient();
	var client = new OAuth2Client(httpClient, fileReader, oauthParams);
	token = await client.GetAccessTokenAsync();
}

Console.WriteLine($"Token: {token.Token}");
Console.WriteLine($"Expires: {token.Expires}");
Console.ReadLine();
```

Ensure you use the details provided by Red Bear in the `OAuth2Params` class.

### Managing Access Tokens

Access tokens issued by Red Bear's Auth server are short-lived. When an access token expires, you'll need to obtain a new token to continue accessing APIs.

The `AccessTokenStore` class (which implements `IAccessTokenStore`) can be used to manage the renewal of access tokens, and to ensure that only *one* request is made to our Auth server when a new token is required (as opposed to several threads all sending their own requests).

The `AccessTokenStore` can be used as follows:

```c#
// Use a singleton instance of oauth2Client
var store = new AccessTokenStore(oauth2Client);
string token = await store.RetrieveAccessTokenAsync();
```

Upon expiry, a new token will be obtained automatically when calling `RetrieveAccessTokenAsync()`.

### Dependency Injection

Both `OAuth2Client` and `AccessTokenStore` should be registered as singletons in your dependency container against their interface types (`IOAuth2Client` and `IAccessTokenStore` respectively).
