using System;
using System.Data;
using kuujinbo.DataTables.Export;
using Xunit;
using Xunit.Abstractions;

namespace kuujinbo.DataTables.Tests.Export
{
    public class CsvTests : IDisposable
    {
        private Csv _csv;
        private static readonly object[] _data = { 0, 1 };
        private readonly ITestOutputHelper output;
        private DataTable _dataTable;

        public CsvTests(ITestOutputHelper output)
        {
            _csv = new Csv();
            this.output = output;

            _dataTable = new DataTable();
            foreach (var i in _data)
            {
                _dataTable.Columns.Add(((int)i).ToString("D2"), typeof(int)); ;
            }
            DataRow row = _dataTable.NewRow();
            row.ItemArray = _data;
            _dataTable.Rows.Add(row);
        }

        public void Dispose()
        {
            _dataTable.Dispose();
        }


        [Fact]
        public void Constructor_Parameterless_InitializesData()
        {
            var length = _data.Length;

            Assert.NotNull(_dataTable);
            Assert.Equal<int>(length, _dataTable.Columns.Count);
            for (var i = 0; i < length; ++i)
            {
                Assert.Equal(_data[i], _dataTable.Rows[0][i]);
            }
        }

        [Fact]
        public void GetCSV_NoDoubleQuotes_IsNoOp()
        {
            var text = "text without double quotes";

            var result = _csv.GetCSV(text);

            Assert.Equal<string>(text, result);
        }

        [Fact]
        public void GetCSV_DoubleQuotes_EscapesQuotes()
        {
            var text = "text with \"double quotes\"";

            var result = _csv.GetCSV(text);

            Assert.Equal<string>(string.Format("\"{0}\"", text), result);
        }
    }
}