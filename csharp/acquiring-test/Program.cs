using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

const string login = "test1";
const string privateKey = "../private-1.pem";
const string host = "https://gateway.nebank.heggi.dev";

string request = @"{
    ""testString"":""Hello!""
}";

using var ecdsa = ECDsa.Create();
ecdsa.ImportFromPem(File.ReadAllText(privateKey));

var client = new HttpClient {
    BaseAddress = new Uri(host),
};
var signature = ecdsa.SignData(Encoding.UTF8.GetBytes(request), HashAlgorithmName.SHA256, DSASignatureFormat.Rfc3279DerSequence);
var authBytes = Encoding.UTF8.GetBytes($"{login}:{Convert.ToBase64String(signature)}");

var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/acquiring/test");
httpRequest.Content = new StringContent(request, Encoding.UTF8, "application/json");
httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", $"{Convert.ToBase64String(authBytes)}");

var response = await client.SendAsync(httpRequest);

if (!response.IsSuccessStatusCode) {
    Console.WriteLine(response.StatusCode);
}
Console.WriteLine(await response.Content.ReadAsStringAsync());
