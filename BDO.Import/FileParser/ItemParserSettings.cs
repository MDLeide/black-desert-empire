namespace BDO.Import.FileParser
{
    public class ItemParserSettings
    {
        public bool HasHeader { get; set; }

        public int NamePosition { get; set; } = 0;
        public int CategoryPosition { get; set; } = 1;
        public int MarketCategoryPosition { get; set; } = 2;
        public int VendorSellsPosition { get; set; } = 3;
        public int VendorCostPosition { get; set; } = 4;
    }
}