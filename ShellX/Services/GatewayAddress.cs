namespace ShellX.Services;

public static class GatewayAddress
{
    /// <summary>The Android emulator's address for the PC it runs on; inside the emulator "localhost" is the emulator itself.</summary>
    public const string EmulatorHostMachine = "10.0.2.2";

    /// <summary>Points a localhost / 127.0.0.1 URL at the host PC so an emulator can reach a Gateway running locally. Anything else is returned as it is.</summary>
    public static string ForAndroidEmulator(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return url;

        return uri.Host is "localhost" or "127.0.0.1"
            ? new UriBuilder(uri) { Host = EmulatorHostMachine }.Uri.AbsoluteUri
            : uri.AbsoluteUri;
    }
}
