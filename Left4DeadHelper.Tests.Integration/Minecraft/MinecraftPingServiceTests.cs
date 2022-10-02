using FluentAssertions;
using Left4DeadHelper.Minecraft;
using Left4DeadHelper.Models.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;

namespace Left4DeadHelper.Tests.Integration.Minecraft;

[TestFixture]
[Explicit("Run manually")]
public class MinecraftPingServiceTests
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
    public async Task TestPingAsync()
    {
        // Arrange
        var pinger = new MinecraftPingService(
            new NullLogger<MinecraftPingService>(),
            new CanPingProvider());
        var serverEntry = _settings.Minecraft!.Servers[0];

        // Act
        var response = await pinger.PingAsync(serverEntry.Hostname, serverEntry.Port);

        // Assert
        response.Should().NotBeNull();
    }
}
