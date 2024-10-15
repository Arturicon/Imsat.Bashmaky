using Jering.Javascript.NodeJS;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Text.Json;
using System.Text;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Imsat.Bashmaky.Model.Settings;
using Imsat.Bashmaky.Model.Database.Entities.Devices;
using Imsat.Web.Mqtt;

namespace TestProject
{
    public class Tests
    {
        MqttClient client;

        [SetUp]
        public void Setup()
        {
            string brokerAddress = "localhost";
            client = new MqttClient(brokerAddress);
            client.Connect(Guid.NewGuid().ToString());
        }



        [Test]
        public async Task Test1()
        {
            string javascriptModule = @"
    module.exports = (callback, x, y) => {
    var result = x + y; 
    callback(null, result); }";
            // Invoke javascript
            int result = await StaticNodeJSService.InvokeFromStringAsync<int>(javascriptModule, args: new object[] { 3, 5 });
            // result == 8
            Assert.AreEqual(8, result);
        }

        [Test]
        public async Task Test2()
        {
            var str = File.ReadAllText("Resources/mqqtPatterns.json");
            MqttSchemeConfig pattern = JsonConvert.DeserializeObject<MqttSchemeConfig>(str);
            string message = "{\"NET\":\"Bee Line GSM\",\"RSSI\":\"-61\",\"VDD\":\"4166\",\"LAT\":\"59.923130\",\"LON\":\"30.245016\"}";

            StringBuilder javascriptModule = new StringBuilder();
            javascriptModule.Append("module.exports = (callback, arg1) => {");
            foreach (MqttPropertyConfig part in pattern.Properties)
            {
                javascriptModule.Append($"{part.Property} = {part.InFunc};");
                if (!string.IsNullOrEmpty(part.ValidateFunc))
                {
                    javascriptModule.Append($"if(!({part.ValidateFunc.Replace("value", part.Property)}))");
                    javascriptModule.Append("{throw new Error('Parameter did not pass validaation');}");
                }               
            }
            javascriptModule.Append("let result={");
            int i = 0;
            foreach (MqttPropertyConfig part in pattern.Properties)
            {
                if (i != 0)
                    javascriptModule.Append(", ");
                javascriptModule.Append($"{part.Property}: {part.Property}");
                
                i++;
            }
            javascriptModule.Append("}; ");
            javascriptModule.Append("callback(null, result); }");

            try
            {
                var jsonRes = await StaticNodeJSService.InvokeFromStringAsync<string>(javascriptModule.ToString(), args: new object[] { message });
                SignalTerminal imei = JsonConvert.DeserializeObject<SignalTerminal>(jsonRes.ToString());
                Assert.AreEqual(59.923130f, imei.LAT);
            }
            catch (Exception ex)
            {

            }
        }
    }
}