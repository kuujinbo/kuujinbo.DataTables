using kuujinbo.DataTables.DataTable;
using Xunit;

namespace kuujinbo.DataTables.Tests.DataTables
{
    public class TableSettingsTests
    {
        [Fact]
        public void StaticConstructor_WithoutAppSettings_SetsDefaultConfigValues()
        {
            var settings = TableSettings.Settings;

            Assert.Equal(TableSettings.DEFAULT_TRUE, settings.BoolTrue);
            Assert.Equal(TableSettings.DEFAULT_FALSE, settings.BoolFalse);
            Assert.Equal(TableSettings.DEFAULT_DATE_FORMAT, settings.DateFormat);
            Assert.Equal(TableSettings.DEFAULT_MAX_SAVE_AS, settings.MaxSaveAs);
        }
    }
}