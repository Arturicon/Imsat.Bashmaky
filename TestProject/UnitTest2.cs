using DotNet.Testcontainers.Builders;
using Imsat.Bashmaky.Model.Database.Entities;
using Imsat.Bashmaky.Model.Database.Entities.Devices;
using Newtonsoft.Json;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace TestProject
{
    public class UnitTest2
    {
        private HttpClient _client;
        private CustomAppFactory _factory;

        [SetUp]
        public void Setup()
        {
            _client = _factory.CreateClient();
        }

        [OneTimeSetUp]
        public async Task SetupContainer()
        {
            const string postgresPwd = "pgpwd";

            var pgContainer = new ContainerBuilder()
                .WithName(Guid.NewGuid().ToString("N"))
                .WithImage("postgres:16")
                .WithHostname(Guid.NewGuid().ToString("N"))
                .WithExposedPort(5432)
                .WithPortBinding(5432, true)
                .WithEnvironment("POSTGRES_PASSWORD", postgresPwd)
                .WithEnvironment("PGDATA", "/pgdata")
                .WithTmpfsMount("/pgdata")
                .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("psql -U postgres -c \"select 1\""))
                .Build();

            await pgContainer.StartAsync();

            _factory = new CustomAppFactory(pgContainer.Hostname, pgContainer.GetMappedPublicPort(5432), postgresPwd);
        }

        [Test]
        public async Task Test_BoxCompletionByBashmaks()
        {
            await Task.Delay(2000);
            MqttClient client = new MqttClient("localhost");
            client.Connect(Guid.NewGuid().ToString());

            string bs56_to_b14 = "{\"TS\":\"231113212238\",\"IMEI\":\"863051060924940\",\"VDD\":\"1111\", \"ST\":\"12\", \"LAT\":\"17.923130\",\"LON\":\"21.245016\"}"; //C2-E2-82-AD-B5-56
            string bs59_out_b16 = "{\"TS\":\"231113212238\",\"IMEI\":\"863051060924940\",\"VDD\":\"1111\", \"ST\":\"10\", \"LAT\":\"71.923130\",\"LON\":\"62.245016\"}";  //C2-E2-82-AD-B5-59
            string bs57_to_b16 = "{\"TS\":\"231113212238\",\"IMEI\":\"863051060924940\",\"VDD\":\"1111\", \"ST\":\"12\", \"LAT\":\"69.923130\",\"LON\":\"60.245016\"}";  //C2-E2-82-AD-B5-57

            client.Publish("C2-E2-82-AD-B5-56/ST", Encoding.UTF8.GetBytes(bs56_to_b14), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            client.Publish("C2-E2-82-AD-B5-59/ST", Encoding.UTF8.GetBytes(bs59_out_b16), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            client.Publish("C2-E2-82-AD-B5-57/ST", Encoding.UTF8.GetBytes(bs57_to_b16), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);

            await Task.Delay(2000);
            var response = await _client.GetAsync("Entity/Boxes/1");
            var context = await response.Content.ReadAsStringAsync();
            var tmp = JsonConvert.DeserializeObject<IEnumerable<Box>>(context);
            var box14 = tmp.First(x => x.Mac == "45-78-92-AA-33-14");
            var box16 = tmp.First(x => x.Mac == "45-78-92-AA-33-16");

            
            Assert.AreEqual(5, tmp.Count());
            Assert.AreEqual(1, box14.Bashmaks.Count());
            Assert.AreEqual(1, box16.Bashmaks.Count());
            Assert.IsNotNull(box16.Bashmaks.FirstOrDefault().Mac == "C2-E2-82-AD-B5-57");
        }
    }
}
