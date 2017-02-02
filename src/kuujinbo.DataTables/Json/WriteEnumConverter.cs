/* =======================================================================
 * make enums more end-user friendly
 * =======================================================================
 */
using kuujinbo.DataTables.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace kuujinbo.DataTables.Json
{
    public class WriteEnumConverter : StringEnumConverter
    {
        public override void WriteJson(
            JsonWriter writer, 
            object value, 
            JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteValue(EnumUtils.DisplayText(value));
            }
        }
    }
}