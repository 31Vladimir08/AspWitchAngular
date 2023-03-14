// discover endpoints from metadata-
using IdentityModel.Client;

var client = new HttpClient();
var disco = await client.GetDiscoveryDocumentAsync("https://localhost:7122");
if (disco.IsError)
{
    Console.WriteLine(disco.Error);
    return;
}

// request token
var tokenResponse2 = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
{
    Address = disco.TokenEndpoint,

    ClientId = "GatewaysAPI",
    ClientSecret = "secret",

    UserName = "admin@admin.com",
    Password = "$Admin12345$"
});

if (tokenResponse2.IsError)
{
    Console.WriteLine(tokenResponse2.Error);
    return;
}

Console.WriteLine(tokenResponse2.AccessToken);
var t = GetUserInfo(tokenResponse2.AccessToken);

string GetUserInfo(string token)
{
    var client = new HttpClient();
    client.SetBearerToken(token);
    var response = client.GetAsync("https://localhost:7122/connect/userinfo").GetAwaiter().GetResult();

    var response1 = client.GetUserInfoAsync(new UserInfoRequest
    {
        Address = "https://localhost:7122/connect/userinfo",
        Token = token
    }).GetAwaiter().GetResult();
    return "f";
}