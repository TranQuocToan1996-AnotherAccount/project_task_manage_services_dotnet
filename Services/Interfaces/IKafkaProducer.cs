using TaskManagement.DTO.Kafka;

namespace TaskManagement.Services.Interfaces;

public interface IKafkaProducer
{
    Task PublishLoginEventAsync(LoginEvent loginEvent);
}
