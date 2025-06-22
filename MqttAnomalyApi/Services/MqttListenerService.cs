using MQTTnet;
using MQTTnet.Client;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MqttAnomalyApi.Data;
using MqttAnomalyApi.Models;
using MqttAnomalyApi.Services;

public class MqttListenerService : BackgroundService
{
    private readonly ILogger<MqttListenerService> _logger;
    private readonly IServiceProvider _services;
    
    public MqttListenerService(ILogger<MqttListenerService> logger, IServiceProvider services)
    {
        _logger = logger;
        _services = services;
    }

    public MqttListenerService(ILogger<MqttListenerService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var mqttFactory = new MqttFactory();
        var mqttClient = mqttFactory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost", 1883)
            .Build();

        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

            try
            {
                var data = JsonSerializer.Deserialize<SensorData>(payload);

                
                using var scope = _services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<SensorDbContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

                db.SensorLogs.Add(new SensorLog { Temperature = data.Temperature });
                await db.SaveChangesAsync();

                if (data.Temperature > 50)
                {
                    _logger.LogWarning($"⚠️ Anomali Tespit Edildi: {data.Temperature}");
                    await emailService.SendAnomalyAlert(data.Temperature);
                }
                else
                {
                    _logger.LogInformation($"Normal Temperature: {data.Temperature}");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Parsing Error: {ex.Message}");
            }

            await Task.CompletedTask;
        };


        await mqttClient.ConnectAsync(options, stoppingToken);
        await mqttClient.SubscribeAsync("sensor/temperature", MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);

        _logger.LogInformation("📡 MQTT Listener is active on 'sensor/temperature' topic");
    }
}
