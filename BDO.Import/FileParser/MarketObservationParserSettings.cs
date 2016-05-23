namespace BDO.Import.FileParser
{
    public class MarketObservationParserSettings
    {
        public bool HasHeader { get; set; }

        public int ItemNamePosition { get; set; } = 0;
        public int EntryTimePosition { get; set; } = 1;
        public int PricePosition { get; set; } = 2;
        public int LastSalePricePosition { get; set; } = 3;
        public int HighPosition { get; set; } = 4;
        public int LowPosition { get; set; } = 5;
        public int TotalTradesPosition { get; set; } = 6;
        public int UnitsOnMarketPosition { get; set; } = 7;
        public int MinPricePosition { get; set; } = 8;
        public int MaxPricePosition { get; set; } = 9;
    }
}