// See https://aka.ms/new-console-template for more information

using IdentityModel.Client;

var client = new HttpClient();

var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
    Address = "https://localhost:5001/connect/token",
    ClientId = "m2m.client",
    ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A",
    Scope = "globoapi_fullaccess"
});

Console.WriteLine("Access Token retrived!!!");
Console.WriteLine(response.AccessToken);
Console.WriteLine("Hello, World!");
