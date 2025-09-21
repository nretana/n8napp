using System.Text.Json;
using System.Text.Json.Serialization;

namespace N8N.API.Utilities.JsonConverters
{
    public class DateOnlyConverter : JsonConverter<DateOnly>
    {
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return DateOnly.ParseExact(value, "yyyy-MM-dd");
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            var dateValue = value.ToString("yyyy-MM-dd");
            writer.WriteStringValue(dateValue);
        }
    }
}
