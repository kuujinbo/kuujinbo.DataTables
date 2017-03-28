using System;

namespace kuujinbo.DataTables
{
    public sealed class Column
    {
        public string Data { get; set; }
        public string Name { get; set; }
        public bool Display { get; set; }
        public bool IsSortable { get; set; }
        public bool IsSearchable { get; set; }
        public Search Search { get; set; }
        /// <summary>
        /// only used when building the table header
        /// see ColumnAttribute.cs && TableHtmlWriter.cs
        /// </summary>
        public int DisplayWidth { get; set; }
        /// <summary>
        /// only used when building the table footer filter(s):
        /// see TableHtmlWriter.cs
        /// </summary>
        public Type Type { get; set; }
    }
}