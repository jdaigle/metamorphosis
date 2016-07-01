using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Metamorphosis.Protocol;

namespace Metamorphosis.Console
{
    static class Program
    {
        private static void Main(string[] args)
        {
            DoWork("10.211.55.2", 9092).GetAwaiter().GetResult();
        }

        private static async Task DoWork(string host, int port)
        {
            IPAddress[] ipAddresses;
            IPAddress ip;
            if (IPAddress.TryParse(host, out ip))
            {
                ipAddresses = new IPAddress[] { ip };
            }
            else
            {
                //trace.Debug("Resolving DNS for Host '{0}'", host);
                ipAddresses = await GetHostAddressesAsync(host);
            }

            var socket = new Socket(ipAddresses[0].AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            //await TcpSocket.ConnectAsync(socket, ipAddresses[0], port);
            socket.Connect(ipAddresses[0], port);

            var buffer = new ByteBuffer(new byte[64 * 1024], 0, 0, 64 * 1024);

            var frameStartOffset = buffer.WriteOffset;
            KafkaBitConverter.WriteInt(buffer, 0); // Size

            var metadataRequest = new MetadataRequest();
            metadataRequest.Header.ApiVersion = 1;
            metadataRequest.Header.CorrelationId = 100;
            metadataRequest.Pack(buffer);

            var requestLength = buffer.WriteOffset - frameStartOffset - KafkaBitConverter.FixedWidth.Int;
            KafkaBitConverter.WriteInt(buffer.Buffer, frameStartOffset, requestLength);

            socket.SendBufferSize = 1024;
            socket.SendTimeout = 1000;
            var bytesSent = socket.Send(buffer.Buffer, buffer.ReadOffset, buffer.LengthAvailableToRead, SocketFlags.None);

            buffer.ResetReadWrite();

            var bytesRead = socket.Receive(buffer.Buffer, 0, 64 * 1024, SocketFlags.None);
            buffer.AppendWrite(bytesRead);

            var responseSize = KafkaBitConverter.ReadInt(buffer);
            var correlationId = KafkaBitConverter.ReadInt(buffer);
            var brokerCount = KafkaBitConverter.ReadInt(buffer);
            for (int i = 0; i < brokerCount; i++)
            {
                var broker_nodeId = KafkaBitConverter.ReadInt(buffer);
                var broker_hostLength = KafkaBitConverter.ReadShort(buffer);
                if (broker_hostLength > 0)
                {
                    var host_NameBuffer = new byte[broker_hostLength];
                    KafkaBitConverter.ReadBytes(buffer, host_NameBuffer, 0, broker_hostLength);
                    var host_name = Encoding.ASCII.GetString(host_NameBuffer);
                }
                var broker_port = KafkaBitConverter.ReadInt(buffer);
                var broker_rack_lenght = KafkaBitConverter.ReadShort(buffer);
                if (broker_rack_lenght > 0)
                {
                    var host_NameBuffer = new byte[broker_rack_lenght];
                    KafkaBitConverter.ReadBytes(buffer, host_NameBuffer, 0, broker_rack_lenght);
                    var host_name = Encoding.ASCII.GetString(host_NameBuffer);
                }
            }
            var controller_id = KafkaBitConverter.ReadInt(buffer);
            var topicCount = KafkaBitConverter.ReadInt(buffer);
            for (int i = 0; i < topicCount; i++)
            {
                var topic_errorCode = KafkaBitConverter.ReadShort(buffer);
                var topic_namelength = KafkaBitConverter.ReadShort(buffer);
                if (topic_namelength > 0)
                {
                    var topic_NameBuffer = new byte[topic_namelength];
                    KafkaBitConverter.ReadBytes(buffer, topic_NameBuffer, 0, topic_namelength);
                    var topic_name = Encoding.ASCII.GetString(topic_NameBuffer);
                }
                var topic_internal = KafkaBitConverter.ReadByte(buffer);
                var topic_partitionCount = KafkaBitConverter.ReadInt(buffer);
                for (int j = 0; j < topic_partitionCount; j++)
                {
                    var partitionErrorCode = KafkaBitConverter.ReadShort(buffer);
                    var partitionId = KafkaBitConverter.ReadInt(buffer);
                    var partitionLeader = KafkaBitConverter.ReadInt(buffer);
                    var partitionReplicasCount = KafkaBitConverter.ReadInt(buffer);
                    for (int k = 0; k < partitionReplicasCount; k++)
                    {
                        var partitionReplicaId = KafkaBitConverter.ReadInt(buffer);
                    }
                    var partitionIsrCount = KafkaBitConverter.ReadInt(buffer);
                    for (int k = 0; k < partitionReplicasCount; k++)
                    {
                        var partitionIsrId = KafkaBitConverter.ReadInt(buffer);
                    }
                }
            }
        }

        private static Task<IPAddress[]> GetHostAddressesAsync(string host)
        {
            return Task.Factory.FromAsync(
                (c, s) => Dns.BeginGetHostAddresses(host, c, s),
                (r) => Dns.EndGetHostAddresses(r),
                null);
        }
    }
}
