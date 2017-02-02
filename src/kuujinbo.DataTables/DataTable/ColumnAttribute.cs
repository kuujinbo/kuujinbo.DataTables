using System;

namespace kuujinbo.DataTables.DataTable
{
    [AttributeUsage(
        AttributeTargets.Property | AttributeTargets.Field, 
        AllowMultiple = false, 
        Inherited = true)
    ]
    public sealed class ColumnAttribute : Attribute
    {
        public bool Display { get; set; }
        public string DisplayName { get; set; }
        public int DisplayOrder { get; set; }
        /// <summary>
        /// CSS width in percent (%)
        /// </summary>
        public int DisplayWidth { get; set; }
        public bool IsSearchable { get; set; }
        public bool IsSortable { get; set; }
        public string FieldAccessor { get; set; }

        public ColumnAttribute()
        {
            Display = true;
            IsSearchable = true;
            IsSortable = true;
        }
    }
}