using System;
using kuujinbo.DataTables.DataTable;
using kuujinbo.DataTables.Json;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace kuujinbo.DataTables.Tests.Json
{
    public class WriteBoolConverterTests
    {
        private WriteBoolConverter _converter;

        public WriteBoolConverterTests()
        {
            _converter = new WriteBoolConverter();
        }

        [Fact]
        public void CanConvert_TypeParameter_OnlyConvertsBool()
        {
            Assert.False(_converter.CanConvert(typeof(int)));
            Assert.False(_converter.CanConvert(typeof(string)));
            Assert.True(_converter.CanConvert(typeof(bool)));
        }

        [Fact]
        public void CanRead_TypeParameter_ReturnsFalse()
        {
            Assert.False(_converter.CanRead);
        }

        [Fact]
        public void CanWrite_ReturnsTrue()
        {
            Assert.True(_converter.CanWrite);
        }

        [Fact]
        public void ReadJson_ThrowsNotImplementedException()
        {
            var reader = new Mock<JsonReader>();

            var exception = Assert.Throws<NotImplementedException>(
                () => _converter.ReadJson(
                    reader.Object, typeof(bool), null, new JsonSerializer()
                )
            );
        }

        [Fact]
        public void WriteJson_Defaults_WritesValues()
        {
            string jsonTrue = JsonConvert.SerializeObject(
                true, Formatting.None, _converter
            ).Replace("\"", "");
            string jsonFalse = JsonConvert.SerializeObject(
                false, Formatting.None, _converter
            ).Replace("\"", "");

            Assert.Equal(TableSettings.DEFAULT_TRUE, jsonTrue);
            Assert.Equal(TableSettings.DEFAULT_FALSE, jsonFalse);
        }

        [Fact]
        public void WriteJson_InstantiatingOverloadedConstructor_WritesTrueAndFalseValues()
        {
            _converter = new WriteBoolConverter("Y", "N");
            string jsonTrue = JsonConvert.SerializeObject(
                true, Formatting.None, _converter
            ).Replace("\"", "");
            string jsonFalse = JsonConvert.SerializeObject(
                false, Formatting.None, _converter
            ).Replace("\"", "");

            Assert.Equal("Y", jsonTrue);
            Assert.Equal("N", jsonFalse);
        }
    }
}