using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ProjectX.GatewayAPI.Tests
{
    public class AzureSignalRConfigurationTest
    {
        private string? _originalApiKey;

        [SetUp]
        public void SetUp()
        {
            // Startup builds the FMP market source eagerly and requires the key to exist
            _originalApiKey = Environment.GetEnvironmentVariable("fmpapikey");
            Environment.SetEnvironmentVariable("fmpapikey", "test-key");
        }

        [TearDown]
        public void TearDown() => Environment.SetEnvironmentVariable("fmpapikey", _originalApiKey);

        [TestCase("true", "Production", true)]
        [TestCase("true", "Development", true)]
        [TestCase("false", "Production", false)]
        [TestCase("false", "Development", false)]
        [TestCase(null, "Production", false)]
        public void WhenConfiguringServices_AzureSignalRIsRegisteredOnlyIfEnabledInConfig(string? enabled, string environmentName, bool expected)
        {
            var settings = new Dictionary<string, string?>
            {
                ["Azure:SignalR:Enabled"] = enabled,
                ["Azure:SignalR:ConnectionString"] = "Endpoint=https://test.service.signalr.net;AccessKey=dGVzdA==;Version=1.0;"
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
            var environment = new FakeHostEnvironment { EnvironmentName = environmentName };
            var services = new ServiceCollection();

            new Startup(configuration, environment).ConfigureServices(services);

            var registered = services.Any(d => d.ServiceType.Namespace?.StartsWith("Microsoft.Azure.SignalR") == true);
            Assert.That(registered, Is.EqualTo(expected));
        }

        private class FakeHostEnvironment : IHostEnvironment
        {
            public string EnvironmentName { get; set; } = string.Empty;
            public string ApplicationName { get; set; } = "ProjectX.GatewayAPI.Tests";
            public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
            public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } = null!;
        }
    }
}
