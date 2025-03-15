namespace MsClean.Infrastructure;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

public class HttpService : IHttpService
{
    private readonly IHttpClientFactory _httpClientFactory;
    public HttpService(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

    public async Task<T> GetAsync<T>(string url)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(content);
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest data)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var json = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, json);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<TResponse>(content);
    }

}
