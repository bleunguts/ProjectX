using System.Globalization;
using ShellX.Fx;

namespace ShellX.Controls;

/// <summary>A number that counts up (or down) to its new value, green when positive and red when negative.</summary>
public class CountUpLabel : Label
{
    private const string AnimationName = "countup";
    private const uint DurationMs = 700;

    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value), typeof(double), typeof(CountUpLabel), 0d,
        propertyChanged: (bindable, oldValue, newValue) => ((CountUpLabel)bindable).Animate((double)oldValue, (double)newValue));

    public static readonly BindableProperty FormatProperty = BindableProperty.Create(
        nameof(Format), typeof(string), typeof(CountUpLabel), "#,#0",
        propertyChanged: (bindable, _, _) => ((CountUpLabel)bindable).Show(((CountUpLabel)bindable).Value));

    public CountUpLabel() => Show(0);

    public double Value { get => (double)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
    public string Format { get => (string)GetValue(FormatProperty); set => SetValue(FormatProperty, value); }

    private void Animate(double from, double to)
    {
        this.AbortAnimation(AnimationName);
        new Animation(t => Show(CountUp.Value(from, to, t)), 0, 1)
            .Commit(this, AnimationName, length: DurationMs, easing: Easing.Linear, finished: (_, _) => Show(to));
    }

    private void Show(double value)
    {
        Text = value.ToString(Format, CultureInfo.CurrentCulture);
        TextColor = (SignColorConverter.Instance.Convert(Text, typeof(Color), null, CultureInfo.CurrentCulture) as Color)!;
    }
}
