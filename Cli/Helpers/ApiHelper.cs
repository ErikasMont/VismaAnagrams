namespace Cli.Helpers;

public class ApiHelper
{
    private const string _getAnagramsURL = "https://localhost:5001/Anagram?input=";
    private HttpClient _client;

    public ApiHelper()
    {
        var httpClientHandler = new HttpClientHandler();
        httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
        _client = new HttpClient(httpClientHandler);
    }
    
    public async Task<string> GetAnagramsRequest(string input)
    {
        var response = await _client.GetAsync(_getAnagramsURL + input);
        var body = await response.Content.ReadAsStringAsync();
        
        return body;
    }
}