using ProjectX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.AnalyticsCppLibShim
{
    public static class Extensions
    {
        public static ProjectXAnalyticsCppLib.OptionType ToNativeOptionType(this OptionType optionType) => optionType switch
        {
            OptionType.Call => ProjectXAnalyticsCppLib.OptionType.Call,
            OptionType.Put => ProjectXAnalyticsCppLib.OptionType.Put,
            _ => throw new NotImplementedException($"{nameof(optionType)} not supported"),
        };
    }
}
