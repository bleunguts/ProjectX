namespace ProjectX.Core.Services
{
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
}
