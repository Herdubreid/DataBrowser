using System;
using System.Collections;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataBrowser.Features
{
    public class DynamicJsonConverter : JsonConverter<DynamicJsonElement>
    {
        public override DynamicJsonElement Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new DynamicJsonElement(JsonSerializer.Deserialize<JsonElement>(ref reader, options));
        }

        public override void Write(Utf8JsonWriter writer, DynamicJsonElement value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
    public class DynamicJsonElement : DynamicObject, IEnumerable
    {
        public JsonElement Element { get; }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            try
            {
                result = Element.GetProperty(binder.Name);
            }
            catch
            {
                result = null;
                return false;
            }
            return true;
        }
        public JsonElement.ObjectEnumerator GetEnumerator()
        {
            return Element.EnumerateObject();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public DynamicJsonElement(JsonElement element)
        {
            Element = element;
        }
    }
}
