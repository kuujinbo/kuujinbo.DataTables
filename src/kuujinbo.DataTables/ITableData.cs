using System.Collections.Generic;

namespace kuujinbo.DataTables
{
    public interface ITableData
    {
        /// <summary>
        /// 
        /// </summary>
        int Draw { get; set; }

        int RecordsTotal { get; set; }

        int RecordsFiltered { get; set; }

        bool SaveAs { get; set; }

        string[] ColumnNames { get; set; }

        IEnumerable<IColumnData> Columns { get; set; }

        List<List<object>> Data { get; set; }

        void ExecuteRequest<T>(IEnumerable<T> entities) where T : class, IIdentifier;
    }
}