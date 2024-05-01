using QLNet;

namespace ProjectX.AnalyticsLib;

public static class Extensions
{
    public static QLNet.Date ToQuantLibDate(this DateTime dt) => new QLNet.Date((int)dt.ToOADate());
    public static DateTime ToDatetime(this QLNet.Date date) => Convert.ToDateTime(date.month() + " " + date.Day.ToString() + ", " + date.year().ToString());
    public static Period[] ToPeriods(this string[] tenors) => tenors.Select(t => t.ToPeriod()).ToArray();
    public static Period ToPeriod(this string tenor) => new(int.Parse(tenor[..^1]), ToTimeUnits(tenor[^1]));
    private static TimeUnit ToTimeUnits(char timeUnit)
    {
        var u = Char.ToLower(timeUnit);
        switch (u)
        {
            case 'm': return TimeUnit.Months;
            case 'y': return TimeUnit.Years;
            default:
                throw new NotSupportedException($"{timeUnit} conversion not supported");
        }
    }
}
