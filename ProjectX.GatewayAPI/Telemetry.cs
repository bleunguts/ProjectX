using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ProjectX.GatewayAPI
{
    public static class Telemetry
    {
        public const string Name = "ProjectX.Pricing";

        public static readonly ActivitySource Source = new(Name);
        private static readonly Meter Meter = new(Name);

        public static readonly Histogram<double> PricingDuration =
            Meter.CreateHistogram<double>("pricing.duration", unit: "ms", description: "Time to price one request");
    }
}
