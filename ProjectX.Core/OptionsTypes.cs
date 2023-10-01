namespace ProjectX.Core;
public enum BarrierType { DownIn, UpIn, DownOut, UpOut }
public enum OptionType { Call, Put };
public enum OptionGreeks
{
    Delta = 0,
    Gamma = 1,
    Theta = 2,
    Rho = 3,
    Vega = 4,
    Price = 5,
};
public record OptionPricerResult(double price, double delta, double gamma, double theta, double rho, double vega);
public class PlotResults
{
    public Point3D[,] PointArray { get; set; }
    public double zmin { get; set; }
    public double zmax { get; set; }
    public double XLimitMin { get; set; }
    public double YLimitMin { get; set; }
    public double XSpacing { get; set; }
    public double YSpacing { get; set; }
    public int XNumber { get; set; }
    public int YNumber { get; set; }
}