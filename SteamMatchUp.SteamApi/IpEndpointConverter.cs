using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Net;

namespace SteamMatchUp.SteamApi
{
    public class IpEndpointConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IPEndPoint);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (!(reader.Value is string))
                throw new ArgumentOutOfRangeException("reader", reader.Value, "Value must be a string");

            var text = (string)reader.Value;

            var colonIndex = text.IndexOf(':');
            if (colonIndex == -1)
                throw new ArgumentOutOfRangeException("text", text, "Text is not a valid ip endpoint");

            var ipaddress = text.Substring(0, colonIndex);
            var port = text.Substring(colonIndex + 1);

            return new IPEndPoint(IPAddress.Parse(ipaddress), int.Parse(port));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
