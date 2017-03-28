/* ============================================================================

 * ============================================================================
 */
using kuujinbo.DataTables;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace kuujinbo.DataTables.Json
{
    public class JsonNetSerializer
    {
        private JsonSerializerSettings _jsonSettings;

        private static readonly IsoDateTimeConverter _isoDateTimeConverter;
        private static readonly WriteBoolConverter _writeBoolConverter;
        static JsonNetSerializer()
        {
            _writeBoolConverter = new WriteBoolConverter(
                TableSettings.Settings.BoolTrue,
                TableSettings.Settings.BoolFalse
            );
            _isoDateTimeConverter = new IsoDateTimeConverter()
            {
                DateTimeFormat = TableSettings.Settings.DateFormat
            };
        }

        public JsonNetSerializer()
        {
            _jsonSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            _jsonSettings.Converters.Add(_isoDateTimeConverter);
        }

        public string Get(object value, bool addConverters = false)
        {
            if (value == null) throw new System.ArgumentNullException("value");

            if (addConverters)
            {
                _jsonSettings.Converters.Add(new WriteEnumConverter());
                _jsonSettings.Converters.Add(_writeBoolConverter);
            }

            return JsonConvert.SerializeObject(
                value, Formatting.Indented, _jsonSettings
            );
        }
    }
}