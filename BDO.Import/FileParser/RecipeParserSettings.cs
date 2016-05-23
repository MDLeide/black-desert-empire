namespace BDO.Import.FileParser
{
    public class RecipeParserSettings
    {
        public bool HasHeader { get; set; }

        public int IdPosition { get; set; } = 0;
        public int ResultItemNamePosition { get; set; } = 1;
        public int TypePosition { get; set; } = 2;
        public int SubTypePosition { get; set; } = 3;
        public int ExpectedYieldPosition { get; set; } = 4;

        public int SecondaryResultsParentIdPosition { get; set; } = 0;
        public int SecondaryResultsItemNamePosition { get; set; } = 1;

        public int MaterialsParentIdPosition { get; set; } = 0;
        public int MaterialsItemNamePosition { get; set; } = 1;
        public int MaterialsQuantityPostiion { get; set; } = 2;
    }
}