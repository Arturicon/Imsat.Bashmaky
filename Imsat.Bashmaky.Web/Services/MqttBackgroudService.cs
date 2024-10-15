using System.Text.Json;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Imsat.Bashmaky.Model;
using Imsat.Web.Mqtt;


namespace Imsat.Bashmaky.Web.Services
{
    class MqttBackgroudService : BackgroundService 
    {
        private readonly MqttServiceManager _mqttManager;

        public MqttBackgroudService(MqttServiceManager mqttService)
        {
            _mqttManager = mqttService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _mqttManager.StartAsync();
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _mqttManager.Stop();
            return base.StopAsync(cancellationToken);
        }
    }
}
