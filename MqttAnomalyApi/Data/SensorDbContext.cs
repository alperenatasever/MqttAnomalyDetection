using Microsoft.EntityFrameworkCore;
using MqttAnomalyApi.Models;

namespace MqttAnomalyApi.Data
{
    public class SensorDbContext : DbContext
    {
        public SensorDbContext(DbContextOptions<SensorDbContext> options) : base(options) { }

        public DbSet<SensorLog> SensorLogs { get; set; }
    }
}