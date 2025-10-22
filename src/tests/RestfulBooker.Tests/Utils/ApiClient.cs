using Polly;
using Polly.Retry;
using RestSharp;
using System.Net;

namespace RestfulBooker.Tests.Utils;

public class ApiClient
{
    private readonly RestClient _client;

    public ApiClient(Action<string>? logAction = null)
    {
        var baseUrl = ConfigurationHelper.Config["ApiSettings:BaseUrl"];
        _client = new RestClient(baseUrl!);
    }

    public async Task<RestResponse<T>> GetAsync<T>(string resource)
    {
        return await GetAsync<T>(resource, true);
    }

    public async Task<RestResponse<T>> GetAsync<T>(string resource, bool isWithCookieHeader)
    {
        var request = new RestRequest(resource, Method.Get);
        await AddDefaultHeaders(request, isWithCookieHeader);

        var retryPolicy = CreateRetryPolicy<T>();
        return await retryPolicy.ExecuteAsync(() => _client.ExecuteAsync<T>(request));
    }

    public async Task<RestResponse<T>> GetByIdAsync<T>(string resource)
    {
        return await GetByIdAsync<T>(resource, true);
    }

    public async Task<RestResponse<T>> GetByIdAsync<T>(string resource, bool isWithCookieHeader)
    {
        var request = new RestRequest(resource, Method.Get);
        await AddDefaultHeaders(request, isWithCookieHeader);

        var retryPolicy = CreateRetryPolicy<T>();

        return await retryPolicy.ExecuteAsync(() => _client.ExecuteAsync<T>(request));
    }

    public async Task<RestResponse<T>> PostAsync<T>(string resource, object body)
    {
        var request = new RestRequest(resource, Method.Post)
            .AddJsonBody(body);
        await AddDefaultHeaders(request);

        var retryPolicy = CreateRetryPolicy<T>();
        return await retryPolicy.ExecuteAsync(() => _client.ExecuteAsync<T>(request));
    }

    public async Task<RestResponse<T>> PatchAsync<T>(string resource, object body)
    {
        return await PatchAsync<T>(resource, body, true);
    }

    public async Task<RestResponse<T>> PatchAsync<T>(string resource, object body, bool isWithCookieHeader)
    {
        var request = new RestRequest(resource, Method.Patch)
           .AddJsonBody(body);
        await AddDefaultHeaders(request, isWithCookieHeader);

        var retryPolicy = CreateRetryPolicy<T>();
        return await retryPolicy.ExecuteAsync(() =>  _client.ExecuteAsync<T>(request));
    }

    public async Task<RestResponse> DeleteAsync(string resource)
    {
        return await DeleteAsync(resource, true);
    }

    public async Task<RestResponse> DeleteAsync(string resource, bool isWithCookieHeader)
    {
        var request = new RestRequest(resource, Method.Delete);
        await AddDefaultHeaders(request, isWithCookieHeader);

        var retryPolicy = CreateRetryPolicyForDelete();
        return await retryPolicy.ExecuteAsync(() => _client.ExecuteAsync(request));
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

    private AsyncRetryPolicy<RestResponse<T>> CreateRetryPolicy<T>()
    {
        return Policy
            .HandleResult<RestResponse<T>>(r =>
                r.StatusCode == HttpStatusCode.RequestTimeout || (int)r.StatusCode >= 500)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (response, timespan, retryCount, context) =>
                {
                    Console.WriteLine($"Retry {retryCount}: {response.Result?.StatusCode}");
                });
    }


    private AsyncRetryPolicy<RestResponse> CreateRetryPolicyForDelete()
    {
        return Policy
            .HandleResult<RestResponse>(r =>
                r.StatusCode == HttpStatusCode.RequestTimeout || (int)r.StatusCode >= 500)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (response, timespan, retryCount, context) =>
                {
                    Console.WriteLine($"Retry {retryCount}: {response.Result?.StatusCode}");
                });
    }
}
