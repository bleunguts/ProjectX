namespace ProjectX.MachineLearning;

public class ExpectedStockPriceMovement
{
    public DateTime? Date { get; set; }
    public decimal? Open { get; set; }
    public decimal? High { get; set; }
    public decimal? Low { get; set; }
    public decimal? Close { get; set; }
    public int Expected { get; set; }
    public int Predicted { get; set; }    
}