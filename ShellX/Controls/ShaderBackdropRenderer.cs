using SkiaSharp;

namespace ShellX.Controls;

/// <summary>
/// A flowing colour field drawn by an SKSL runtime shader. The shader is run into a small offscreen bitmap and
/// scaled up (soft gradients lose nothing), which keeps it cheap enough for a CPU-backed canvas. If the shader
/// cannot be compiled the backdrop falls back to a plain dark gradient. No MAUI dependency.
/// </summary>
public sealed class ShaderBackdropRenderer : IDisposable
{
    private const int Downscale = 4;
    private const int MinSize = 64;

    private const string Source = """
        uniform float2 iResolution;
        uniform float iTime;

        half4 main(float2 fragCoord) {
            float2 uv = fragCoord / iResolution;
            float t = iTime * 0.30;
            float2 p = uv * float2(3.2, 2.2);

            float v = sin(p.x + t) + sin(p.y * 1.4 - t * 1.3) + sin((p.x + p.y) * 0.9 + t * 0.7);
            v += sin(length(p - float2(1.6 + sin(t), 1.1 + cos(t * 0.8))) * 2.2 - t);

            float m = 0.5 + 0.5 * sin(v);
            float n = 0.5 + 0.5 * sin(v * 1.7 + 2.0);

            float3 baseColor = float3(0.045, 0.07, 0.16);
            float3 blue = float3(0.12, 0.32, 0.95);
            float3 amber = float3(0.95, 0.50, 0.08);

            float3 col = baseColor + blue * (0.62 * m) + amber * (0.30 * (1.0 - m) * n);

            float2 d = uv - 0.5;
            col *= 1.0 - 0.45 * dot(d, d) * 2.0;
            return half4(half3(col), 1.0);
        }
        """;

    private readonly SKRuntimeEffect? _effect;
    private SKBitmap? _small;

    public ShaderBackdropRenderer()
    {
        _effect = SKRuntimeEffect.CreateShader(Source, out var errors);
        Error = errors;
    }

    /// <summary>The compiler's message when the shader failed, otherwise null or empty.</summary>
    public string? Error { get; }

    public bool UsesShader => _effect is not null;

    public void Draw(SKCanvas canvas, int width, int height, double seconds)
    {
        if (width <= 0 || height <= 0)
            return;

        var smallWidth = Math.Max(width / Downscale, MinSize);
        var smallHeight = Math.Max(height / Downscale, MinSize);
        if (_small is null || _small.Width != smallWidth || _small.Height != smallHeight)
        {
            _small?.Dispose();
            _small = new SKBitmap(smallWidth, smallHeight);
        }

        using (var small = new SKCanvas(_small))
        {
            if (_effect is null)
                DrawFallback(small, smallWidth, smallHeight);
            else
                DrawShader(small, smallWidth, smallHeight, seconds);
        }

        using var image = SKImage.FromBitmap(_small);
        canvas.DrawImage(image, new SKRect(0, 0, width, height), new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.None));
    }

    private void DrawShader(SKCanvas canvas, int width, int height, double seconds)
    {
        var uniforms = new SKRuntimeEffectUniforms(_effect!)
        {
            ["iResolution"] = new[] { (float)width, (float)height },
            ["iTime"] = (float)seconds,
        };

        using var shader = _effect!.ToShader(uniforms);
        using var paint = new SKPaint { Shader = shader };
        canvas.DrawRect(0, 0, width, height, paint);
    }

    private static void DrawFallback(SKCanvas canvas, int width, int height)
    {
        using var shader = SKShader.CreateLinearGradient(
            new SKPoint(0, 0), new SKPoint(width, height),
            [SKColor.Parse("#0B1020"), SKColor.Parse("#16295E")], SKShaderTileMode.Clamp);
        using var paint = new SKPaint { Shader = shader };
        canvas.DrawRect(0, 0, width, height, paint);
    }

    public void Dispose()
    {
        _small?.Dispose();
        _effect?.Dispose();
    }
}
