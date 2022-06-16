namespace Cli.Helpers;

public class AnagramApiClient
{
    private string _anagramsApiUrl;
    private HttpClient _client;

    public AnagramApiClient(string anagramApiUrl)
    {
        var httpClientHandler = new HttpClientHandler();
        httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
        _client = new HttpClient(httpClientHandler);
        _anagramsApiUrl = anagramApiUrl;
    }
    
    public async Task<string> GetAnagramsRequestAsync(string input)
    {
        var response = await _client.GetAsync(_anagramsApiUrl + "?input=" + input);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            return "Request was not successful. Code: " + ex.StatusCode;
        }
        var body = await response.Content.ReadAsStringAsync();
        
        return body;
    }
}