using Chart3DControl;
using ProjectX.Core;
using System;
using System.Windows.Media.Media3D;

namespace Shell
{
    public static class ChartFunctions
    {
        public static void Peak3D(DataSeries3D ds)
        {
            double xmin = -3;
            double xmax = 3;
            double ymin = -3;
            double ymax = 3;

            ds.XLimitMin = xmin;
            ds.YLimitMin = ymin;
            ds.XSpacing = 0.2;
            ds.YSpacing = 0.2;
            ds.XNumber = Convert.ToInt16((xmax - xmin) / ds.XSpacing) + 1;
            ds.YNumber = Convert.ToInt16((ymax - ymin) / ds.YSpacing) + 1;

            Point3D[,] pts = new Point3D[ds.XNumber, ds.YNumber];
            for (int i = 0; i < ds.XNumber; i++)
            {
                for (int j = 0; j < ds.YNumber; j++)
                {
                    double x = ds.XLimitMin + i * ds.XSpacing;
                    double y = ds.YLimitMin + j * ds.YSpacing;
                    double z = 3 * Math.Pow(1 - x, 2) * Math.Exp(-x * x - (y + 1) * (y + 1)) - 10 *
                        (0.2 * x - Math.Pow(x, 3) - Math.Pow(y, 5)) * Math.Exp(-x * x - y * y) - 1 / 3 *
                        Math.Exp(-(x + 1) * (x + 1) - y * y);
                    pts[i, j] = new Point3D(x, y, z);
                }
            }
            ds.PointArray = pts;
        }

        public static void Sinc3D(DataSeries3D ds)
        {
            double xmin = -8;
            double xmax = 8;
            double ymin = -8;
            double ymax = 8;

            ds.XLimitMin = xmin;
            ds.YLimitMin = ymin;
            ds.XSpacing = 0.5;
            ds.YSpacing = 0.5;
            ds.XNumber = Convert.ToInt16((xmax - xmin) / ds.XSpacing) + 1;
            ds.YNumber = Convert.ToInt16((ymax - ymin) / ds.YSpacing) + 1;

            Point3D[,] pts = new Point3D[ds.XNumber, ds.YNumber];
            for (int i = 0; i < ds.XNumber; i++)
            {
                for (int j = 0; j < ds.YNumber; j++)
                {
                    double x = ds.XLimitMin + i * ds.XSpacing;
                    double y = ds.YLimitMin + j * ds.YSpacing;
                    double r = Math.Sqrt(x * x + y * y) + 0.000001;
                    double z = Math.Sin(r) / r;
                    pts[i, j] = new Point3D(x, y, z);
                }
            }
            ds.PointArray = pts;
        }
    }
}
