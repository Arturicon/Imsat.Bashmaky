using Imsat.Bashmaky.Model.Database.Entities.Devices;
using Imsat.Web.Mqtt;
using System.Text;
using System.Text.RegularExpressions;

namespace Imsat.Bashmaky.Web.Services
{
    public class SaveToDbService : IMqttMessageHandler
    {
        private readonly Func<DomainService> _dbContextFactory;
        private readonly JsModuleBuilder _jsBuilder;
        private readonly ILogger<SaveToDbService> _logger;

        public SaveToDbService(Func<DomainService> dbContextFactory, JsModuleBuilder jsBuilder, ILogger<SaveToDbService> logger)
        {
            _dbContextFactory = dbContextFactory;
            _jsBuilder = jsBuilder;
            _logger = logger;
        }

        async Task IMqttMessageHandler.Accept(MqttServiceConfig? service, byte[] message, string topic)
        {
            try
            {
                var jsonMessage = Encoding.UTF8.GetString(message);
                string modifiedMessage = ModifyMessage(jsonMessage, topic);
                var domain = _dbContextFactory.Invoke();
                switch (service.Type)
                {
                    case "logTerminal":
                        var st = await _jsBuilder.HandleMessageByJsConverter<SignalTerminal>(service, modifiedMessage);
                        await domain.AddSignalTerminalAsync(st);
                        break;
                    case "logBashmak":
                        var sb = await _jsBuilder.HandleMessageByJsConverter<SignalBashmak>(service, modifiedMessage);
                        await domain.AddSignalBashmakAsync(sb);
                        break;
                    case "logBox":
                        var sb2 = await _jsBuilder.HandleMessageByJsConverter<SignalBox>(service, modifiedMessage);
                        await domain.AddSignalBoxAsync(sb2);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Обработка mqtt сообщения провалилась");
            }
        }

        private string ModifyMessage(string message, string topic)
        {
            var modifiedMessage = PutSourceInMessage(message);
            modifiedMessage = PutIDInMessage(modifiedMessage, topic);
            return modifiedMessage;

            string PutSourceInMessage(string message)
            {
                var extra = $", \"Source\":\"{message.Replace("\"", "\\\"")}\"";
                var newMessage = message.Insert(message.Length - 1, extra);
                return newMessage;
            }
            string PutIDInMessage(string message, string topic)
            {
                var id = new Regex("^.+?(?=/)").Match(topic).Value;
                var extra = $", \"ID\":\"{id}\"";
                var newMessage = message.Insert(message.Length - 1, extra);
                return newMessage;
            }
        }
    }
}
