namespace BDO.Import.FileParser
{
    public class ParseError
    {
        internal ParseError(int row, int col, string field, string msg)
        {
            
        }

        public int Row { get; internal set; }
        public int Column { get; internal set; }
        public string FieldName { get; internal set; }
        public string Message { get; internal set; }
    }
}