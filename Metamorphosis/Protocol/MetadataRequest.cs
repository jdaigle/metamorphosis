using System;
using System.Text;

namespace Metamorphosis.Protocol
{
    public struct MetadataRequest : IRequest
    {
        public RequestHeader Header;
        public string[] Topics;

        public void Pack(ByteBuffer buffer)
        {
            Header.ApiKey = 3;
            Header.Pack(buffer);

            if (Topics == null)
            {
                Topics = Array.Empty<string>();
            }

            // very special case for v1 of MetadataRequest API,
            // instead of an empty array, send a nullable array
            if (Topics.Length == 0 && Header.ApiVersion == 1)
            {
                KafkaBitConverter.WriteInt(buffer, -1);
            }
            else
            {
                KafkaBitConverter.WriteInt(buffer, Topics.Length);
            }

            if (Topics.Length > 0)
            {
                foreach (var topic in Topics)
                {
                    PackString(buffer, topic);
                }
            }
        }

        public void PackString(ByteBuffer buffer, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                KafkaBitConverter.WriteShort(buffer, -1);
            }
            else
            {
                var stringBuffer = Encoding.UTF8.GetBytes(value);
                KafkaBitConverter.WriteShort(buffer, (short)stringBuffer.Length);
                KafkaBitConverter.WriteBytes(buffer, stringBuffer, 0, stringBuffer.Length);
            }
        }
    }
}
