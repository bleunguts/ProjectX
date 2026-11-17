using ShellX.Services;

namespace ShellX.Tests.Services;

public class GatewayAddressTests
{
    [TestCase("https://localhost:8081", "https://10.0.2.2:8081/")]
    [TestCase("https://localhost:8081/streamHub", "https://10.0.2.2:8081/streamHub")]
    [TestCase("http://127.0.0.1:5000/api?x=1", "http://10.0.2.2:5000/api?x=1")]
    [TestCase("https://LOCALHOST/streamHub", "https://10.0.2.2/streamHub")]
    public void Loopback_hosts_become_the_emulator_host_machine(string url, string expected)
    {
        Assert.That(GatewayAddress.ForAndroidEmulator(url), Is.EqualTo(expected));
    }

    [TestCase("https://projectxgatewayapi.example.com/streamHub")]
    [TestCase("https://192.168.1.20:8081/")]
    [TestCase("https://localhost.example.com/")]
    public void Other_hosts_are_left_alone(string url)
    {
        Assert.That(GatewayAddress.ForAndroidEmulator(url), Is.EqualTo(new Uri(url).AbsoluteUri));
    }

    [TestCase("")]
    [TestCase("not a url")]
    public void Values_that_are_not_absolute_urls_are_returned_unchanged(string value)
    {
        Assert.That(GatewayAddress.ForAndroidEmulator(value), Is.EqualTo(value));
    }
}

public class EmulatorHostTests
{
    [TestCase("https://10.0.2.2:8081", true)]
    [TestCase("https://10.0.2.2:8081/streamHub", true)]
    [TestCase("http://10.0.2.2:8080/", true)]
    [TestCase("https://localhost:8081", false)]
    [TestCase("https://10.0.2.20:8081", false)]
    [TestCase("https://projectx.example.com", false)]
    [TestCase("", false)]
    [TestCase("not a url", false)]
    public void Only_the_emulator_host_machine_counts_as_the_emulator_host(string url, bool expected)
    {
        Assert.That(GatewayAddress.IsEmulatorHost(url), Is.EqualTo(expected));
    }
}
