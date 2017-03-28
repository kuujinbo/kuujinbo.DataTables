using System;
using System.Data;
using Xunit;
using Xunit.Abstractions;

namespace kuujinbo.DataTables.Tests.Export
{
    public class CsvTests : IDisposable
    {
        private static readonly object[] _data = { 0, 1 };
        private readonly ITestOutputHelper output;
        private DataTable _dataTable;

        public CsvTests(ITestOutputHelper output)
        {
            _dataTable = new DataTable();
            foreach (var i in _data)
            {
                _dataTable.Columns.Add(((int)i).ToString("D2"), typeof(int)); ;
            }
            DataRow row = _dataTable.NewRow();
            row.ItemArray = _data;
            _dataTable.Rows.Add(row);

            this.output = output;
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
    }
}