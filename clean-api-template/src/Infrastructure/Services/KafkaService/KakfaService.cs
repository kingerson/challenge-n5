namespace MsClean.Infrastructure;
using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

public class KakfaService : IKakfaService
{
    private readonly IProducer<string, string> _producer;
    private readonly IConfiguration _configuration;

    public KakfaService(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        var bootstrapServers = _configuration["KafkaConfig:BootstrapServers"];

        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task<bool> ProduceAsync(string topic, string message)
    {
        var msg = new Message<string, string> { Key = Guid.NewGuid().ToString(), Value = message };
        await _producer.ProduceAsync(topic, msg);
        return true;
    }
}
