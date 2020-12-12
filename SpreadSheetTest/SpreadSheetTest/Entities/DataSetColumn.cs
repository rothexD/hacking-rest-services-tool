using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace SpreadSheetTest.Entities
{
    public class DataSetColumn
    {
        public Guid TableRefId { get; set; }

        public TableRef TableRef { get; set; }

        public byte Position { get; set; }

        public string Name { get; set; }

        public ColumnType Type { get; set; }

        public bool Nullable { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ColumnType
    {
        Integer,
        Real,
        Timestamp,
        String
    }
}
