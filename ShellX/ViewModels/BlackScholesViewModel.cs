using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using ProjectX.Core;
using ProjectX.Core.Requests;
using ShellX.OptionsPricing;
using ShellX.Services;
using ShellX.Tables;

namespace ShellX.ViewModels;

public partial class BlackScholesViewModel : ObservableObject
{
    private static readonly TableData EmptyResults = new OptionResultsTableBuilder().Build();

    private readonly IGatewayApiClient _gatewayApiClient;
    private readonly StatusService _status;
    private readonly IDialogService _dialogs;
    private CancellationTokenSource _cts = new();
    private IReadOnlyList<MaturityAndOptionGreeksResultPair> _results = [];

    public BlackScholesViewModel(IGatewayApiClient gatewayApiClient, StatusService status, IDialogService dialogs)
    {
        _gatewayApiClient = gatewayApiClient;
        _status = status;
        _dialogs = dialogs;
        CalculatorTypeDesc = CalculatorCycle.Describe(CalculatorType);
        MarkSelectedCalculator();
    }

    public string[] OptionTypes { get; } = ["Call", "Put"];

    public IReadOnlyList<ChoiceChip> CalculatorChips { get; } =
    [
        new("C#", nameof(OptionsPricingCalculatorType.OptionsPricer)),
        new("C++", nameof(OptionsPricingCalculatorType.OptionsPricerCpp)),
        new("Monte Carlo", nameof(OptionsPricingCalculatorType.MonteCarloCppPricer)),
    ];

    public IReadOnlyList<ChoiceChip> PlotChips { get; } =
    [
        new("Price", "Price"), new("Delta", "Delta"), new("Gamma", "Gamma"), new("Theta", "Theta"),
        new("Rho", "Rho"), new("Vega", "Vega"), new("Experimental", "Experimental"),
    ];

    [ObservableProperty] private string _optionType = "Call";
    [ObservableProperty] private string _spot = "100";
    [ObservableProperty] private string _strike = "100";
    [ObservableProperty] private string _rate = "0.1";
    [ObservableProperty] private string _carry = "0.04";
    [ObservableProperty] private string _vol = "0.3";

    [ObservableProperty] private TableData _optionTable = EmptyResults;
    [ObservableProperty] private OptionsPricingCalculatorType _calculatorType = OptionsPricingCalculatorType.OptionsPricer;
    [ObservableProperty] private string _calculatorTypeDesc = string.Empty;

    [ObservableProperty] private bool _isCompareVisible;
    [ObservableProperty] private TableData _optionTable2 = EmptyResults;
    [ObservableProperty] private OptionsPricingCalculatorType _calculatorType2 = OptionsPricingCalculatorType.OptionsPricerCpp;
    [ObservableProperty] private string _calculatorTypeDesc2 = "Default c++ pricer";

    [ObservableProperty] private HeatMapData? _surface;
    [ObservableProperty] private string _zLabel = string.Empty;

    // The selected maturity (a row of the results) drives the hero price and the Greek tiles.
    [ObservableProperty] private string _heroBig = "--";
    [ObservableProperty] private string _heroSmall = string.Empty;
    [ObservableProperty] private string _heroMaturity = "no results yet";
    [ObservableProperty] private bool _hasResults;
    [ObservableProperty] private double _delta;
    [ObservableProperty] private double _gamma;
    [ObservableProperty] private double _theta;
    [ObservableProperty] private double _rho;
    [ObservableProperty] private double _vega;

    /// <summary>Changes every time new results arrive, so the page can pulse the hero card.</summary>
    [ObservableProperty] private int _resultsVersion;

    private OptionInputs Inputs => new(OptionType, Spot, Strike, Rate, Carry, Vol);

    public async Task ActivateAsync()
    {
        _cts = new CancellationTokenSource();
        try
        {
            _status.Report($"Connecting to backend... {_gatewayApiClient}");
            await _gatewayApiClient.StartHubAsync();
        }
        catch (HttpRequestException)
        {
            await _dialogs.AlertAsync("You made it this far...", "Your connection string to the backend is incorrect.  Hint: it is unset by default.");
        }
        catch (Exception ex)
        {
            // Timeouts, TLS failures etc. must not escape the async void OnAppearing and take the app down.
            _status.Report($"Backend unavailable: {ex.Message}");
        }

        try
        {
            _gatewayApiClient.HubConnection.On("PricingResults", (Action<OptionsPricingByMaturityResults>)OnPricingResults);
            _gatewayApiClient.HubConnection.On<PlotOptionsPricingResult>("PlotResults", OnPlotResults);
        }
        catch (Exception ex)
        {
            await _dialogs.AlertAsync("Press ok to continue.", $"Disabling backend responses as we cannot stream from SignalR Hub, Reason: '{ex.Message}'");
        }
    }

    public async Task DeactivateAsync()
    {
        _cts.Cancel();
        await _gatewayApiClient.StopHubAsync();
    }

