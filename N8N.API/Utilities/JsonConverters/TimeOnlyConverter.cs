using System.Text.Json;
using System.Text.Json.Serialization;

namespace N8N.API.Utilities.JsonConverters
{
    public class TimeOnlyConverter : JsonConverter<TimeOnly>
    {
        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var timeValue = reader.GetString();
            return TimeOnly.ParseExact(timeValue, "HH:mm");
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            var currentTime = value.ToString("HH:mm");
            writer.WriteStringValue(currentTime);
        }
    }
}
