namespace MqttAnomalyApi.Models;
public class SensorLog
{
    public int Id { get; set; }
    public double Temperature { get; set; }
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
}