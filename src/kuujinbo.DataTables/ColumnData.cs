using System;

namespace kuujinbo.DataTables
{
    public interface IColumnData
    {
        /// <summary>
        /// Column data rendered in HTML table cell.
        /// </summary>
        string Data { get; set; }

        /// <summary>
        /// Column name used to lookup the type's value.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Table heading text.
        /// </summary>
        string HeadingText { get; set; }

        /// <summary>
        /// Column visibility
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Column sortability
        /// </summary>
        bool Sortable { get; set; }

        /// <summary>
        /// Column searchability
        /// </summary>
        bool Searchable { get; set; }

        /// <summary>
        /// Column search properties.
        /// <see cref="kuujinbo.DataTables.Search" />.
        /// </summary>
        Search Search { get; set; }

        /// <summary>
        /// Table column width; HTML rendered as CSS percent (%) value.
        /// <see cref="kuujinbo.DataTables.TableWriter" />.
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// Used to build table footer filters for bool and enum.
        /// <see cref="kuujinbo.DataTables.TableWriter.GetTfoot" />.
        /// </summary>
        Type Type { get; set; }
    }

    public sealed class ColumnData : IColumnData
    {
        public ColumnData()
        {
            Visible = true;
            Searchable = true;
            Sortable = true;
        }

        public string Data { get; set; }
        public string Name { get; set; }
        public string HeadingText { get; set; }
        public bool Visible { get; set; }
        public bool Sortable { get; set; }
        public bool Searchable { get; set; }
        public Search Search { get; set; }
        public int Width { get; set; }
        public Type Type { get; set; }
    }
}