using System.Collections.Generic;

namespace SqlMapAPIWrapperLib.Entity
{
    public class SqlmapData
    {
        public bool Success { get; set; }
        public List<string> Error { get; set; } = null;
        public List<SqlmapDataItem> Data { get; set; } = new List<SqlmapDataItem>();
    }
}