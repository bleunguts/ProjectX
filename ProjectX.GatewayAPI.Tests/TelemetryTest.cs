using System.Diagnostics.Metrics;

namespace ProjectX.GatewayAPI.Tests
{
    public class TelemetryTest
    {
        [Test]
        public void WhenPricingDurationIsRecorded_MeterListenerSeesItUnderTheProjectXPricingMeter()
        {
            var seen = new List<string>();
            using var listener = new MeterListener();
            listener.InstrumentPublished = (instrument, l) =>
            {
                if (instrument.Meter.Name == Telemetry.Name) l.EnableMeasurementEvents(instrument);
            };
            listener.SetMeasurementEventCallback<double>((instrument, value, tags, state) => seen.Add(instrument.Name));
            listener.Start();

            Telemetry.PricingDuration.Record(12.5);

            Assert.That(seen, Does.Contain("pricing.duration"));
        }
    }
}
