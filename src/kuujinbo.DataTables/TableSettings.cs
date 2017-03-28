/* ============================================================================
 * get appSettings values specifically used by JqueryDataTables 
 * ============================================================================
 */
using System.Configuration;
using kuujinbo.DataTables.Utils;

namespace kuujinbo.DataTables
{
    // no reason to test ConfigurationManager.AppSettings
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed class TableSettings
    {
        // singleton
        public static readonly TableSettings Settings;

        // ISO format
        public const string DEFAULT_DATE_FORMAT = "yyyy-MM-dd";
        // replacement for true
        public const string DEFAULT_TRUE = "Yes";
        // replacement for false
        public const string DEFAULT_FALSE = "No";
        // max count from DB when saving results to a file
        public const int DEFAULT_MAX_SAVE_AS = 5000;

        // appSettings keys
        public const string BOOL_TRUE = "BoolTrue";
        public const string BOOL_FALSE = "BoolFalse";
        public const string DATE_FORMAT = "DateFormat";
        public const string MAX_SAVE_AS = "MaxSaveAsRecords";

        static TableSettings()
        {
            Settings = new TableSettings();

            var settings = ConfigurationManager.AppSettings;
            if (settings.Count > 0)
            {
                Settings.BoolTrue = !string.IsNullOrWhiteSpace(settings[BOOL_TRUE]) 
                    ? settings[BOOL_TRUE] : DEFAULT_TRUE;
                Settings.BoolFalse = !string.IsNullOrWhiteSpace(settings[BOOL_FALSE])
                    ? settings[BOOL_FALSE] : DEFAULT_FALSE;
                Settings.DateFormat =
                    !string.IsNullOrWhiteSpace(settings[DATE_FORMAT])
                    && DateFormatValidator.TryParse(settings[DATE_FORMAT])
                        ? settings[DATE_FORMAT] : DEFAULT_DATE_FORMAT;
                int maxSaveAs;
                Settings.MaxSaveAs = int.TryParse(settings[MAX_SAVE_AS], out maxSaveAs)
                    ? maxSaveAs : DEFAULT_MAX_SAVE_AS;
            }
            else
            {
                Settings.BoolTrue = DEFAULT_TRUE;
                Settings.BoolFalse = DEFAULT_FALSE;
                Settings.DateFormat = DEFAULT_DATE_FORMAT;
                Settings.MaxSaveAs = DEFAULT_MAX_SAVE_AS;
            }
        }

        public string BoolTrue { get; private set; }
        public string BoolFalse { get; private set; }
        public string DateFormat { get; private set; }
        public int MaxSaveAs { get; private set; }
    }
}