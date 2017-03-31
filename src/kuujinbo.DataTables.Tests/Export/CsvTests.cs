using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using kuujinbo.DataTables.Export;
using Xunit;
using Xunit.Abstractions;

namespace kuujinbo.DataTables.Tests.Export
{
    public class CsvTests : IDisposable
    {
        #region test class
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
        #endregion


        #region tests
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
        public void GetField_NoSpecialCharacters_IsNoOp()
        {
            var text = "only alphabetic text";

            var result = _csv.GetField(text);

            Assert.Equal<string>(text, result);
        }

        [Fact]
        public void GetField_DoubleQuotes_InsertsDoubleQuotes()
        {
            var text = "text with \"double quotes\".";

            var result = _csv.GetField(text);

            Assert.StartsWith("\"", result);
            Assert.EndsWith("\"", result);
            // start + end + quotes (2) * 2
            Assert.Equal(6, result.Count(x => '"' == x));
        }

        [Fact]
        public void GetField_Comma_InsertsDoubleQuotes()
        {
            var text = "text with comma,";

            var result = _csv.GetField(text);

            Assert.Equal(string.Format("\"{0}\"", text), result);
        }

        [Fact]
        public void GetField_NewLine_InsertsDoubleQuotes()
        {
            var text = @"text with
                        newline";

            var result = _csv.GetField(text);

            output.WriteLine(result);
            Assert.Equal(string.Format("\"{0}\"", text), result);
        }

        [Fact]
        public void Export_NullParameter_Throws()
        {
            var exception = Assert.Throws<ArgumentException>(
                 () => _csv.Export(null)
             );
        }

        [Fact]
        public void Export_BadParameter_Throws()
        {
            var exception = Assert.Throws<ArgumentException>(
                 () => _csv.Export("string")
             );
        }

        [Fact]
        public void Export_DataTable_ReturnsByteArray()
        {
            var dt = new DataTable();
            var headers = new string[] { "Header 1", "Header 2" };
            dt.Columns.Add(new DataColumn(headers[0], typeof(int)));
            dt.Columns.Add(new DataColumn(headers[1], typeof(float)));
            var data = new object[] { 0, 0.0004f };
            var dr = dt.NewRow();
            dr.ItemArray = data;
            dt.Rows.Add(dr);

            var result = _csv.Export(dt);
            var stringExpected = string.Format(
                "{0}{1}{2}{1}",
                string.Join(",", headers),
                Environment.NewLine, 
                string.Join(",", data)
            );

            Assert.NotNull(result);
            Assert.True(result is byte[]);
            Assert.Equal(stringExpected, Encoding.UTF8.GetString(result));
        }

        [Fact]
        public void Export_ListOfList_ReturnsByteArray()
        {
            var list = new List<List<object>>() { new List<object>() { 1, null, "1" } };

            var result = _csv.Export(list);

            Assert.NotNull(result);
            Assert.True(result is byte[]);
            Assert.Equal(
                string.Join(",", list[0].ToArray()) + Environment.NewLine, 
                Encoding.UTF8.GetString(result)
            );
        }
        #endregion
    }
}