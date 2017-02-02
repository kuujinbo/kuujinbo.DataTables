using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace kuujinbo.DataTables.DataTable.Export
{
    public class Csv : IExport
    {
        private TextWriter _writer;
        private int _columnCount;

        public const string DOUBLE_QUOTE_ESCAPE = @"""""";
        public const string DOUBLE_QUOTE = @"""";
        // ---------------------------------------------------------------------------
        // make sure individual fields are properly quoted:
        // http://www.rfc-editor.org/rfc/rfc4180.txt
        public string GetCSV(string s)
        {
            return DOUBLE_QUOTE
              + s.Replace(@"""", DOUBLE_QUOTE_ESCAPE)
              + DOUBLE_QUOTE
              ;
        }

        public byte[] Export(object data)
        {
            using (var stream = new MemoryStream())
            {
                using (_writer = new StreamWriter(stream))
                {
                    if (data is System.Data.DataTable)
                    {
                        var dataTable = (System.Data.DataTable)(data);
                        _columnCount = dataTable.Columns.Count;
                        WriteDataTable(dataTable);
                    }
                    else if (data is List<List<object>>)
                    {
                        var list = (List<List<object>>)(data);
                        _columnCount = list[0].Count();

                        WriteList(list);
                    }
                    else { throw new Exception("unsupported parameter"); }
                }
                return stream.ToArray();
            }
        }

        private void WriteList(List<List<object>> data)
        {
            // header **AND** data
            foreach (var r in data) WriteRow(r.ToArray());
        }

        private void WriteDataTable(System.Data.DataTable dataTable)
        {
            // header
            WriteRow(dataTable
                .Columns.Cast<DataColumn>()
                .Select(x => x.ColumnName)
                .ToArray()
            );
            // data
            foreach (DataRow r in dataTable.Rows) WriteRow(r.ItemArray);
        }

        private void WriteRow(object[] row)
        {
            for (int i = 0; i < _columnCount; i++)
            {
                if (i > 0) _writer.Write(",");

                var val = row[i] != null ? row[i].ToString() : string.Empty;
                _writer.Write(GetCSV(val));
            }
            _writer.WriteLine();
        }
    }
}