using System.Text.Json.Serialization;

public class SensorData
{
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }
}