namespace TaskManagement.DTO.Kafka;

public class LoginEvent
{
    public Guid UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public string IpAddress { get; set; } = string.Empty;
}
