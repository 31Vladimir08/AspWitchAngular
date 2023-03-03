// discover endpoints from metadata-
using System.Text.Json;

using IdentityModel.Client;

var client = new HttpClient();
var disco = await client.GetDiscoveryDocumentAsync("https://localhost:7122");
if (disco.IsError)
{
    Console.WriteLine(disco.Error);
    return;
}

/*var a = disco.AuthorizeEndpoint;
var t = disco.TokenEndpoint;
// request token
var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
    Address = disco.TokenEndpoint,

    ClientId = "client",
    ClientSecret = "secret",
    Scope = "api1"
});*/
var tokenResponse2 = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
{
    Address = disco.TokenEndpoint,

    ClientId = "client",
    Scope = "identityApi",
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

// call api
var apiClient = new HttpClient();
apiClient.SetBearerToken(tokenResponse2.AccessToken);

var response = await apiClient.GetAsync("https://localhost:7017/WeatherForecast");
if (!response.IsSuccessStatusCode)
{
    Console.WriteLine(response.StatusCode);
}
else
{
    var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
    Console.WriteLine(JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true }));
}