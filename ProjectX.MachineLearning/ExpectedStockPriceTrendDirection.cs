namespace ProjectX.MachineLearning;
public enum StockPriceTrendDirection { Flat, Upward, Downward, Unset = -1 }

public class ExpectedStockPriceTrendDirection
{
    public DateTime? Date { get; set; }
    public decimal? Open { get; set; }
    public decimal? High { get; set; }
    public decimal? Low { get; set; }
    public decimal? Close { get; set; }
    public StockPriceTrendDirection Expected { get; set; }
    public StockPriceTrendDirection Predicted { get; set; }    
}