    private void OnPricingResults(OptionsPricingByMaturityResults result)
    {
        _status.Report($"Pricing Result with {result.ResultsCount} results received. Backed completed operation in {result.AuditTrail.ElapsedMilliseconds} ms using calculator {result.AuditTrail.CalculatorType}");

        var builder = new OptionResultsTableBuilder();
        builder.SetRows(result.Results);
        var table = builder.Build();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (result.AuditTrail.CalculatorType == CalculatorType)
            {
                OptionTable = table;
                _results = result.Results;
                SelectMaturity(0);
                ResultsVersion++;
            }
            else if (result.AuditTrail.CalculatorType == CalculatorType2)
                OptionTable2 = table;
            else
                _status.Report($"Can't find which option table to update based on {result.AuditTrail.CalculatorType}");
        });
    }

    private void OnPlotResults(PlotOptionsPricingResult result)
    {
        var plot = result.PlotResults;
        _status.Report($"Pricing 3D Plot Result: {plot.PointArray.Length} coordinates completed. Backed completed operation in {result.AuditTrail.ElapsedMilliseconds} ms using calculator {result.AuditTrail.CalculatorType}");

        var data = HeatMapData.FromPlot(plot, result.Request.ZDecimalPlaces, result.Request.ZTickDecimalPlaces);
        MainThread.BeginInvokeOnMainThread(() => ShowSurface(data, result.Request.ZLabel));
    }

    /// <summary>Show the hero price and Greeks for the results row at <paramref name="index"/>.</summary>
    [RelayCommand]
    private void SelectMaturity(int index)
    {
        if (SelectedMaturity.Pick(_results, index) is not { } pair)
        {
            HasResults = false;
            HeroBig = "--";
            HeroSmall = string.Empty;
            HeroMaturity = "no results yet";
            Delta = Gamma = Theta = Rho = Vega = 0;
            return;
        }

        var figure = OptionFigure.Split(pair.OptionGreeks.price);
        HasResults = true;
        HeroBig = figure.Big;
        HeroSmall = figure.Small;
        HeroMaturity = $"t = {pair.Maturity:0.##}";
        Delta = pair.OptionGreeks.delta;
        Gamma = pair.OptionGreeks.gamma;
        Theta = pair.OptionGreeks.theta;
        Rho = pair.OptionGreeks.rho;
        Vega = pair.OptionGreeks.vega;
    }

    [RelayCommand]
    private async Task CalculatePriceAsync()
    {
        OptionTable = EmptyResults;
        _results = [];
        SelectMaturity(0);
        await SubmitPricingAsync(CalculatorType);
    }

    [RelayCommand]
    private async Task CalculatePrice2Async()
    {
        if (CalculatorType2 == CalculatorType)
        {
            await _dialogs.AlertAsync("Compare", "You must set different calculator types for comparison");
            return;
        }
        OptionTable2 = EmptyResults;
        await SubmitPricingAsync(CalculatorType2);
    }

    private async Task SubmitPricingAsync(OptionsPricingCalculatorType calculatorType)
    {
        try
        {
            await _gatewayApiClient.SubmitPricingRequest(Inputs.ToPricingRequest(calculatorType), _cts.Token);
            _status.Report("Price submitted to backend.");
        }
        catch (Exception ex)
        {
            await _dialogs.AlertAsync("Calulate Price issue", $"Failed to send pricing request to backend to price\nReason:'{ex.Message}");
        }
    }

    [RelayCommand]
    private void SelectCalculator(string key)
    {
        if (!Enum.TryParse<OptionsPricingCalculatorType>(key, out var type))
            return;

        CalculatorType = type;
        CalculatorTypeDesc = CalculatorCycle.Describe(type);
        MarkSelectedCalculator();
    }

    private void MarkSelectedCalculator()
    {
        foreach (var chip in CalculatorChips)
            chip.IsSelected = chip.Key == CalculatorType.ToString();
    }

    [RelayCommand]
    private void ToggleCalculator() => (CalculatorType, CalculatorTypeDesc) = CalculatorCycle.Next(CalculatorType);

    [RelayCommand]
    private void ToggleCalculator2() => (CalculatorType2, CalculatorTypeDesc2) = CalculatorCycle.Next(CalculatorType2);

    [RelayCommand]
    private void ToggleCompare() => IsCompareVisible = !IsCompareVisible;

    [RelayCommand]
    private async Task PlotAsync(string greek)
    {
        var (greekType, decimalPlaces, tickDecimalPlaces) = greek switch
        {
            "Price" => (OptionGreeks.Price, 1, 1),
            "Delta" => (OptionGreeks.Delta, 1, 1),
            "Gamma" => (OptionGreeks.Gamma, 2, 3),
            "Theta" => (OptionGreeks.Theta, 0, 0),
            "Rho" => (OptionGreeks.Rho, 0, 0),
            "Vega" => (OptionGreeks.Vega, 0, 0),
            _ => throw new ArgumentException($"Unknown greek '{greek}'", nameof(greek)),
        };

        MarkSelectedPlot(greek);
        try
        {
            var request = Inputs.ToPlotRequest(greekType, greek, decimalPlaces, tickDecimalPlaces, CalculatorType);
            await _gatewayApiClient.SubmitPlotRequest(request, _cts.Token);
            _status.Report("Submitted Plot Price Request to backend.");
        }
        catch (Exception ex)
        {
            await _dialogs.AlertAsync("Calulate Plot parameters issue", $"Failed to send plot request to backend to price\nReason:'{ex.Message}");
        }
    }

    [RelayCommand]
    private void PlotExperimental()
    {
        MarkSelectedPlot("Experimental");
        ShowSurface(HeatMapData.Peak(), string.Empty);
    }

    private void MarkSelectedPlot(string key)
    {
        foreach (var chip in PlotChips)
            chip.IsSelected = chip.Key == key;
    }

    private void ShowSurface(HeatMapData data, string zLabel)
    {
        ZLabel = zLabel;
        Surface = data;
    }
}
