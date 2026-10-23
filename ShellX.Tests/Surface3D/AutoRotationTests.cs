using ShellX.Surface3D;

namespace ShellX.Tests.Surface3D;

public class AutoRotationTests
{
    [Test]
    public void Advances_by_degrees_per_second_times_elapsed_time()
    {
        Assert.That(AutoRotation.Step(0, elapsedSeconds: 0.5, degreesPerSecond: 20), Is.EqualTo(10).Within(1e-9));
    }

    [Test]
    public void Defaults_to_twenty_degrees_a_second()
    {
        Assert.That(AutoRotation.Step(-50, 1), Is.EqualTo(-30).Within(1e-9));
    }

    [Test]
    public void Wraps_past_180_back_to_minus_180()
    {
        Assert.That(AutoRotation.Step(170, 1, 20), Is.EqualTo(-170).Within(1e-9));
    }

    [Test]
    public void A_full_turn_returns_to_the_start()
    {
        Assert.That(AutoRotation.Step(-50, 18, 20), Is.EqualTo(-50).Within(1e-9));
    }

    [Test]
    public void Negative_speed_rotates_the_other_way_and_wraps_below_minus_180()
    {
        Assert.That(AutoRotation.Step(-170, 1, -20), Is.EqualTo(170).Within(1e-9));
    }

    [Test]
    public void Result_always_lies_in_minus_180_to_180()
    {
        for (double start = -720; start <= 720; start += 37)
        {
            var result = AutoRotation.Step(start, 0.033);
            Assert.That(result, Is.InRange(-180d, 180d));
        }
    }
}
