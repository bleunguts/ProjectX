using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.AnalyticsLibNative.Tests
{
    public static class Utils
    {
        public static void AddEnvironmentPaths(IEnumerable<string> paths)
        {
            var path = new[] { Environment.GetEnvironmentVariable("PATH") ?? string.Empty };

            string newPath = string.Join(Path.PathSeparator.ToString(), path.Concat(paths));

            Environment.SetEnvironmentVariable("PATH", newPath);
        }

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        public static void UnloadImportedDll(string DllPath)
        {
            foreach (System.Diagnostics.ProcessModule mod in System.Diagnostics.Process.GetCurrentProcess().Modules)
            {
                var fileInfo = new FileInfo(mod.FileName);
                var file = Path.GetFileNameWithoutExtension(mod.FileName);
                if (mod.FileName == DllPath || file == DllPath)
                {
                    FreeLibrary(mod.BaseAddress);
                }
            }
        }
    }
}
