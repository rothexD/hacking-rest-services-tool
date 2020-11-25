namespace SqlMapAPIWrapper.Entity
{
    public class SqlmapDataItem
    {
        public int Status { get; set; }
        public int Type { get; set; }
        public object Value { get; set; }
        public bool IsJObject { get; set; } = false;
    }
}