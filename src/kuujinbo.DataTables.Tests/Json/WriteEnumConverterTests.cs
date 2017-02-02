using kuujinbo.DataTables.Json;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace kuujinbo.DataTables.Tests.Json
{
    public class WriteEnumConverterTests
    {
        enum TestEnum { Zero, OneTwo }
        WriteEnumConverter _converter;
        string _json;

        public WriteEnumConverterTests()
        {
            _converter = new WriteEnumConverter();
        }

        [Fact]
        public void WriteJson_NullValue_WritesStringEmpty()
        {
            var writer = new Mock<JsonWriter>();
            _converter.WriteJson(writer.Object, null, null);

            _json = JsonConvert.SerializeObject(
                null, Formatting.None, _converter
            );

            writer.Verify(x => x.WriteNull(), Times.Once());
            Assert.Equal("null", _json);
        }

        [Fact]
        public void WriteJson_EnumValue_WritesEnum()
        {
            _json = JsonConvert.SerializeObject(
                TestEnum.Zero, Formatting.None, _converter
            ).Replace("\"", "");

            Assert.Equal(TestEnum.Zero.ToString(), _json);
        }

        [Fact]
        public void WriteJson_PascalCaseEnum_WritesEnumWithSpaces()
        {
            _json = JsonConvert.SerializeObject(
                TestEnum.OneTwo, Formatting.None, _converter
            ).Replace("\"", "");

            Assert.Equal("One Two", _json);
        }
    }
}