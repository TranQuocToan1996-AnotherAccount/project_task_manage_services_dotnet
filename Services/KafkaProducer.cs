using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text.Json;
using TaskManagement.Configurations;
using TaskManagement.DTO.Kafka;
using TaskManagement.Services.Interfaces;

namespace TaskManagement.Services;

public class KafkaProducer : IKafkaProducer, IDisposable
{
    private readonly IProducer<Null, string> _producer;
    private readonly KafkaOptions _kafkaOptions;
    private readonly Serilog.ILogger _logger = Log.ForContext<KafkaProducer>();

    public KafkaProducer(IOptions<KafkaOptions> kafkaOptions)
    {
        _kafkaOptions = kafkaOptions.Value;

        var config = new ProducerConfig
        {
            BootstrapServers = _kafkaOptions.BootstrapServers,
            Acks = Acks.All,
            RetryBackoffMs = 100,
            MessageSendMaxRetries = _kafkaOptions.ProducerRetries,
            EnableIdempotence = false
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task PublishLoginEventAsync(LoginEvent loginEvent)
    {
        try
        {
            var messageJson = JsonSerializer.Serialize(loginEvent);
            var message = new Message<Null, string>
            {
                Value = messageJson
            };

            await _producer.ProduceAsync(_kafkaOptions.Topic, message);
            _logger.Information("Successfully published login event for user {UserId} to Kafka topic {Topic}", 
                loginEvent.UserId, _kafkaOptions.Topic);
        }
        catch (ProduceException<Null, string> ex)
        {
            _logger.Error(ex, "Failed to publish login event for user {UserId} to Kafka. Error: {Error}", 
                loginEvent.UserId, ex.Error.Reason);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error publishing login event for user {UserId}", loginEvent.UserId);
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}
