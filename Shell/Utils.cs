using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

using TargetPoint3D = System.Windows.Media.Media3D.Point3D;
using ProjectXPoint3D = ProjectX.Core.Point3D;
using System.Windows.Media;
using ProjectX.Core;

namespace Shell
{
    public static class Utils
    {
        public static TargetPoint3D ToMedia3DPoint3D(this ProjectXPoint3D point) => new TargetPoint3D(point.x, point.y, point.z);
        public static TargetPoint3D[,] ToChartablePointArray(this ProjectXPoint3D[,] matrix)
        {
            TargetPoint3D[,] targetPoints = new TargetPoint3D[matrix.GetLength(0), matrix.GetLength(1)];
            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                for (int column = 0; column < matrix.GetLength(1); column++)
                {
                    var current = matrix[row, column];
                    targetPoints[row, column] = current.ToMedia3DPoint3D();    
                }                
            }
            return targetPoints;
        }

        public static OptionType ToOptionType(this string optionTypeString)
        {                  
            switch(optionTypeString.ToLower())
            {
                case "call": return OptionType.Call;
                case "put": return OptionType.Put;
                default: throw new NotSupportedException(nameof(optionTypeString)); 
            }            
        }
    }
}
