using Confluent.Kafka;

namespace KafkaConsumer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConsumer<string, string> _consumer;
        private readonly string _bootstrapServers = "kafka:9092";

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = "worker-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _consumer = new ConsumerBuilder<string, string>(config).Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe("test-topic");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumerResult = _consumer.Consume(stoppingToken);
                    _logger.LogInformation($"Message received: {consumerResult.Message.Key} - {consumerResult.Message.Value}");
                }
                catch (Exception)
                {
                    _logger.LogInformation("Topic not available");
                }
            }
        }

        public override void Dispose()
        {
            _consumer.Close();
            _consumer.Dispose();
            base.Dispose();
        }
    }
}
