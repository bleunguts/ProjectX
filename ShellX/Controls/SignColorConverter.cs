using System.Globalization;
using ShellX.Fx;

namespace ShellX.Controls;

/// <summary>Green for a positive PnL cell, red for a negative one, the normal text colour otherwise.</summary>
public class SignColorConverter : IValueConverter
{
    public static readonly SignColorConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var dark = Application.Current?.RequestedTheme == AppTheme.Dark;
        return PnlSign.Of(value as string ?? string.Empty) switch
        {
            > 0 => Color.FromArgb("#16A34A"),
            < 0 => Color.FromArgb("#DC2626"),
            _ => dark ? Colors.White : Color.FromArgb("#212121"),
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}
