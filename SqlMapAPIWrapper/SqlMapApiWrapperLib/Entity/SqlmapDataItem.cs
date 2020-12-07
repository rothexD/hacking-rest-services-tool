namespace SqlMapAPIWrapperLib.Entity
{
    public enum JsonReturnType
    {
        Object,
        Array,
        String
    }
    public class SqlmapDataItem
    {
        public int Status { get; set; }
        public int Type { get; set; }
        public object Value { get; set; }
        public JsonReturnType JsonReturnType { get; set; } = JsonReturnType.Array;
    }
}