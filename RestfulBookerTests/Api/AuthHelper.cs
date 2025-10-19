using RestSharp;

namespace RestfulBookerTests.Api;

public static class AuthHelper
{
    private static string? _token;

    public static async Task<string> GetTokenAsync()
    {
        if (!string.IsNullOrEmpty(_token))
        {
            return _token;
        }
        var client = new RestClient(ConfigurationHelper.Config["ApiSettings:BaseUrl"]!);
        var request = new RestRequest("auth", Method.Post);
        request.AddJsonBody(new
        {
            username = ConfigurationHelper.Config["ApiSettings:AuthUsername"],
            password = ConfigurationHelper.Config["ApiSettings:AuthPassword"]
        });
        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful)
        {
            throw new ApplicationException($"Auth failed : {response.StatusCode} - {response.Content}");
          
        }
        // Parse JSON manually
        var json = System.Text.Json.JsonDocument.Parse(response.Content!);
        _token = json.RootElement.GetProperty("token").GetString()!;
        return _token;
    }
}
