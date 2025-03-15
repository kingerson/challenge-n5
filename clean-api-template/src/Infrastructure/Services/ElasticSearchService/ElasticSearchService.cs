namespace MsClean.Infrastructure;
using System;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using MsClean.Domain;

public class ElasticSearchService<T> : IElasticSearchService<T> where T : Entity
{
    private readonly ElasticsearchClient _client;
    private readonly IConfiguration _configuration;
    public ElasticSearchService(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        var settings = new ElasticsearchClientSettings(new Uri(_configuration["ElasticSearchConfig:Url"]))
            .DefaultIndex(_configuration["ElasticSearchConfig:DefaultIndex"]);
        _client = new ElasticsearchClient(settings);
        _configuration = configuration;
    }

    public async Task<bool> IndexAsync(T model)
    {
        var response = await _client.IndexAsync(model, idx => idx
                                        .Index(_configuration["ElasticSearchConfig:DefaultIndex"])
                                        .Id(model.Id).Refresh(Refresh.WaitFor));
        Console.WriteLine($"ElasticSearResponse : {response}");
        return response.IsValidResponse;
    }

    public async Task<bool> IndexBulkAsync(IEnumerable<T> models)
    {
        var response = await _client.BulkAsync(b => b
                                        .Index(_configuration["ElasticSearchConfig:DefaultIndex"])
                                        .UpdateMany(models, (ud, u) => ud.Doc(u).DocAsUpsert(true)));
        Console.WriteLine($"ElasticSearResponse : {response}");
        return response.IsValidResponse;
    }
}
