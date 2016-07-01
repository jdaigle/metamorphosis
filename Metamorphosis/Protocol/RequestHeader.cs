using System.Text;

namespace Metamorphosis.Protocol
{
    public struct RequestHeader
    {
        public short ApiKey;
        public short ApiVersion;
        public int CorrelationId;
        public string ClientId;

        public void Pack(ByteBuffer buffer)
        {
            KafkaBitConverter.WriteShort(buffer, ApiKey);
            KafkaBitConverter.WriteShort(buffer, ApiVersion);
            KafkaBitConverter.WriteInt(buffer, CorrelationId);
            if (string.IsNullOrWhiteSpace(ClientId))
            {
                KafkaBitConverter.WriteShort(buffer, -1);
            }
            else
            {
                var stringBuffer = Encoding.UTF8.GetBytes(ClientId);
                KafkaBitConverter.WriteShort(buffer, (short)stringBuffer.Length);
                KafkaBitConverter.WriteBytes(buffer, stringBuffer, 0, stringBuffer.Length);
            }
        }
    }
}
