using System;
using System.Linq;
using kuujinbo.DataTables.Json;
using Xunit;

namespace kuujinbo.DataTables.Tests.Json
{
    public class JsonNetSerializerTests
    {
        private JsonNetSerializer _jsonNetSerializer;
        private DateTime _date;
        private static readonly char[] DATE_SPLIT_CHAR = new char[] { '-' };

        public JsonNetSerializerTests()
        {
            _jsonNetSerializer = new JsonNetSerializer();
            _date = DateTime.Now;
        }

        [Fact]
        public void Get_NullObject_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => _jsonNetSerializer.Get(null)
            );

            Assert.Equal<string>("value", exception.ParamName);
        }

        [Fact]
        public void Get_DateTime_ReturnsIsoStringDate()
        {
            var _date = DateTime.Now;

            var json = _jsonNetSerializer.Get(_date);
            var dateParts = json.Trim(new char[] { '"' })
                .Split(DATE_SPLIT_CHAR, StringSplitOptions.None)
                .Select(x => Int32.Parse(x))
                .ToArray();

            Assert.Equal(3, dateParts.Length);
            Assert.Equal(_date.Year, dateParts[0]);
            Assert.Equal(_date.Month, dateParts[1]);
            Assert.Equal(_date.Day, dateParts[2]);
        }

        public class TestClass 
        {
            public string Name { get; set; }
            public TestEnum TestEnum { get; set; }
        }
        public enum TestEnum { TestEnum }

        [Fact]
        public void SimpleEnumConverter_WriteJsonNull_WritesNullString()
        {
            Assert.Equal<string>(
                "\"Test Enum\"", _jsonNetSerializer.Get(TestEnum.TestEnum, true)
            );
        }
    }
}