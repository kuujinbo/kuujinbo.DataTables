using kuujinbo.DataTables.Utils;
using Xunit;

namespace kuujinbo.DataTables.Tests.Utils
{
    public class RegexUtilsTests
    {
        [Fact]
        public void PascalCaseSplit_MatchingString_InsertsSpaces()
        {
            Assert.Equal<string>(
                "Hello World World",
                RegexUtils.PascalCaseSplit("HelloWorldWorld")
            );
        }

        [Fact]
        public void PascalCaseSplit_MatchingString_IsNoOp()
        {
            string expected = "TH T7 T^ jk $# 08 j$0";

            Assert.Equal<string>(
                expected,
                RegexUtils.PascalCaseSplit(expected)
            );
        }

        [Fact]
        public void PascalCaseSplit_NullString_ReturnsNull()
        {
            Assert.Null(RegexUtils.PascalCaseSplit(null));
        }
    }
}