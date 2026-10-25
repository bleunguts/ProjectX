using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;

namespace ProjectX.GatewayAPI.Tests
{
    public class OpenApiConfigurationTest
    {
        private string? _originalApiKey;
        private ServiceCollection _services = null!;

        [SetUp]
        public void SetUp()
        {
            // Startup builds the FMP market source eagerly and requires the key to exist
            _originalApiKey = Environment.GetEnvironmentVariable("fmpapikey");
            Environment.SetEnvironmentVariable("fmpapikey", "test-key");

            _services = new ServiceCollection();
            new Startup(new ConfigurationBuilder().Build(), new TestHostEnvironment()).ConfigureServices(_services);
        }

        [TearDown]
        public void TearDown() => Environment.SetEnvironmentVariable("fmpapikey", _originalApiKey);

        [Test]
        public void WhenConfiguringServices_BuiltInOpenApiIsRegistered()
        {
            Assert.That(_services.Any(d => d.ServiceType.Namespace == "Microsoft.AspNetCore.OpenApi" || d.ImplementationType?.Namespace == "Microsoft.AspNetCore.OpenApi"), Is.True);
        }

        [Test]
        public void WhenConfiguringServices_SwashbuckleIsNotRegistered()
        {
            Assert.That(_services.Any(d => d.ServiceType.Namespace?.StartsWith("Swashbuckle") == true), Is.False);
        }

        private class TestHostEnvironment : IHostEnvironment
        {
            public string EnvironmentName { get; set; } = Environments.Production;
            public string ApplicationName { get; set; } = "ProjectX.GatewayAPI.Tests";
            public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
            public IFileProvider ContentRootFileProvider { get; set; } = null!;
        }
    }
}
