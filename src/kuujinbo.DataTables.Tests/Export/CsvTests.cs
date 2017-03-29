﻿using System;
using System.Data;
using System.Linq;
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
        public void GetField_WithDoubleQuotes_InsertsDoubleQuotes()
        {
            var text = "text with \"double quotes\".";

            var result = _csv.GetField(text);

            Assert.StartsWith("\"", result);
            Assert.EndsWith("\"", result);
            // start + end + quotes (2) * 2
            Assert.Equal(6, result.Count(x => '"' == x));
        }

        [Fact]
        public void GetField_WithComma_InsertsDoubleQuotes()
        {
            var text = "text with comma,";

            var result = _csv.GetField(text);

            Assert.Equal(string.Format("\"{0}\"", text), result);
        }

        [Fact]
        public void GetField_WithNewLine_InsertsDoubleQuotes()
        {
            var text = @"text with
                        newline";

            var result = _csv.GetField(text);

            output.WriteLine(result);
            Assert.Equal(string.Format("\"{0}\"", text), result);
        }

        #endregion
    }
}