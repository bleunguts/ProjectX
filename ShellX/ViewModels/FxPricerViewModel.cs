using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.AspNetCore.SignalR.Client;
using ProjectX.Core;
using ProjectX.Core.Requests;
using ProjectX.Core.Services;
using ShellX.Fx;
using ShellX.Services;
using ShellX.Tables;

namespace ShellX.ViewModels;

public partial class FxPricerViewModel : ObservableObject
{
    private const int MaxLogLines = 200;

    private readonly IGatewayApiClient _gatewayApiClient;
    private readonly IFXTradeExecutionService _fxTrader;
    private readonly StatusService _status;
    private readonly IDialogService _dialogs;
    private readonly FxTickBuffer _ticks = new();
    private readonly LinkedList<string> _log = new();
    private readonly LineSeries<double> _bid = new() { Name = "Bid", GeometrySize = 0, Fill = null, LineSmoothness = 0, Values = Array.Empty<double>() };
    private readonly LineSeries<double> _ask = new() { Name = "Ask", GeometrySize = 0, Fill = null, LineSmoothness = 0, Values = Array.Empty<double>() };
    private CancellationTokenSource _cts = new();
    private SpotPrice? _latestSpotPrice;
    private DateTimeOffset _latestTimestamp;

    public FxPricerViewModel(IGatewayApiClient gatewayApiClient, IFXTradeExecutionService fxTrader, StatusService status, IDialogService dialogs)
    {
        _gatewayApiClient = gatewayApiClient;
        _fxTrader = fxTrader;
        _status = status;
        _dialogs = dialogs;
        Series = [_bid, _ask];
        PositionsTable = new FxPositionsTableBuilder().Build();
    }

    public string[] Currency { get; } = ["GBPUSD", "EURUSD"];

    public ISeries[] Series { get; }
    public Axis[] XAxes { get; } = [new Axis { IsVisible = false }];
    public Axis[] YAxes { get; } = [new Axis { Labeler = value => value.ToString("0.0000") }];

    [ObservableProperty] private string _selectedCurrency = "EURUSD";
    [ObservableProperty] private string _clientName = "ProjectX Trader";
    [ObservableProperty] private string _notional = "1000";
    [ObservableProperty] private string _latestPrice = "N/A";
    [ObservableProperty] private string _latestPriceTimeStamp = "---";
    [ObservableProperty] private string _priceStream = "N/A";
    [ObservableProperty] private string _spreadInPips = string.Empty;
    [ObservableProperty] private string _statusLog = "Ready.";
    [ObservableProperty] private IReadOnlyList<string> _prices = [];
    [ObservableProperty] private TableData _positionsTable;

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
            _status.Report($"Backend unavailable: {ex.Message}");
        }

        try
        {
            _gatewayApiClient.HubConnection.On<SpotPriceResult>("PushFxRate", result =>
                MainThread.BeginInvokeOnMainThread(() => OnPrice(result.SpotPriceResponse.SpotPrice, result.Timestamp)));
            _gatewayApiClient.HubConnection.On<string>("StopFxRate", ccyPair =>
                MainThread.BeginInvokeOnMainThread(() => AppendStatus($"Stream unsubscribed to {ccyPair}.")));
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

    private void OnPrice(SpotPrice price, DateTimeOffset timestamp)
    {
        // The Gateway broadcasts every tick to every client; show only the selected pair.
        if (!FxPriceFormatter.IsForPair(price, SelectedCurrency))
            return;

        _latestSpotPrice = price;
        _latestTimestamp = timestamp;
        LatestPrice = FxPriceFormatter.LatestPrice(price);
        LatestPriceTimeStamp = timestamp.ToLocalTime().ToString();
        PriceStream = price.CurrencyPair;
        SpreadInPips = FxPriceFormatter.SpreadText(price);

        _ticks.Add(new FxTick(timestamp, price));
        RefreshHistory();
        AppendStatus($"New Spot Price arrived at {timestamp.ToLocalTime()} for {price.CurrencyPair}.");
    }

    private void RefreshHistory()
    {
        Prices = _ticks.NewestFirst.Select(t => FxPriceFormatter.LatestPrice(t.Price)).ToList();
        _bid.Values = _ticks.OldestFirst.Select(t => (double)t.Price.BidPrice).ToArray();
        _ask.Values = _ticks.OldestFirst.Select(t => (double)t.Price.AskPrice).ToArray();
    }

    // Newest line first, as in the WPF tail; capped so a long stream does not grow the text without bound.
    private void AppendStatus(string message)
    {
        _log.AddFirst($"[{DateTime.Now.ToLocalTime()}] {message}");
        if (_log.Count > MaxLogLines)
            _log.RemoveLast();
        StatusLog = string.Join(Environment.NewLine, _log);
    }

    [RelayCommand]
    private async Task SubscribeAsync()
    {
        try
        {
            await _gatewayApiClient.SubmitFxRateSubscribeRequest(new SpotPriceRequest(SelectedCurrency, ClientName, SpotPriceSubscriptionMode.Subscribe), _cts.Token);
            AppendStatus($"subscribed to price stream {SelectedCurrency} successfully.");
        }
        catch (Exception ex)
        {
            await _dialogs.AlertAsync("FX Rate issue", $"Failed to send fx subscription request to backend to price\nReason:'{ex.Message}");
        }
    }

    [RelayCommand]
    private async Task UnsubscribeAsync()
    {
        try
        {
            await _gatewayApiClient.SubmitFxRateUnsubscribeRequest(new SpotPriceRequest(SelectedCurrency, ClientName, SpotPriceSubscriptionMode.Unsubscribe), _cts.Token);
        }
        catch (Exception ex)
        {
            await _dialogs.AlertAsync("FX Rate issue", $"Failed to send fx unsubscription request to backend to price\nReason:'{ex.Message}");
        }
    }

    [RelayCommand]
    private Task BuyTradeAsync() => MakeTradeAsync(BuySell.Buy);

    [RelayCommand]
    private Task SellTradeAsync() => MakeTradeAsync(BuySell.Sell);

    private async Task MakeTradeAsync(BuySell buySell)
    {
        if (_latestSpotPrice is not { } price)
            return;

        if (!int.TryParse(Notional, NumberStyles.Integer | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out var quantity) || quantity <= 0)
        {
            await _dialogs.AlertAsync("Quantity", $"'{Notional}' is not a valid quantity.");
            return;
        }

        var response = _fxTrader.ExecuteTrade(new TradeRequest(FXProductType.Spot, price, quantity, buySell, ClientName, _latestTimestamp));
        AppendStatus($"Trade executed @ {response.BuySell} {response.Quantity} x {response.TransactionPrice}/unit totalling=${response.TotalPrice:0.0}");
        ShowPositions();
    }

    [RelayCommand]
    private void ShowPositions()
    {
        var builder = new FxPositionsTableBuilder();
        builder.SetRows(_fxTrader.PositionsFor(ClientName));
        PositionsTable = builder.Build();
    }

    [RelayCommand]
    private void ClearHistory()
    {
        _ticks.Clear();
        RefreshHistory();
        AppendStatus("Price history cleared.");
    }
}
