using System;
using System.Globalization;
using Newtonsoft.Json;

namespace BadgeFarmer
{
    class StringToDecimalConverter:JsonConverter<decimal>
    {
        public override void WriteJson(JsonWriter writer, decimal value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

        public override decimal ReadJson(JsonReader reader, Type objectType, decimal existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var value = (string)reader.Value;

            var result = Decimal.Parse(value, NumberStyles.Any);

            return result;
        }
    }
}