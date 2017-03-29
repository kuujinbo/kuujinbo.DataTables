using kuujinbo.DataTables.Utils;
using Xunit;

namespace kuujinbo.DataTables.Tests.Utils
{
    public class DateFormatValidatorTests
    {
        [Fact]
        public void TryParse_InvalidFormat_ReturnsFalse()
        {
            Assert.False(DateFormatValidator.TryParse("invalid"));
        }

        [Fact]
        public void TryParse_ValidFormat_ReturnsTrue()
        {
            Assert.True(DateFormatValidator.TryParse(TableSettings.DEFAULT_DATE_FORMAT));
        }
    }
}
