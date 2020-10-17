using System;
using System.Buffers;
using System.Buffers.Text;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace System.Text.Json
{
    public class GuidConvert : JsonConverter<Guid>
    {
        public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return Guid.Empty;

            if (reader.TokenType == JsonTokenType.String)
            {
                ReadOnlySpan<byte> span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
                if (Utf8Parser.TryParse(span, out Guid value, out var bytesConsumed) && span.Length == bytesConsumed)
                    return value;

                return Guid.Empty;
            }

            throw new Exception();
        }

        public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
        {
            if (Guid.Empty.Equals(value))
                writer.WriteStringValue("");
            else
                writer.WriteStringValue(value.ToString());
        }
    }

    public class decimalConvert : JsonConverter<decimal?>
    {
        public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string stringValue = reader.GetString();
                if (decimal.TryParse(stringValue, out decimal value))
                {
                    return value;
                }
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetDecimal();
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(Convert.ToDouble(value));
        }
    }

    public class datetimeConvert : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string stringValue = reader.GetString();
                if (string.IsNullOrEmpty(stringValue))
                {
                    return null;
                }
                if (reader.TryGetDateTime(out DateTime datetime))
                {
                    return datetime;
                }
            }
            else if (reader.TokenType == JsonTokenType.Null)
            {
                return reader.GetDateTime();
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(Convert.ToDateTime(value));
        }
    }
}
