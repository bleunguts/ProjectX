using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.AspNetCore.SignalR.Client;
using ProjectX.Core;
using ProjectX.Core.Requests;
using ProjectX.Core.Services;
using ShellX.Fx;
using ShellX.Services;
using ShellX.Tables;
using SkiaSharp;

namespace ShellX.ViewModels;

public partial class FxPricerViewModel : ObservableObject
{
    private const int MaxLogLines = 200;
    private const int LadderTrailLength = 40;

    private static readonly SKColor BidColor = SKColor.Parse("#2563EB");
    private static readonly SKColor AskColor = SKColor.Parse("#D97706");
    private static readonly SKColor AxisColor = SKColor.Parse("#8A8F98");

    private readonly IGatewayApiClient _gatewayApiClient;
    private readonly IFXTradeExecutionService _fxTrader;
    private readonly StatusService _status;
    private readonly IDialogService _dialogs;
    private readonly FxTickBuffer _ticks = new();
    private readonly LinkedList<string> _log = new();
    private readonly LineSeries<double> _bid = new() { Name = "Bid", GeometrySize = 0, Fill = null, LineSmoothness = 0, Values = Array.Empty<double>(), Stroke = new SolidColorPaint(BidColor, 2) };
    private readonly LineSeries<double> _ask = new() { Name = "Ask", GeometrySize = 0, Fill = null, LineSmoothness = 0, Values = Array.Empty<double>(), Stroke = new SolidColorPaint(AskColor, 2) };
    private CancellationTokenSource _cts = new();
    private SpotPrice? _latestSpotPrice;
    private DateTimeOffset _latestTimestamp;
    private DateTimeOffset? _lastTickAt;

    public FxPricerViewModel(IGatewayApiClient gatewayApiClient, IFXTradeExecutionService fxTrader, StatusService status, IDialogService dialogs)
    {
        _gatewayApiClient = gatewayApiClient;
        _fxTrader = fxTrader;
        _status = status;
        _dialogs = dialogs;
        Series = [_bid, _ask];
        YAxes =
        [
            new Axis
            {
                Labeler = value => value.ToString("0.0000"),
                TextSize = 11,
                LabelsPaint = new SolidColorPaint(AxisColor),
                SeparatorsPaint = new SolidColorPaint(AxisColor.WithAlpha(40), 1),
            },
        ];
        PositionsTable = new FxPositionsTableBuilder().Build();
    }

    public string[] Currency { get; } = ["GBPUSD", "EURUSD"];
    public IReadOnlyList<FxQuantityChip> QuantityChips => FxQuantity.Chips;

    public ISeries[] Series { get; }
    public Axis[] XAxes { get; } = [new Axis { IsVisible = false }];
    public Axis[] YAxes { get; }

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

    [ObservableProperty] private FxPriceParts _bidParts = FxPriceFormatter.Split(null);
    [ObservableProperty] private FxPriceParts _askParts = FxPriceFormatter.Split(null);
    [ObservableProperty] private string _sellLabel = FxPriceFormatter.SellLabel(null);
    [ObservableProperty] private string _buyLabel = FxPriceFormatter.BuyLabel(null);
    [ObservableProperty] private bool _hasPrice;
    [ObservableProperty] private string _tickArrow = string.Empty;
    [ObservableProperty] private FxTickDirection _tickDirection;
    [ObservableProperty] private int _tickCount;
    [ObservableProperty] private bool _isLive;
    [ObservableProperty] private double _totalPnl;
    [ObservableProperty] private double _ladderBid;
    [ObservableProperty] private double _ladderAsk;
    [ObservableProperty] private IReadOnlyList<LadderTick> _ladderTrail = [];
    [ObservableProperty] private RectangularSection[] _sections = [];

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

    /// <summary>Called about once a second by the page so the LIVE dot goes grey when ticks stop.</summary>
    public void RefreshLiveness() => IsLive = FxLiveness.IsLive(_lastTickAt, DateTimeOffset.UtcNow);

    private void OnPrice(SpotPrice price, DateTimeOffset timestamp)
    {
        // The Gateway broadcasts every tick to every client; show only the selected pair.
        if (!FxPriceFormatter.IsForPair(price, SelectedCurrency))
            return;

        var direction = FxTickDirections.Between(_latestSpotPrice, price);

        _latestSpotPrice = price;
        _latestTimestamp = timestamp;
        _lastTickAt = DateTimeOffset.UtcNow;
        LatestPrice = FxPriceFormatter.LatestPrice(price);
        LatestPriceTimeStamp = timestamp.ToLocalTime().ToString();
        PriceStream = price.CurrencyPair;
        SpreadInPips = FxPriceFormatter.SpreadText(price);

        BidParts = FxPriceFormatter.Split(price.BidPrice);
        AskParts = FxPriceFormatter.Split(price.AskPrice);
        SellLabel = FxPriceFormatter.SellLabel(price);
        BuyLabel = FxPriceFormatter.BuyLabel(price);
        HasPrice = true;
        IsLive = true;
        TickArrow = FxTickDirections.Arrow(direction);
        TickDirection = direction;
        TickCount++;

        _ticks.Add(new FxTick(timestamp, price));
        RefreshHistory();
        AppendStatus($"New Spot Price arrived at {timestamp.ToLocalTime()} for {price.CurrencyPair}.");
    }

    private void RefreshHistory()
    {
        var oldestFirst = _ticks.OldestFirst.ToList();
        var bids = oldestFirst.Select(t => (double)t.Price.BidPrice).ToArray();
        var asks = oldestFirst.Select(t => (double)t.Price.AskPrice).ToArray();

        Prices = _ticks.NewestFirst.Select(t => FxPriceFormatter.LatestPrice(t.Price)).ToList();
        _bid.Values = bids;
        _ask.Values = asks;

        if (FxChartRange.Padded(bids.Concat(asks)) is { } range)
        {
            YAxes[0].MinLimit = range.Min;
            YAxes[0].MaxLimit = range.Max;
        }
        else
        {
            YAxes[0].MinLimit = null;
            YAxes[0].MaxLimit = null;
        }

        if (oldestFirst.Count > 0)
        {
            var last = oldestFirst[^1].Price;
            LadderBid = (double)last.BidPrice;
            LadderAsk = (double)last.AskPrice;
            var mid = (LadderBid + LadderAsk) / 2;
            Sections = [new RectangularSection { Yi = mid, Yj = mid, Stroke = new SolidColorPaint(AxisColor, 1) }];
        }
        else
        {
            Sections = [];
        }
        LadderTrail = oldestFirst.TakeLast(LadderTrailLength).Select(t => new LadderTick((double)t.Price.BidPrice, (double)t.Price.AskPrice)).ToList();
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
    private void SetQuantity(int quantity) => Notional = quantity.ToString();

    [RelayCommand]
    private void ShowPositions()
    {
        var builder = new FxPositionsTableBuilder();
        builder.SetRows(_fxTrader.PositionsFor(ClientName));
        PositionsTable = builder.Build();
        TotalPnl = (double)builder.Total;
    }

    [RelayCommand]
    private void ClearHistory()
    {
        _ticks.Clear();
        RefreshHistory();
        AppendStatus("Price history cleared.");
    }
}
