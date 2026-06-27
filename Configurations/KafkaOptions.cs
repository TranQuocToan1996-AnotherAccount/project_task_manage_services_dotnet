namespace TaskManagement.Configurations;

public class KafkaOptions
{
    public const string SectionName = "Kafka";
    
    public string BootstrapServers { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string ConsumerGroupId { get; set; } = string.Empty;
    public int ProducerRetries { get; set; } = 3;
    public int MaxRetryAttempts { get; set; } = 5;
}
