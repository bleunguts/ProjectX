using System.ComponentModel.DataAnnotations;
using ShellX.Services;

namespace ShellX.Tests.Services;

public class GatewayApiClientOptionsTests
{
    private static List<ValidationResult> Validate(GatewayApiClientOptions options)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(options, new ValidationContext(options), results, validateAllProperties: true);
        return results;
    }

    [Test]
    public void Valid_urls_pass_validation()
    {
        var options = new GatewayApiClientOptions { BaseUrl = "https://localhost:8081", SignalRUrl = "https://localhost:8081/streamHub" };

        Assert.That(Validate(options), Is.Empty);
    }

    [Test]
    public void Missing_base_url_fails_validation()
    {
        var options = new GatewayApiClientOptions { BaseUrl = "", SignalRUrl = "https://localhost:8081/streamHub" };

        Assert.That(Validate(options).SelectMany(r => r.MemberNames), Does.Contain(nameof(GatewayApiClientOptions.BaseUrl)));
    }

    [Test]
    public void Malformed_signalr_url_fails_validation()
    {
        var options = new GatewayApiClientOptions { BaseUrl = "https://localhost:8081", SignalRUrl = "not a url" };

        Assert.That(Validate(options).SelectMany(r => r.MemberNames), Does.Contain(nameof(GatewayApiClientOptions.SignalRUrl)));
    }
}
