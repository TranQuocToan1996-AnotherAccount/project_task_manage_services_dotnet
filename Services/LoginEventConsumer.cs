using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text.Json;
using TaskManagement.Configurations;
using TaskManagement.DTO.Kafka;
using TaskManagement.Models;
using TaskManagement.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace TaskManagement.Services;

public class LoginEventConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly KafkaOptions _kafkaOptions;
    private readonly ILogger<LoginEventConsumer> _logger;
    private IConsumer<Ignore, string>? _consumer;

    public LoginEventConsumer(IServiceProvider serviceProvider, IOptions<KafkaOptions> kafkaOptions, ILogger<LoginEventConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _kafkaOptions = kafkaOptions.Value;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaOptions.BootstrapServers,
            GroupId = _kafkaOptions.ConsumerGroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        _logger.LogInformation("Creating Kafka consumer with BootstrapServers: {BootstrapServers}, GroupId: {GroupId}", 
            _kafkaOptions.BootstrapServers, _kafkaOptions.ConsumerGroupId);

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        _consumer.Subscribe(_kafkaOptions.Topic);

        _logger.LogInformation("LoginEventConsumer started. Listening to topic: {Topic}", _kafkaOptions.Topic);

        Task.Run(() => ConsumeMessages(stoppingToken), stoppingToken);

        return Task.CompletedTask;
    }

    private async Task ConsumeMessages(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting to consume messages from Kafka...");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Kiểm tra null cho _consumer
                if (_consumer == null) continue;

                _logger.LogInformation("Waiting for message from Kafka...");
                var consumeResult = _consumer.Consume(stoppingToken);
                
                _logger.LogInformation("Received message from Kafka topic: {Topic}, partition: {Partition}, offset: {Offset}", 
                    consumeResult.Topic, consumeResult.Partition, consumeResult.Offset);
                
                await ProcessMessageWithRetry(consumeResult, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("LoginEventConsumer cancellation requested.");
                break;
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Kafka consume error: {Error}", ex.Error.Reason);
                await Task.Delay(5000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in LoginEventConsumer");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }

    private async Task ProcessMessageWithRetry(ConsumeResult<Ignore, string> consumeResult, CancellationToken stoppingToken)
    {
        var retryCount = 0;
        var maxRetries = _kafkaOptions.MaxRetryAttempts;

        while (retryCount < maxRetries)
        {
            try
            {
                var loginEvent = JsonSerializer.Deserialize<LoginEvent>(consumeResult.Message.Value);
                if (loginEvent == null)
                {
                    _logger.LogWarning("Failed to deserialize login event message");
                    _consumer?.Commit(consumeResult);
                    return;
                }

                using var scope = _serviceProvider.CreateScope();
                var loginInfoRepository = scope.ServiceProvider.GetRequiredService<ILoginInfoRepository>();

                var loginInfo = new LoginInfo
                {
                    UserId = loginEvent.UserId,
                    Timestamp = loginEvent.Timestamp,
                    IpAddress = loginEvent.IpAddress
                };

                await loginInfoRepository.CreateAsync(loginInfo);

                _logger.LogInformation("Successfully saved login info for user {UserId} to database", loginEvent.UserId);
                _consumer?.Commit(consumeResult);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LỖI LƯU DB: Chi tiết lỗi: {Message}. Dữ liệu: {Data}", ex.Message, consumeResult.Message.Value);
                retryCount++;
                if (retryCount >= maxRetries)
                {
                    _logger.LogError(ex, "Failed to process login event after {RetryCount} attempts.", retryCount);
                    _consumer?.Commit(consumeResult);
                    return;
                }

                var delay = TimeSpan.FromSeconds(Math.Pow(2, retryCount - 1));
                _logger.LogWarning("Attempt {RetryCount}/{MaxRetries} failed. Retrying in {Delay}s.", 
                    retryCount, maxRetries, delay.TotalSeconds);
                await Task.Delay(delay, stoppingToken);
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("LoginEventConsumer is stopping...");
        _consumer?.Close();
        await base.StopAsync(cancellationToken);
    }
}