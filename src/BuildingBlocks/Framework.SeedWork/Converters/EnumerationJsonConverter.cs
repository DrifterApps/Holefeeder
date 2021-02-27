using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Converters
{
    public class EnumerationJsonConverter<T> : JsonConverter<T> where T : Enumeration
    {
        private const string ID_PROPERTY = "id";
        private const string NAME_PROPERTY = "name";

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsSubclassOf(typeof(Enumeration));
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"Invalid Enumeration type {typeToConvert.FullName}");
            }

            T enumerationType = null;
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.EndObject:
                        return enumerationType;
                    case JsonTokenType.PropertyName:
                    {
                        try
                        {
                            var propertyName = reader.GetString();
                            if (propertyName?.Equals(ID_PROPERTY, StringComparison.InvariantCultureIgnoreCase) ?? false)
                            {
                                reader.Read();
                                if (reader.TokenType != JsonTokenType.Number)
                                {
                                    throw new JsonException(
                                        $"Enumeration property type {ID_PROPERTY} should be number");
                                }

                                var typeValue = reader.GetInt32();
                                var type = Enumeration.FromValue<T>(typeValue);
                                if (enumerationType != null && enumerationType != type)
                                {
                                    throw new JsonException($"Invalid {typeof(T).Name}");
                                }

                                enumerationType = type;
                            }
                            else if (propertyName?.Equals(NAME_PROPERTY, StringComparison.InvariantCultureIgnoreCase) ??
                                     false)
                            {
                                reader.Read();
                                if (reader.TokenType != JsonTokenType.String)
                                {
                                    throw new JsonException(
                                        $"Enumeration property type {NAME_PROPERTY} should be string");
                                }

                                var typeName = reader.GetString();
                                var type = Enumeration.FromName<T>(typeName);
                                if (enumerationType != null && enumerationType != type)
                                {
                                    throw new JsonException($"Invalid {typeof(T).Name}");
                                }

                                enumerationType = type;
                            }
                        }
                        catch (InvalidOperationException e)
                        {
                            throw new JsonException(e.Message, e);
                        }

                        break;
                    }
                }
            }

            throw new JsonException("Badly composed Enumeration Type");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNull(NAME_PROPERTY);
            }
            else
            {
                writer.WriteStartObject();
                writer.WriteNumber(ID_PROPERTY, value.Id);
                writer.WriteString(NAME_PROPERTY, value.Name);
                writer.WriteEndObject();
            }
        }
    }
}
