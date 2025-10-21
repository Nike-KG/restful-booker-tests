using RestSharp;

namespace RestfulBookerTests.Api;

public class ApiClient
{
    private readonly RestClient _client;
    public ApiClient()
    {
        var baseUrl = ConfigurationHelper.Config["ApiSettings:BaseUrl"];
        _client = new RestClient(baseUrl!);
    }
    public async Task<RestResponse> GetAsync(string resource)
    {
        
        return await GetAsync(resource, true);
    }

    public async Task<RestResponse> GetAsync(string resource, bool isWithCookieHeader)
    {
        var request = new RestRequest(resource, Method.Get);
        await AddDefaultHeaders(request, isWithCookieHeader);
        return await _client.ExecuteAsync(request);
    }

    public async Task<RestResponse> PostAsync(string resource, object body)
    {
        var request = new RestRequest(resource, Method.Post)
            .AddJsonBody(body);
        await AddDefaultHeaders(request);
        return await _client.ExecuteAsync(request);
    }

    public async Task<RestResponse> PatchAsync(string resource, object body)
    {
       return await PatchAsync(resource, body, true);
    }

    public async Task<RestResponse> PatchAsync(string resource, object body, bool isWithCookieHeader)
    {
        var request = new RestRequest(resource, Method.Patch)
           .AddJsonBody(body);
        await AddDefaultHeaders(request, isWithCookieHeader);
        return await _client.ExecuteAsync(request);
    }

    public async Task<RestResponse> DeleteAsync(string resource)
    {
        return await DeleteAsync(resource, true);
    }

    public async Task<RestResponse> DeleteAsync(string resource, bool isWithCookieHeader)
    {
        var request = new RestRequest(resource, Method.Delete);
        await AddDefaultHeaders(request, isWithCookieHeader);
        return await _client.ExecuteAsync(request);
    }


    private async Task AddDefaultHeaders(RestRequest request, bool isWithCookieHeader = true)
    {
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        if (isWithCookieHeader)
        {
            request.AddHeader("Cookie", $"token={await AuthHelper.GetTokenAsync()}");
        }
        
    }

}
