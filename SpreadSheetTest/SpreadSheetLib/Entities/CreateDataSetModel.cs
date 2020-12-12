using System;

namespace SpreadSheetLib.Entities
{
    public class CreateDataSetModel
    {
        /// <summary>
        /// An optional display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The columns that this table consists of.
        /// </summary>
        public ColumnDefinition[] Columns { get; set; }

        public class ColumnDefinition
        {
            public ColumnType Type { get; set; }

            public string Name { get; set; }

            public bool Nullable { get; set; }
        }
    }
}
