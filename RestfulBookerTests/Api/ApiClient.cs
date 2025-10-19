using RestSharp;

namespace RestfulBookerTests.Api;

public class ApiClient
{
    private readonly RestClient _client;
    public ApiClient()
    {
        var baseUrl = ConfigurationHelper.Config["ApiSettings:BaseUrl"];
        _client = new RestClient(baseUrl);
    }
    public async Task<RestResponse> GetAsync(string resource) 
        => await _client.ExecuteAsync(new RestRequest (resource, Method.Get));

    public async Task<RestResponse> GetWithTokenAsync(string resource) 
        => await _client.ExecuteAsync(new RestRequest(resource, Method.Get)
        .AddHeader("Cookie", $"token={await AuthHelper.GetTokenAsync()}"));

    public async Task<RestResponse> PostAsync(string resource, object body) 
        => await _client.ExecuteAsync(new RestRequest(resource, Method.Post)
        .AddJsonBody(body));

    public async Task<RestResponse> PatchAsync(string resource, object body) 
        => await _client.ExecuteAsync(new RestRequest(resource, Method.Patch)
        .AddJsonBody(body).AddHeader("Cookie", $"token={await AuthHelper.GetTokenAsync()}"));

    public async Task<RestResponse> DeleteAsync(string resource) 
        => await _client.ExecuteAsync(new RestRequest(resource, Method.Delete)
            .AddHeader("Cookie", $"token={await AuthHelper.GetTokenAsync()}"));


}
