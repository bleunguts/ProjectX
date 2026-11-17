using CommunityToolkit.Mvvm.ComponentModel;

namespace ShellX.ViewModels;

/// <summary>One option in a row of chips (a calculator, a Greek to plot); <see cref="IsSelected"/> highlights it.</summary>
public partial class ChoiceChip : ObservableObject
{
    public ChoiceChip(string label, string key)
    {
        Label = label;
        Key = key;
    }

    public string Label { get; }
    public string Key { get; }

    [ObservableProperty] private bool _isSelected;
}
