using CoreRCON;
using Left4DeadHelper.Models.Configuration;
using Left4DeadHelper.Rcon;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Left4DeadHelper.Tests.Integration
{
    [TestFixture]
    [Explicit("Run manually")]
    public class Left4DeadIntegrationTests
    {
        private Settings _settings;

        [SetUp]
        public void SetUp()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) //From NuGet Package Microsoft.Extensions.Configuration.Json
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .Build();

            _settings = config.Get<Settings>();
        }

        [Test]
        public async Task TestRconPrintInfo()
        {
            var serverInfo = _settings.Left4DeadSettings.ServerInfo;
            using var rcon = new RCON(new IPEndPoint(IPAddress.Parse(serverInfo.Ip), serverInfo.Port), serverInfo.RconPassword);
            await rcon.ConnectAsync();

            var printInfo = await rcon.SendCommandAsync<PrintInfo>("sm_printinfo");
        }

        [Test]
        public async Task TestRconGameMode()
        {
            var serverInfo = _settings.Left4DeadSettings.ServerInfo;
            using var rcon = new RCON(new IPEndPoint(IPAddress.Parse(serverInfo.Ip), serverInfo.Port), serverInfo.RconPassword);
            await rcon.ConnectAsync();

            var gamemode = await rcon.SendCommandAsync<SmCvar>("sm_cvar mp_gamemode");
        }
    }
}
