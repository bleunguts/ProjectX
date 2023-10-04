namespace ProjectX.Core
{
    public interface IFXSpotPricer
    {
        SpotPrice Price(string ccyPair, SpotPrice spotPrice, int spreadInPips);
    }
}


