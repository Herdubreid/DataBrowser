using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataBrowser.Features
{
    public class ObjectArrayConverter : JsonConverter<object[]>
    {
        public override object[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var json = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
            List<object> els = new List<object>();
            foreach (var el in json.EnumerateObject())
            {
                int i;
                switch (el.Value.ValueKind)
                {
                    case JsonValueKind.String:
                        els.Add(el.Value.GetString());
                        break;
                    case JsonValueKind.Number:
                        if (el.Value.TryGetInt32(out i)) els.Add(i);
                        else els.Add(el.Value.GetDecimal());
                        break;
                    case JsonValueKind.Object:
                        if (el.Name.Equals("groupBy"))
                        {
                            foreach (var sel in el.Value.EnumerateObject())
                            {
                                if (sel.Value.ValueKind == JsonValueKind.String)
                                {
                                    els.Add(sel.Value.GetString());
                                }
                                else
                                {
                                    if (sel.Value.TryGetInt32(out i)) els.Add(i);
                                    else els.Add(sel.Value.GetDecimal());
                                }
                            }
                        }
                        else if (el.Value.TryGetProperty("internalValue", out var value))
                        {
                            if (value.ValueKind == JsonValueKind.String)
                            {
                                els.Add(el.Value.GetString());
                            }
                            else
                            {
                                if (el.Value.TryGetInt32(out i)) els.Add(i);
                                else els.Add(el.Value.GetDecimal());
                            }
                        }
                        break;
                }
            }
            return els.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, object[] value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
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
    public class DynamicJsonElement : DynamicObject
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
        public DynamicJsonElement(JsonElement element)
        {
            Element = element;
        }
    }
}
