using kuujinbo.DataTables.DataTable;
using Newtonsoft.Json;
/* =======================================================================
 * 'true' and 'false' are obviously not user friendly. use converter to 
 * **serialize** (display only) bool to more meaningful values
 * =======================================================================
 */
using System;

namespace kuujinbo.DataTables.Json
{
    public class WriteBoolConverter : JsonConverter
    {
        private string _boolTrue, _boolFalse;

        public WriteBoolConverter() : this(null, null) { }
        public WriteBoolConverter(string boolTrue, string boolFalse)
        {
            _boolTrue = boolTrue;
            _boolFalse = boolFalse;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }

        // do **NOT** allow deserialization
        public override bool CanRead { get { return false; } }
        // allow serialization
        public override bool CanWrite { get { return true; } }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            Object existingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException(
                "CanRead is false. The type will skip the converter."
            );
        }

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            writer.WriteValue(
                ((bool)value) 
                ? _boolTrue ?? TableSettings.Settings.BoolTrue 
                : _boolFalse ??  TableSettings.Settings.BoolFalse
            );
        }
    }
}