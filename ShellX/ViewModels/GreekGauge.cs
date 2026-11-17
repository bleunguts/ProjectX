using CommunityToolkit.Mvvm.ComponentModel;
using ShellX.OptionsPricing;

namespace ShellX.ViewModels;

/// <summary>One dial on the hero page; the view animates its sweep whenever <see cref="Fraction"/> changes.</summary>
public partial class GreekGauge : ObservableObject
{
    public GreekGauge(string name) => Name = name;

    public string Name { get; }

    [ObservableProperty] private double _fraction = 0.5;
    [ObservableProperty] private string _text = "0.00";

    public void Apply(GaugeReading reading)
    {
        Fraction = reading.Fraction;
        Text = reading.Text;
    }
}
