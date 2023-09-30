namespace ProjectX.Core.Analytics
{
    public interface IFXSpotPricer
    {
        SpotPrice Price(string ccyPair, SpotPrice spotPrice, int spreadInPips);
    }
}


